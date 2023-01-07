using Newtonsoft.Json;
using RequisicionesAlmacen.Areas.Compras.Compras.Models;
using RequisicionesAlmacen.Areas.Compras.Compras.Models.ViewModel;
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
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static RequisicionesAlmacenBL.Models.Mapeos.ControlMaestroMapeo;

namespace RequisicionesAlmacen.Areas.Compras.Compras.Controllers
{
    [Authenticated(nodoMenuId = MenuPrincipalMapeo.ID.INVITACION_DE_COMPRA)]
    public class InvitacionCompraController : BaseController<ARtblInvitacionCompra, InvitacionCompraViewModel>
    {
        private string API_FICHA = "/compras/compras/invitacioncompra/";

        public override ActionResult Nuevo()
        {
            throw new NotImplementedException();
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
            InvitacionCompraService service = new InvitacionCompraService();

            //Creamos el objeto
            InvitacionCompraViewModel viewModel = new InvitacionCompraViewModel();

            //Buscamos el Objeto por el Id que se envio como parametro
            ARtblInvitacionCompra invitacionCompra = service.BuscaPorId(id);

            //Asignamos el Objeto al ViewModel
            viewModel.InvitacionCompra = invitacionCompra != null ? invitacionCompra : new ARtblInvitacionCompra();

            if (invitacionCompra == null || invitacionCompra.EstatusId.Equals(AREstatusInvitacionCompra.CANCELADA))
            {
                //Asignamos el error
                SetViewBagError("La Invitación de Compra no existe o está Cancelada. Favor de revisar.", API_FICHA + "listar");
            }
            else
            {
                //Asignamos el string de la fecha
                invitacionCompra.FechaInvitacion = service.GetFechaConFormato(invitacionCompra.Fecha);

                //Asignamos los detalles
                viewModel.ListInvitacionCompraDetalles = service.BuscaDetallesPorInvitacionCompraId(invitacionCompra.InvitacionCompraId);

                //Agregamos todos los datos necesarios para el funcionamiento de la ficha
                //como son los Listados para combos, tablas, arboles.
                GetDatosFicha(ref viewModel);

                //Asignamos el string del numero de proveedores
                int contadorProveedores = 0;

                foreach (var proveedor in viewModel.ListProveedores)
                {
                    if (proveedor.Seleccionado.GetValueOrDefault())
                    {
                        contadorProveedores++;
                    }
                }

                invitacionCompra.Proveedores = contadorProveedores;

                //Validamos si alún registro permite editar
                bool permiteEditar = false;

                if (soloLectura == null
                    && !invitacionCompra.EstatusId.Equals(AREstatusInvitacionCompra.CANCELADA)
                    && !invitacionCompra.EstatusId.Equals(AREstatusInvitacionCompra.FINALIZADA))
                {
                    foreach (ARspConsultaInvitacionCompraDetalles_Result detalle in viewModel.ListInvitacionCompraDetalles)
                    {
                        if (detalle.PermiteEditar.GetValueOrDefault())
                        {
                            permiteEditar = true;
                        }
                    }
                }

                //Modo Solo Lectura
                viewModel.SoloLectura = soloLectura == null ? !permiteEditar : soloLectura.GetValueOrDefault();
            }

            //Retornamos la vista junto con su Objeto Modelo
            return View("InvitacionCompra", viewModel);
        }

        [JsonException]
        public override JsonResult Guardar(ARtblInvitacionCompra invitacionCompra)
        {
            throw new NotImplementedException();
        }

        [JsonException]
        public JsonResult GuardaCambios(ARtblInvitacionCompra invitacionCompra,
                                        List<InvitacionCompraDetalleItem> detallesEliminados,
                                        List<InvitacionCompraProveedorItem> proveedoresInvitados,
                                        List<ArtblInvitacionCompraDetallePrecioProveedor> preciosProveedor,
                                        List<InvitacionCompraProveedorCotizacionItem> cotizacionesEliminadas)
        {
            int usuarioId = SessionHelper.GetUsuario().UsuarioId;
            DateTime fecha = DateTime.Now;

            InvitacionCompraService service = new InvitacionCompraService();

            //Verificamos que no se haya modidicado
            ARtblInvitacionCompra temp = service.BuscaPorId(invitacionCompra.InvitacionCompraId);

            if (!StructuralComparisons.StructuralEqualityComparer.Equals(invitacionCompra.Timestamp, temp.Timestamp))
            {
                throw new Exception("La Invitación con el código [" + temp.CodigoInvitacion + "] ha sido modificada por otro usuario. Favor de recargar la vista antes de guardar.");
            }

            //Actualizamos la cabecera
            invitacionCompra.ModificadoPorId = usuarioId;
            invitacionCompra.FechaUltimaModificacion = fecha;

            List<ARtblInvitacionCompraDetalle> detallesCancelados = new List<ARtblInvitacionCompraDetalle>();

            if (detallesEliminados != null)
            {
                foreach (InvitacionCompraDetalleItem registro in detallesEliminados)
                {
                    ARtblInvitacionCompraDetalle detalle = (ARtblInvitacionCompraDetalle)registro;

                    //Actualizamos los detalles cancelados
                    detalle.EstatusId = AREstatusInvitacionCompraDetalle.CANCELADO;
                    detalle.ModificadoPorId = usuarioId;
                    detalle.FechaUltimaModificacion = fecha;

                    detallesCancelados.Add(detalle);
                }
            }

            List<ARtblInvitacionCompraProveedor> proveedoresInvitadosTemp = new List<ARtblInvitacionCompraProveedor>();

            if (proveedoresInvitados != null)
            {
                foreach (InvitacionCompraProveedorItem registro in proveedoresInvitados)
                {
                    ARtblInvitacionCompraProveedor proveedor = (ARtblInvitacionCompraProveedor)registro;

                    //Si es un registro nuevo
                    if (proveedor.InvitacionCompraProveedorId < 0)
                    {
                        proveedor.EstatusId = EstatusRegistro.ACTIVO;
                        proveedor.CreadoPorId = usuarioId;
                    }

                    //Actualizamos los detalles cancelados
                    else
                    {
                        proveedor.ModificadoPorId = usuarioId;
                        proveedor.FechaUltimaModificacion = fecha;
                    }

                    if (proveedor.Cotizaciones != null)
                    {
                        foreach (ArtblInvitacionCompraProveedorCotizacion cotizacion in proveedor.Cotizaciones)
                        {
                            cotizacion.EstatusId = EstatusRegistro.ACTIVO;
                            cotizacion.CreadoPorId = usuarioId;
                        }
                    }

                    proveedoresInvitadosTemp.Add(proveedor);
                }
            }

            if (preciosProveedor != null)
            {
                foreach (ArtblInvitacionCompraDetallePrecioProveedor precioProveedor in preciosProveedor)
                {
                    //Verificamos si es registro nuevo
                    if (precioProveedor.InvitacionCompraDetallePrecioProveedorId < 0)
                    {
                        precioProveedor.CreadoPorId = usuarioId;
                    }

                    //De lo contrario actualizamos el registro
                    else
                    {
                        precioProveedor.ModificadoPorId = usuarioId;
                        precioProveedor.FechaUltimaModificacion = fecha;
                    }
                }
            }

            List<ArtblInvitacionCompraProveedorCotizacion> cotizaciones = new List<ArtblInvitacionCompraProveedorCotizacion>();

            if (cotizacionesEliminadas != null)
            {
                foreach (InvitacionCompraProveedorCotizacionItem cotizacion in cotizacionesEliminadas)
                {
                    ArtblInvitacionCompraProveedorCotizacion cotizacionTemp = (ArtblInvitacionCompraProveedorCotizacion)cotizacion;

                    cotizacionTemp.EstatusId = EstatusRegistro.BORRADO;
                    cotizacionTemp.ModificadoPorId = usuarioId;
                    cotizacionTemp.FechaUltimaModificacion = fecha;

                    cotizaciones.Add(cotizacionTemp);
                }
            }

            //Retornamos un mensaje de Exito si todo salio correctamente
            return Json(service.GuardaCambios(invitacionCompra,
                                              detallesCancelados,
                                              null,
                                              proveedoresInvitadosTemp,
                                              preciosProveedor,
                                              cotizaciones), "Registro guardado con Exito!");
        }

        [JsonException]
        public JsonResult ConvertirOC(ARtblInvitacionCompra invitacionCompra, InvitacionOrdenCompraItem invitacionOC)
        {
            int usuarioId = SessionHelper.GetUsuario().UsuarioId;
            DateTime fecha = DateTime.Now;

            InvitacionCompraService service = new InvitacionCompraService();

            //Verificamos que no se haya modidicado
            ARtblInvitacionCompra temp = service.BuscaPorId(invitacionCompra.InvitacionCompraId);

            if (!StructuralComparisons.StructuralEqualityComparer.Equals(invitacionCompra.Timestamp, temp.Timestamp))
            {
                throw new Exception("La Invitación con el código [" + temp.CodigoInvitacion + "] ha sido modificada por otro usuario. Favor de recargar la vista antes de guardar.");
            }

            //Retornamos un mensaje de Exito si todo salio correctamente
            return Json(service.ConvertirOC((tblOrdenCompra)invitacionOC, usuarioId, fecha), "Registro guardado con Exito!");
        }

        public override JsonResult Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        [JsonException]
        public JsonResult EliminarPorId(int invitacionCompraId, byte[] timestamp)
        {
            ARtblInvitacionCompra invitacionCompra = new ARtblInvitacionCompra();

            invitacionCompra.InvitacionCompraId = invitacionCompraId;
            invitacionCompra.Timestamp = timestamp;

            return EliminarPorModelo(invitacionCompra);
        }

        [JsonException]
        public JsonResult EliminarPorModelo(ARtblInvitacionCompra invitacionCompra)
        {
            InvitacionCompraService service = new InvitacionCompraService();

            int usuarioId = SessionHelper.GetUsuario().UsuarioId;
            DateTime fecha = DateTime.Now;

            ARtblInvitacionCompra temp = new ARtblInvitacionCompra().GetModelo(service.BuscaPorId(invitacionCompra.InvitacionCompraId));

            if (!StructuralComparisons.StructuralEqualityComparer.Equals(invitacionCompra.Timestamp, temp.Timestamp))
            {
                throw new Exception("La Invitación con el código [" + temp.CodigoInvitacion + "] ha sido modificada por otro usuario. Favor de recargar la vista antes de guardar.");
            }

            temp.EstatusId = AREstatusInvitacionCompra.CANCELADA;
            temp.ModificadoPorId = usuarioId;
            temp.FechaUltimaModificacion = fecha;

            List<ARtblInvitacionCompraDetalle> detallesTemp = service.BuscaDetallesNoCancelados(temp.InvitacionCompraId);
            List<ARtblInvitacionCompraDetalle> detalles = new List<ARtblInvitacionCompraDetalle>();

            foreach (ARtblInvitacionCompraDetalle detalle in detallesTemp)
            {
                detalle.EstatusId = AREstatusInvitacionCompraDetalle.CANCELADO;
                detalle.ModificadoPorId = usuarioId;
                detalle.FechaUltimaModificacion = fecha;

                detalles.Add(new ARtblInvitacionCompraDetalle().GetModelo(detalle));
            }

            service.GuardaCambios(temp, detalles);

            //Retornamos un mensaje de Exito si todo salio correctamente
            return Json("Registro eliminado con Exito!");
        }

        public override ActionResult Listar()
        {
            InvitacionCompraViewModel viewModel = new InvitacionCompraViewModel();

            viewModel.ListInvitacionCompra = new InvitacionCompraService().BuscaListado();
            
            return View("ListadoInvitacionCompra", viewModel);
        }

        protected override void GetDatosFicha(ref InvitacionCompraViewModel viewModel)
        {
            InvitacionCompraService invitacionCompraService = new InvitacionCompraService();
            int invitacionCompraId = viewModel.InvitacionCompra.InvitacionCompraId;

            //Datos de Invitación de OC
            viewModel.InvitacionCompra.Estatus = AREstatusInvitacionCompra.Nombre[viewModel.InvitacionCompra.EstatusId];
            viewModel.ListProveedores = invitacionCompraService.BuscaInvitacionCompraListadoProveedores(invitacionCompraId);
            viewModel.ListAlmacenes = new AlmacenService().BuscaTodos();
            viewModel.ListPreciosProveedores = new InvitacionCompraDetallePrecioProveedorService().BuscaInvitacionCompraListadoPreciosProveedores(invitacionCompraId);
            viewModel.ListProveedoresCotizaciones = new InvitacionCompraProveedorCotizacionService().BuscaInvitacionCompraListadoCotizaciones(invitacionCompraId);
            viewModel.ListOrdenesCompra = invitacionCompraService.BuscaInvitacionCompraOrdenesCompra(invitacionCompraId);
            viewModel.ListTarifasImpuesto = new TarifaImpuestoService().BuscaTodos();
        }

        [JsonException]
        [HttpPost]
        public JsonResult GuardaArchivoTemporal(HttpPostedFileBase file)
        {
            string fileName = null;
            Stream fileInputStream = null;

            if (file != null)
            {
                // Checking for Internet Explorer  
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fileName = testfiles[testfiles.Length - 1];
                }
                else
                {
                    fileName = file.FileName;
                }

                fileInputStream = file.InputStream;
            }

            string nombreArchivoTemp = new ArchivoService().GuardaArchivoTemporal(fileName, fileInputStream);

            //Retornamos un mensaje de Exito si todo salio correctamente
            return Json(nombreArchivoTemp);
        }

        [JsonException]
        public ActionResult BuscarArchivo(Nullable<Guid> archivoId, string nombreArchivoTmp, string nombreOriginal)
        {
            Dictionary<string, byte[]> diccionarioArchivo = null;

            if (nombreArchivoTmp != null && !nombreArchivoTmp.Equals(""))
            {
                diccionarioArchivo = new ArchivoService().Download(nombreArchivoTmp, nombreOriginal);
            }
            else if (archivoId != null)
            {
                diccionarioArchivo = new ArchivoService().Download(archivoId);                
            }

            if (diccionarioArchivo != null)
            {
                foreach (KeyValuePair<string, byte[]> archivo in diccionarioArchivo)
                {
                    Session["InvitacionCompra_NombreArchivo"] = archivo.Key;
                    Session["InvitacionCompra_DescargarArchivo"] = archivo.Value;
                }
            }
            else 
            {
                throw new Exception("No se pudo descargar el archivo.");
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [JsonException]
        public ActionResult DescargarArchivo()
        {
            Object archivo = Session["InvitacionCompra_DescargarArchivo"];

            if (archivo != null)
            {
                return File(archivo as byte[], "application/octet-stream", Session["InvitacionCompra_NombreArchivo"].ToString());
            }
            else
            {
                throw new Exception("No se pudo descargar el archivo.");
            }
        }
    }
}