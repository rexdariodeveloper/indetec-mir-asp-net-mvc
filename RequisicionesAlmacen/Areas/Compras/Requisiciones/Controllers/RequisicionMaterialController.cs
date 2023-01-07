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
using RequisicionesAlmacenBL.Services.Sistema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static RequisicionesAlmacenBL.Models.Mapeos.ControlMaestroMapeo;
using static RequisicionesAlmacenBL.Models.Mapeos.AlertaDefinicion;

namespace RequisicionesAlmacen.Areas.Compras.Requisiciones.Controllers
{
    [Authenticated(nodoMenuId = MenuPrincipalMapeo.ID.REQUISICION_MATERIAL)]
    public class RequisicionMaterialController : BaseController<ARtblRequisicionMaterial, RequisicionMaterialViewModel>
    {
        private string API_FICHA = "/compras/requisiciones/requisicionmaterial/";

        public override ActionResult Nuevo()
        {
            //Crear un objeto nuevo
            RequisicionMaterialViewModel viewModel = new RequisicionMaterialViewModel();

            //Asignamos el modelo al modelView
            viewModel.RequisicionMaterial = new ARtblRequisicionMaterial();
            viewModel.RequisicionMaterial.RequisicionMaterialId = 0;
            viewModel.RequisicionMaterial.EstatusId = AREstatusRequisicion.GUARDADA;
            viewModel.RequisicionMaterial.FechaRequisicion = DateTime.Now;

            //Buscamos el Empleado que realizará la Requisición
            RHtblEmpleado empleado = new EmpleadoService().BuscaPorUsuarioId(SessionHelper.GetUsuario().UsuarioId);

            if (empleado == null || empleado.AreaAdscripcionId == null || empleado.AreaAdscripcionId.Trim().CompareTo("") == 0)
            {
                //Asignamos el error
                SetViewBagError("El usuario actual " + (empleado == null ? "no está relacionado a ningún empleado." : "no tiene Área de Adscripción."), API_FICHA + "listar");
            }

            viewModel.Empleado = empleado;

            // Modo Solo Lectura
            viewModel.SoloLectura = false;

            //Agregamos todos los datos necesarios para el funcionamiento de la ficha
            //como son los Listados para combos, tablas, arboles.
            GetDatosFicha(ref viewModel);

            //Retornamos la vista junto con su Objeto Modelo
            return View("RequisicionMaterial", viewModel);
        }

        public override ActionResult Editar(int id)
        {
            return ViewModel(id, null);
        }

        public ActionResult Ver(int id)
        {
            return ViewModel(id, true);
        }

        public ActionResult ViewModel(int id, Nullable<bool> soloLectura)
        {
            RequisicionMaterialService service = new RequisicionMaterialService();

            //Creamos el objeto
            RequisicionMaterialViewModel viewModel = new RequisicionMaterialViewModel();

            //Buscamos el Objeto por el Id que se envio como parametro
            ARtblRequisicionMaterial requisicion = service.BuscaPorId(id);

            if (requisicion == null || requisicion.EstatusId == AREstatusRequisicion.CANCELADA)
            {
                //Asignamos el error
                SetViewBagError("La Requisición no existe o está Cancelada. Favor de revisar.", API_FICHA  + "listar");

                //Asignamos el Objeto al ViewModel
                viewModel.RequisicionMaterial = new ARtblRequisicionMaterial();
            }
            else
            {
                //Buscamos el Empleado que realizará la Requisición
                RHtblEmpleado empleado = new EmpleadoService().BuscaPorUsuarioId(SessionHelper.GetUsuario().UsuarioId);

                if (empleado == null || empleado.AreaAdscripcionId == null || empleado.AreaAdscripcionId.Trim().CompareTo("") == 0)
                {
                    //Asignamos el error
                    SetViewBagError("El usuario actual " + (empleado == null ? "no está relacionado a ningún empleado." : "no tiene Área de Adscripción."), API_FICHA + "listar");
                }

                viewModel.Empleado = empleado;

                //Asignamos el Objeto al ViewModel
                viewModel.RequisicionMaterial = requisicion;

                //Asignamos los detalles
                List<ARspConsultaRequisicionMaterialDetalles_Result> listDetalles = service.BuscaDetallesPorRequisicionMaterialId(requisicion.RequisicionMaterialId);
                viewModel.ListRequisicionMaterialDetalles = listDetalles.ConvertAll(new Converter<ARspConsultaRequisicionMaterialDetalles_Result, RequisicionMaterialDetalleItem>(RequisicionMaterialDetalleItem.ConvertTo));

                if (soloLectura == null)
                {
                    int estatusId = requisicion.EstatusId;

                    soloLectura = estatusId != AREstatusRequisicion.GUARDADA
                        && estatusId != AREstatusRequisicion.REVISION
                        && estatusId != AREstatusRequisicion.EN_PROCESO
                        && estatusId != AREstatusRequisicion.POR_COMPRAR;
                }
            }

            //Agregamos todos los datos necesarios para el funcionamiento de la ficha
            //como son los Listados para combos, tablas, arboles.
            GetDatosFicha(ref viewModel);

            // Modo Solo Lectura
            viewModel.SoloLectura = soloLectura.GetValueOrDefault();

            //Retornamos la vista junto con su Objeto Modelo
            return View("RequisicionMaterial", viewModel);
        }

        [JsonException]
        public override JsonResult Guardar(ARtblRequisicionMaterial requisicionMaterial)
        {
            throw new NotImplementedException();
        }

        [JsonException]
        public JsonResult GuardaCambios(ARtblRequisicionMaterial requisicionMaterial, List<RequisicionMaterialDetalleItem> detalles)
        {
            RequisicionMaterialService service = new RequisicionMaterialService();

            int usuarioId = SessionHelper.GetUsuario().UsuarioId;
            DateTime fecha = DateTime.Now;

            //Si es un nuevo registro llenamos el campo de creadoPor
            if (requisicionMaterial.RequisicionMaterialId == 0)
            {
                requisicionMaterial.EstatusId = AREstatusRequisicion.GUARDADA;
                requisicionMaterial.CreadoPorId = usuarioId;
            }

            //De lo contrario llenamos el campo de ModificadoPor y Fecha de Ultima Modificacion
            else
            {
                ARtblRequisicionMaterial temp = service.BuscaPorId(requisicionMaterial.RequisicionMaterialId);

                if (!StructuralComparisons.StructuralEqualityComparer.Equals(requisicionMaterial.Timestamp, temp.Timestamp))
                {
                    throw new Exception("La Requisición con el código [" + requisicionMaterial.CodigoRequisicion + "] ha sido modificada por otro usuario. Favor de recargar la vista antes de guardar.");
                }

                requisicionMaterial.ModificadoPorId = usuarioId;
                requisicionMaterial.FechaUltimaModificacion = fecha;
            }

            List<ARtblRequisicionMaterialDetalle> requisicionMaterialDetalles = new List<ARtblRequisicionMaterialDetalle>();

            foreach (RequisicionMaterialDetalleItem registro in detalles)
            {
                if (registro.EstatusId == AREstatusRequisicionDetalle.ACTIVO
                    || registro.EstatusId == AREstatusRequisicionDetalle.MODIFICADO
                    || registro.EstatusId == AREstatusRequisicionDetalle.CANCELADO)
                {
                    ARtblRequisicionMaterialDetalle detalle = (ARtblRequisicionMaterialDetalle)registro;

                    // Si es un registro nuevo
                    if (registro.RequisicionMaterialDetalleId < 0)
                    {
                        detalle.CreadoPorId = usuarioId;
                    }
                    // Si un registro que se editó o eliminó
                    else
                    {
                        detalle.ModificadoPorId = usuarioId;
                        detalle.FechaUltimaModificacion = fecha;
                    }

                    requisicionMaterialDetalles.Add(detalle);
                }
            }

            //Retornamos un mensaje de Exito si todo salio correctamente
            return Json(service.GuardaCambios(requisicionMaterial, requisicionMaterialDetalles), "Registro guardado con Exito!");
        }

        [JsonException]
        public JsonResult EnviarAutorizar(ARtblRequisicionMaterial requisicionMaterial, List<RequisicionMaterialDetalleItem> detalles)
        {
            RequisicionMaterialService service = new RequisicionMaterialService();

            int usuarioId = SessionHelper.GetUsuario().UsuarioId;
            DateTime fecha = DateTime.Now;

            ARtblRequisicionMaterial temp = service.BuscaPorId(requisicionMaterial.RequisicionMaterialId);

            if (!StructuralComparisons.StructuralEqualityComparer.Equals(requisicionMaterial.Timestamp, temp.Timestamp))
            {
                throw new Exception("La Requisición con el código [" + requisicionMaterial.CodigoRequisicion + "] ha sido modificada por otro usuario. Favor de recargar la vista antes de guardar.");
            }

            requisicionMaterial.EstatusId = AREstatusRequisicion.ENVIADA;
            requisicionMaterial.ModificadoPorId = usuarioId;
            requisicionMaterial.FechaUltimaModificacion = fecha;

            List<ARtblRequisicionMaterialDetalle> requisicionMaterialDetalles = new List<ARtblRequisicionMaterialDetalle>();

            foreach (RequisicionMaterialDetalleItem registro in detalles)
            {
                if (registro.EstatusId != AREstatusRequisicionDetalle.RECHAZADO)
                {
                    ARtblRequisicionMaterialDetalle detalle = (ARtblRequisicionMaterialDetalle)registro;

                    detalle.EstatusId = AREstatusRequisicionDetalle.ENVIADO;
                    detalle.ModificadoPorId = usuarioId;
                    detalle.FechaUltimaModificacion = fecha;

                    requisicionMaterialDetalles.Add(detalle);
                }
            }

            service.GuardaCambios(requisicionMaterial, requisicionMaterialDetalles);

            int alertaId = new AlertaService().IniciarAlerta(AlertaDefinicion.REQUISICION_MATERIA_AUTORIZACION,
                                                             requisicionMaterial.RequisicionMaterialId,
                                                             requisicionMaterial.CodigoRequisicion,
                                                             "Requisición: " + requisicionMaterial.CodigoRequisicion,
                                                             usuarioId);

            //Retornamos un mensaje de Exito si todo salio correctamente
            return Json(requisicionMaterial.RequisicionMaterialId, "Registro guardado con Exito!");
        }

        public override JsonResult Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        [JsonException]
        public JsonResult EliminarPorId(int requisicionMaterialId, byte[] timestamp)
        {
            ARtblRequisicionMaterial requisicionMaterial = new ARtblRequisicionMaterial();

            requisicionMaterial.RequisicionMaterialId = requisicionMaterialId;
            requisicionMaterial.Timestamp = timestamp;

            return EliminarPorModelo(requisicionMaterial);
        }

        [JsonException]
        public JsonResult EliminarPorModelo(ARtblRequisicionMaterial requisicionMaterial)
        {
            RequisicionMaterialService service = new RequisicionMaterialService();

            int usuarioId = SessionHelper.GetUsuario().UsuarioId;
            DateTime fecha = DateTime.Now;

            ARtblRequisicionMaterial temp = service.BuscaPorId(requisicionMaterial.RequisicionMaterialId);

            if (!StructuralComparisons.StructuralEqualityComparer.Equals(requisicionMaterial.Timestamp, temp.Timestamp))
            {
                throw new Exception("La Requisición con el código [" + temp.CodigoRequisicion + "] ha sido modificada por otro usuario. Favor de recargar la vista antes de guardar.");
            }

            temp.EstatusId = AREstatusRequisicion.CANCELADA;
            temp.ModificadoPorId = usuarioId;
            temp.FechaUltimaModificacion = fecha;

            List<ARspConsultaRequisicionMaterialDetalles_Result> listDetalles = service.BuscaDetallesPorRequisicionMaterialId(requisicionMaterial.RequisicionMaterialId);
            List<RequisicionMaterialDetalleItem> listDetalleItems = listDetalles.ConvertAll(new Converter<ARspConsultaRequisicionMaterialDetalles_Result, RequisicionMaterialDetalleItem>(RequisicionMaterialDetalleItem.ConvertTo));
            List<ARtblRequisicionMaterialDetalle> detalles = new List<ARtblRequisicionMaterialDetalle>();

            foreach (RequisicionMaterialDetalleItem detalleItem in listDetalleItems)
            {
                ARtblRequisicionMaterialDetalle detalle = (ARtblRequisicionMaterialDetalle)detalleItem;

                detalle.EstatusId = AREstatusRequisicionDetalle.CANCELADO;
                detalle.ModificadoPorId = usuarioId;
                detalle.FechaUltimaModificacion = fecha;

                detalles.Add(new ARtblRequisicionMaterialDetalle().GetModelo(detalle));
            }

            service.GuardaCambios(new ARtblRequisicionMaterial().GetModelo(temp), detalles);

            //Retornamos un mensaje de Exito si todo salio correctamente
            return Json("Registro eliminado con Exito!");
        }

        public override ActionResult Listar()
        {
            RequisicionMaterialViewModel viewModel = new RequisicionMaterialViewModel();

            viewModel.ListRequisicionMaterial = new RequisicionMaterialService().BuscaListado();

            return View("ListadoRequisicionMaterial", viewModel);
        }

        protected override void GetDatosFicha(ref RequisicionMaterialViewModel viewModel)
        {
            viewModel.ListAreas = viewModel.Empleado != null ? new DependenciaService().BuscaAreasPorEmpleadoId(viewModel.Empleado.EmpleadoId) : new DependenciaService().BuscaTodos();
            viewModel.ListUnidadesAdministrativas = new DependenciaService().BuscaTodos();
            viewModel.ListProyectos = new ProyectoService().BuscaTodos();
            viewModel.ListUnidadesMedida = new UnidadDeMedidaService().BuscaTodos();

            //Ejercicio para los campos de Fecha
            viewModel.EjercicioUsuario = SessionHelper.GetUsuario().Ejercicio;
        }

        [JsonException]
        public ContentResult GetProductos(string areaId, string unidadAdministrativaId, string proyectoId)
        {
            unidadAdministrativaId = unidadAdministrativaId == "" ? null : unidadAdministrativaId;
            proyectoId = proyectoId == "" ? null : proyectoId;

            var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };

            var result = new ContentResult
            {
                Content = serializer.Serialize(new RequisicionMaterialService().BuscaComboProductos(areaId, unidadAdministrativaId, proyectoId)),
                ContentType = "application/json"
            };

            return result;
        }

        public ActionResult RptSolicitudMateriales(int id)
        {
            ReportHelper reportHelper = new ReportHelper();

            Dictionary<string, object> parametros = new Dictionary<string, object>();
            parametros.Add("@pNombreEnte", "El ente de aqui de Jalisco");
            parametros.Add("@pEstado", "Jalisco");
            parametros.Add("@pTituloReporte", "Solicitud de Materiales");
            parametros.Add("@pRequisicionId", id);
            parametros.Add("@pUsuarioId", SessionHelper.GetUsuario().UsuarioId);

            ViewBag.WebReport = reportHelper.GetWebReport("Almacen/RequisicionMaterial/ARrptSolicitudMateriales.frx", parametros);

            return View("RptSolicitudMateriales");
        }
    }
}