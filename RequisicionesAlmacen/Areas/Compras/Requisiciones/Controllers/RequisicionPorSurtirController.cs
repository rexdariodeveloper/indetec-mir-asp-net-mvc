using Newtonsoft.Json;
using RequisicionesAlmacen.Areas.Compras.Requisiciones.Models;
using RequisicionesAlmacen.Areas.Compras.Requisiciones.Models.ViewModel;
using RequisicionesAlmacen.Controllers;
using RequisicionesAlmacen.Helpers;
using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Models.Mapeos;
using RequisicionesAlmacenBL.Services;
using RequisicionesAlmacenBL.Services.Compras;
using RequisicionesAlmacenBL.Services.SAACG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static RequisicionesAlmacenBL.Models.Mapeos.ControlMaestroMapeo;

namespace RequisicionesAlmacen.Areas.Compras.Requisiciones.Controllers
{
    [Authenticated(nodoMenuId = MenuPrincipalMapeo.ID.REQUISICION_POR_SURTIR)]
    public class RequisicionPorSurtirController : BaseController<ARtblRequisicionMaterial, RequisicionPorSurtirViewModel>
    {
        private string API_FICHA = "/compras/requisiciones/requisicionporsurtir/";

        public override ActionResult Nuevo()
        {
            throw new NotImplementedException();
        }

        public override ActionResult Editar(int id)
        {
            // Creamos el objeto
            RequisicionPorSurtirViewModel viewModel = new RequisicionPorSurtirViewModel();

            //Buscamos el Objeto por el Id que se envio como parametro
            ARtblRequisicionMaterial requisicionMaterial = new RequisicionMaterialService().BuscaPorId(id);

            //Asignamos el modelo
            viewModel.RequisicionMaterial = requisicionMaterial != null ? requisicionMaterial : new ARtblRequisicionMaterial();

            if (requisicionMaterial == null)
            {
                //Asignamos el error
                SetViewBagError("La Requisición no existe o está Cancelada. Favor de revisar.", API_FICHA + "listar");
            }
            else if (requisicionMaterial.EstatusId != AREstatusRequisicion.AUTORIZADA
                && requisicionMaterial.EstatusId != AREstatusRequisicion.EN_PROCESO
                && requisicionMaterial.EstatusId != AREstatusRequisicion.EN_ALMACEN)
            {
                //Asignamos el error
                SetViewBagError("La Requisición no está Autorizada, En Proceso o En Almacén. Favor de revisar.", API_FICHA + "listar");
            }
            else
            {
                //Asignamos los detalles
                viewModel.ListRequisicionMaterialDetalles = new RequisicionPorSurtirService().BuscaDetallesPorRequisicionMaterialId(id);

                //Asignamos las existencias
                viewModel.ListExistencias = new RequisicionPorSurtirService().BuscaExistenciaProducto(id);

                //Agregamos todos los datos necesarios para el funcionamiento de la ficha
                //como son los Listados para combos, tablas, arboles.
                GetDatosFicha(ref viewModel);
            }

            //Retornamos la vista junto con su Objeto Modelo
            return View("RequisicionPorSurtir", viewModel);
        }

        [JsonException]
        public override JsonResult Guardar(ARtblRequisicionMaterial requisicion)
        {
            throw new NotImplementedException();
        }

        [JsonException]
        public JsonResult GuardaCambios(ARtblRequisicionMaterial requisicion, 
                                        List<RequisicionDetalleSurtirItem> movimientos,
                                        List<ARspConsultaRequisicionPorSurtirDetalles_Result> detallesRevision)
        {
            int usuarioId = SessionHelper.GetUsuario().UsuarioId;
            DateTime fecha = DateTime.Now; 
            
            RequisicionMaterialService requisicionService = new RequisicionMaterialService();

            ARtblRequisicionMaterial temp = requisicionService.BuscaPorId(requisicion.RequisicionMaterialId);

            if (!StructuralComparisons.StructuralEqualityComparer.Equals(requisicion.Timestamp, temp.Timestamp))
            {
                throw new Exception("La Requisición con el código [" + requisicion.CodigoRequisicion + "] ha sido modificada por otro usuario. Favor de recargar la vista antes de guardar.");
            }

            List<ARtblRequisicionMaterialDetalle> detalles = new List<ARtblRequisicionMaterialDetalle>();

            if (movimientos != null)
            {
                foreach (RequisicionDetalleSurtirItem movimiento in movimientos)
                {
                    ARtblRequisicionMaterialDetalle detalleTemp = new ARtblRequisicionMaterialDetalle().GetModelo(requisicionService.BuscaDetallePorId(movimiento.RequisicionMaterialDetalleId));

                    detalleTemp.EstatusId = detalleTemp.Cantidad == movimiento.CantidadSurtir ? AREstatusRequisicionDetalle.SURTIDO : AREstatusRequisicionDetalle.SURTIDO_PARCIAL;
                    detalleTemp.ModificadoPorId = usuarioId;
                    detalleTemp.FechaUltimaModificacion = fecha;

                    detalles.Add(detalleTemp);
                }
            }

            if (detallesRevision != null)
            {
                foreach (ARspConsultaRequisicionPorSurtirDetalles_Result detalle in detallesRevision)
                {
                    ARtblRequisicionMaterialDetalle detalleTemp = new ARtblRequisicionMaterialDetalle().GetModelo(requisicionService.BuscaDetallePorId(detalle.RequisicionMaterialDetalleId));

                    detalleTemp.EstatusId = detalle.Revision.GetValueOrDefault() ? AREstatusRequisicionDetalle.REVISION : AREstatusRequisicionDetalle.POR_COMPRAR;
                    detalleTemp.ModificadoPorId = usuarioId;
                    detalleTemp.FechaUltimaModificacion = fecha;

                    detalles.Add(detalleTemp);
                }
            }

            new RequisicionPorSurtirService().GuardaCambios(temp.RequisicionMaterialId, temp.CodigoRequisicion, movimientos, detalles, usuarioId);

            //Retornamos un mensaje de Exito si todo salio correctamente
            return Json("Inventario afectado con Exito!");
        }

        public override JsonResult Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        public override ActionResult Listar()
        {
            RequisicionPorSurtirViewModel viewModel = new RequisicionPorSurtirViewModel();

            viewModel.ListRequisicionPorSurtir = new RequisicionPorSurtirService().BuscaListado();
            
            return View("ListadoRequisicionPorSurtir", viewModel);
        }

        protected override void GetDatosFicha(ref RequisicionPorSurtirViewModel viewModel)
        {
            ARtblRequisicionMaterial requisicion = viewModel.RequisicionMaterial;

            GRtblUsuario usuario = new UsuarioService().BuscaPorId(requisicion.CreadoPorId);
            tblDependencia area = new DependenciaService().BuscaDependenciaPorId(requisicion.AreaId);

            viewModel.Solicitante = new RequisicionMaterialService().GetNombreCompletoEmpleado(usuario.EmpleadoId.GetValueOrDefault());
            viewModel.Area = area.DependenciaId + " - " + area.Nombre;
            viewModel.Fecha = new RequisicionMaterialService().GetFechaConFormato(requisicion.FechaRequisicion);
            viewModel.Estatus = new ControlMaestroService().BuscaPorId(requisicion.EstatusId).Valor;
        }

        public ActionResult RptSurtidoSolicitud(int id)
        {
            ReportHelper reportHelper = new ReportHelper();

            Dictionary<string, object> parametros = new Dictionary<string, object>();
            parametros.Add("@pNombreEnte", "El ente de aqui de Jalisco");
            parametros.Add("@pEstado", "Jalisco");
            parametros.Add("@pTituloReporte", "Surtido de Solicitud de Materiales");
            parametros.Add("@pRequisicionId", id);
            parametros.Add("@pUsuarioId", SessionHelper.GetUsuario().UsuarioId);

            ViewBag.WebReport = reportHelper.GetWebReport("Almacen/RequisicionMaterial/ARrptSurtidoSolicitud.frx", parametros);

            return View("RptSurtidoSolicitud");
        }
    }
}