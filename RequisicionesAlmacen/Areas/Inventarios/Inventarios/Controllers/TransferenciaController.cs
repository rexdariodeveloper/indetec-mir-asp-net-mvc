using Newtonsoft.Json;
using RequisicionesAlmacen.Areas.Inventarios.Inventarios.Models;
using RequisicionesAlmacen.Areas.Inventarios.Inventarios.Models.ViewModel;
using RequisicionesAlmacen.Controllers;
using RequisicionesAlmacen.Helpers;
using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Models.Mapeos;
using RequisicionesAlmacenBL.Services;
using RequisicionesAlmacenBL.Services.Almacen;
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

namespace RequisicionesAlmacen.Areas.Inventarios.Inventarios.Controllers
{
    [Authenticated(nodoMenuId = MenuPrincipalMapeo.ID.TRANSFERENCIAS)]
    public class TransferenciaController : BaseController<ARtblTransferencia, TransferenciaViewModel>
    {
        private string API_FICHA = "/inventarios/inventarios/transferencia/";

        public override ActionResult Nuevo()
        {
            //Crear un objeto nuevo
            TransferenciaViewModel viewModel = new TransferenciaViewModel();

            //Asignamos el modelo al modelView
            ARtblTransferencia modelo = new ARtblTransferencia();

            modelo.TransferenciaId = 0;
            modelo.Codigo = "TRA000000";
            modelo.Fecha = DateTime.Now;

            viewModel.Transferencia = modelo;

            //Modo Solo Lectura
            viewModel.SoloLectura = false;

            //Agregamos todos los datos necesarios para el funcionamiento de la ficha
            //como son los Listados para combos, tablas, arboles.
            GetDatosFicha(ref viewModel);

            //Retornamos la vista junto con su Objeto Modelo
            return View("Transferencia", viewModel);
        }

        public override ActionResult Editar(int id)
        {
            throw new NotImplementedException();
        }

        public ActionResult Ver(int id)
        {
            TransferenciaService service = new TransferenciaService();

            //Creamos el objeto
            TransferenciaViewModel viewModel = new TransferenciaViewModel();

            //Buscamos el Objeto por el Id que se envio como parametro
            ARtblTransferencia modelo = service.BuscaPorId(id);

            //Asignamos el Objeto al ViewModel
            viewModel.Transferencia = modelo != null ? modelo : new ARtblTransferencia();

            if (modelo == null || !modelo.EstatusId.Equals(EstatusRegistro.ACTIVO))
            {
                //Asignamos el error
                SetViewBagError("La Transferencia no existe o está Cancelada. Favor de revisar.", API_FICHA + "listar");
            }
            else
            {
                //Asignamos la fecha
                modelo.FechaTransferencia = service.GetFechaConFormato(modelo.Fecha);

                //Asignamos los detalles
                viewModel.ListTransferenciaMovtos = service.BuscaMovtosPorTransferenciaId(modelo.TransferenciaId);

                //Modo Solo Lectura
                viewModel.SoloLectura = true;

                //Agregamos todos los datos necesarios para el funcionamiento de la ficha
                //como son los Listados para combos, tablas, arboles.
                GetDatosFicha(ref viewModel);
            }

            //Retornamos la vista junto con su Objeto Modelo
            return View("Transferencia", viewModel);
        }

        public override JsonResult Guardar(ARtblTransferencia transferencia)
        {
            throw new NotImplementedException();
        }

        [JsonException]
        public JsonResult GuardaCambios(ARtblTransferencia transferencia, List<TransferenciaMovtoItem> movimientos)
        {
            int usuarioId = SessionHelper.GetUsuario().UsuarioId;
            DateTime fecha = DateTime.Now;

            TransferenciaService service = new TransferenciaService();

            //Actualizamos la cabecera
            transferencia.EstatusId = EstatusRegistro.ACTIVO;
            transferencia.CreadoPorId = usuarioId;

            //Asignamos el numero de partida
            int numeroLinea = 1;

            //Agregamos los detalles
            foreach (TransferenciaMovtoItem movimientoTemp in movimientos)
            {
                ARtblTransferenciaMovto movimiento = (ARtblTransferenciaMovto)movimientoTemp;

                movimiento.NumeroLinea = numeroLinea;
                movimiento.EstatusId = EstatusRegistro.ACTIVO;
                movimiento.CreadoPorId = usuarioId;

                transferencia.ARtblTransferenciaMovto.Add(movimiento);

                numeroLinea++;
            }

            //Retornamos un mensaje de Exito si todo salio correctamente
            return Json(service.GuardaCambios(transferencia), "Registro guardado con Exito!");
        }

        public override JsonResult Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        public override ActionResult Listar()
        {
            TransferenciaViewModel viewModel = new TransferenciaViewModel();

            viewModel.ListTransferecias = new TransferenciaService().BuscaListado();
            
            return View("ListadoTransferencia", viewModel);
        }

        protected override void GetDatosFicha(ref TransferenciaViewModel viewModel)
        {
            //Datos de la Transferencia
            viewModel.ListAlmacenes = new AlmacenService().BuscaTodos();

            if (!viewModel.SoloLectura)
            {
                viewModel.ListProductos = new TransferenciaService().BuscaProductos();
            }

            //Ejercicio para los campos de Fecha
            viewModel.EjercicioUsuario = SessionHelper.GetUsuario().Ejercicio;
        }
    }
}