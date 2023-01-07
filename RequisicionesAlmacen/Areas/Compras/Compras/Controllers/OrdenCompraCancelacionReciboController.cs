﻿using Newtonsoft.Json;
using RequisicionesAlmacen.Areas.Compras.Compras.Models;
using RequisicionesAlmacen.Areas.Compras.Compras.Models.ViewModel;
using RequisicionesAlmacen.Controllers;
using RequisicionesAlmacen.Helpers;
using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Models.Mapeos;
using RequisicionesAlmacenBL.Services;
using RequisicionesAlmacenBL.Services.Compras;
using RequisicionesAlmacenBL.Services.SAACG;
using SACG.sysSacg.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static RequisicionesAlmacenBL.Models.Mapeos.ControlMaestroMapeo;

namespace RequisicionesAlmacen.Areas.Compras.Compras.Controllers
{
    [Authenticated(nodoMenuId = MenuPrincipalMapeo.ID.CANCELACION_RECIBO_OC)]
    public class OrdenCompraCancelacionReciboController : BaseController<tblCompra, OrdenCompraCancelacionReciboViewModel>
    {
        private string API_FICHA = "/compras/compras/ordencompracancelacionrecibo/";

        public override ActionResult Nuevo()
        {
            throw new NotImplementedException();
        }

        public override ActionResult Editar(int id)
        {
            OrdenCompraReciboService service = new OrdenCompraReciboService();

            //Creamos el objeto
            OrdenCompraCancelacionReciboViewModel viewModel = new OrdenCompraCancelacionReciboViewModel();

            //Buscamos el Objeto por el Id que se envio como parametro
            tblCompra compra = service.BuscaPorId(id);

            //Asignamos el Objeto al ViewModel
            viewModel.Compra = compra != null ? compra : new tblCompra();

            if (compra == null || compra.Status.Equals(EstatusOrdenCompraRecibo.CANCELADO))
            {
                //Asignamos el error
                SetViewBagError("El recibo no existe o está Cancelado. Favor de revisar.", API_FICHA + "listar");
            }
            else
            {
                //Asignamos la fecha del Recibo y de Cancelación
                compra.FechaRecibo = new OrdenCompraService().GetFechaConFormato(compra.Fecha);
                compra.FechaCancelacion = DateTime.Now;

                //Asignamos los detalles
                List<ARspConsultaOrdenCompraReciboDetalles_Result> detalles = service.BuscaDetallesPorCompraId(compra.CompraId);
                viewModel.ListOrdenCompraReciboDetalles = detalles.ConvertAll(new Converter<ARspConsultaOrdenCompraReciboDetalles_Result, OrdenCompraReciboDetalleItem>(OrdenCompraReciboDetalleItem.ConvertTo));

                //Agregamos todos los datos necesarios para el funcionamiento de la ficha
                //como son los Listados para combos, tablas, arboles.
                GetDatosFicha(ref viewModel);

                //Validamos que el recibo tenga detalles relacionados
                if (viewModel.ListOrdenCompraReciboDetalles.Count == 0)
                {
                    //Asignamos el error
                    SetViewBagError("El recibo no tiene detalles. Favor de revisar.", API_FICHA + "listar");
                }
                else
                {
                    //Buscamos los datos de la Orden de Compra
                    ARspConsultaDatosOCReciboPorId_Result ordenCompra = service.BuscaDatosOCReciboPorId(viewModel.ListOrdenCompraReciboDetalles[0].OrdenCompraId);

                    if (ordenCompra != null)
                    {
                        viewModel.Compra.OrdenCompraId = ordenCompra.OrdenCompraId;
                        viewModel.Compra.FechaOC = ordenCompra.FechaOC;
                        viewModel.Compra.EstatusOC = ordenCompra.EstatusOC;
                        viewModel.Compra.StatusOC = ordenCompra.Status;
                    }
                }
            }

            //Modo Solo Lectura
            viewModel.SoloLectura = false;

            //Retornamos la vista junto con su Objeto Modelo
            return View("OrdenCompraCancelacionRecibo", viewModel);
        }

        [JsonException]
        public override JsonResult Guardar(tblCompra ordenCompra)
        {
            throw new NotImplementedException();
        }

        [JsonException]
        public JsonResult GuardaCambios(int ordenCompraId, 
                                        string statusOC, 
                                        tblCompra compra, 
                                        List<OrdenCompraReciboDetalleItem> detalles)
        {
            int usuarioId = SessionHelper.GetUsuario().UsuarioId;
            DateTime fecha = DateTime.Now;

            OrdenCompraReciboService service = new OrdenCompraReciboService();

            //Cambiamos el Estatus a Cancelado
            compra.Status = EstatusOrdenCompraRecibo.CANCELADO;

            //Verificamos que no se haya modidicado la OC
            tblOrdenCompra temp = new OrdenCompraService().BuscaPorId(ordenCompraId);

            if (!statusOC.Equals(temp.Status))
            {
                throw new Exception("La OC con el código [" + temp.OrdenCompraId + "] ha sido modificada por otro usuario. Favor de recargar la vista antes de guardar.");
            }

            //Creamos la info de la Cancelación
            ARtblCompraCancelInfo cancelInfo = new ARtblCompraCancelInfo();
            cancelInfo.CompraId = compra.CompraId;
            cancelInfo.FechaCancelacion = compra.FechaCancelacion;
            cancelInfo.Motivo = compra.MotivoCancelacion;

            //Verificamos que no se haya modificado la Requisición
            RequisicionMaterialService requisicionService = new RequisicionMaterialService();

            List<int> requisicionesIds = new List<int>();
            List<ARtblRequisicionMaterialDetalle> detallesRecibidos = new List<ARtblRequisicionMaterialDetalle>();

            foreach (OrdenCompraReciboDetalleItem detalle in detalles)
            {
                if (detalle.RequisicionMaterialId != null)
                {
                    ARtblRequisicionMaterial requisicionTemp = requisicionService.BuscaPorId(detalle.RequisicionMaterialId.GetValueOrDefault());
                    ARtblRequisicionMaterialDetalle detalleTemp = requisicionService.BuscaDetallePorId(detalle.RequisicionMaterialDetalleId.GetValueOrDefault());

                    if (!StructuralComparisons.StructuralEqualityComparer.Equals(detalle.RequisicionTimestamp, requisicionTemp.Timestamp)
                            || !StructuralComparisons.StructuralEqualityComparer.Equals(detalle.DetalleTimestamp, detalleTemp.Timestamp))
                    {
                        throw new Exception("La Requisición con el código [" + detalle.Solicitud + "] ha sido modificada por otro usuario. Favor de recargar la vista antes de guardar.");
                    }

                    if (!requisicionesIds.Contains(requisicionTemp.RequisicionMaterialId))
                    {
                        requisicionesIds.Add(requisicionTemp.RequisicionMaterialId);
                    }

                    detalleTemp.EstatusId = AREstatusRequisicionDetalle.RELACIONADO_OC;
                    detalleTemp.FechaUltimaModificacion = fecha;
                    detalleTemp.ModificadoPorId = usuarioId;

                    detallesRecibidos.Add(new ARtblRequisicionMaterialDetalle().GetModelo(detalleTemp));
                }
            }

            List<tblCompraDet> detallesTemp = detalles.ConvertAll(new Converter<OrdenCompraReciboDetalleItem, tblCompraDet>(OrdenCompraReciboDetalleItem.ConvertTo));

            int compraId = service.GuardaCambios(ordenCompraId,
                                           usuarioId, 
                                           compra,
                                           detallesTemp,
                                           requisicionesIds,
                                           detallesRecibidos,
                                           true,
                                           cancelInfo);

            //Retornamos un mensaje de Exito si todo salio correctamente
            return Json(compraId, "Registro guardado con Exito!");
        }

        public override JsonResult Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        public override ActionResult Listar()
        {
            OrdenCompraCancelacionReciboViewModel viewModel = new OrdenCompraCancelacionReciboViewModel();

            viewModel.ListOrdenCompraRecibo = new OrdenCompraReciboService().BuscaListado();
            
            return View("ListadoOrdenCompraCancelacionRecibo", viewModel);
        }

        protected override void GetDatosFicha(ref OrdenCompraCancelacionReciboViewModel viewModel)
        {
            //Datos de OC            
            viewModel.ListOrdenesCompra = new OrdenCompraReciboService().BuscaComboOrdenesCompra();
            viewModel.ListProveedores = new ProveedorService().BuscaTodos();
            viewModel.ListAlmacenes = new AlmacenService().BuscaTodos();

            //Datos de Recibo
            viewModel.Compra.Estatus = EstatusOrdenCompraRecibo.Nombre[viewModel.Compra.Status];

            //Datos Detalles
            viewModel.ListTarifasImpuesto = new TarifaImpuestoService().BuscaTodos();

            //Ejercicio para los campos de Fecha
            viewModel.EjercicioUsuario = SessionHelper.GetUsuario().Ejercicio;
        }

        public ActionResult RptSurtimientoOC(int id)
        {
            ReportHelper reportHelper = new ReportHelper();
            Entidad entidad = reportHelper.GetDatosEntidad();

            Dictionary<string, object> parametros = new Dictionary<string, object>();
            parametros.Add("@pNombreEnte", entidad.Nombre);
            parametros.Add("@pEstado", entidad.Estado);
            parametros.Add("@pTituloReporte", "Surtimiento de OC");
            parametros.Add("@pReciboId", id);
            parametros.Add("@pUsuarioId", SessionHelper.GetUsuario().UsuarioId);

            ViewBag.WebReport = reportHelper.GetWebReport("Almacen/Compras/ARrptSurtimientoOC.frx", parametros);

            return View("RptSurtimientoOC");
        }
    }
}