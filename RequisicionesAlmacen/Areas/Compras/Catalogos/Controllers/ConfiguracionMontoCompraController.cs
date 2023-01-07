using RequisicionesAlmacen.Areas.Compras.Catalogos.Models.ViewModel;
using RequisicionesAlmacen.Controllers;
using RequisicionesAlmacen.Helpers;
using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Models.Mapeos;
using RequisicionesAlmacenBL.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace RequisicionesAlmacen.Areas.Compras.Catalogos.Controllers
{
    [Authenticated(nodoMenuId = MenuPrincipalMapeo.ID.CONFIGURACION_MONTOS_COMPRAS)]
    public class ConfiguracionMontoCompraController : BaseController<List<ARtblControlMaestroConfiguracionMontoCompra>, ControlMaestroConfiguracionMontoCompraViewModel>
    {
        public override ActionResult Editar(int id)
        {
            throw new NotImplementedException();
        }

        public override JsonResult Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        [JsonException]
        public override JsonResult Guardar(List<ARtblControlMaestroConfiguracionMontoCompra> listControlMaestroConfiguracionMontoCompra)
        {
            int usuarioId = SessionHelper.GetUsuario().UsuarioId;
            DateTime fecha = DateTime.Now;

            ControlMaestroConfiguracionMontoCompraService service = new ControlMaestroConfiguracionMontoCompraService();
            
            foreach (ARtblControlMaestroConfiguracionMontoCompra configuracionMontoCompra in listControlMaestroConfiguracionMontoCompra)
            {
                if (configuracionMontoCompra.ConfiguracionMontoId > 0)
                {
                    configuracionMontoCompra.ModificadoPorId = usuarioId;
                    configuracionMontoCompra.FechaUltimaModificacion = fecha;
                }
                else
                {
                    configuracionMontoCompra.CreadoPorId = usuarioId;
                }
            }

            new ControlMaestroConfiguracionMontoCompraService().GuardaCambios(listControlMaestroConfiguracionMontoCompra);

            return Json("Registros con Exito!");
        }

        // GET: Compras/ConfiguracionMontoCompra/Listar
        public override ActionResult Listar()
        {
            ControlMaestroConfiguracionMontoCompraViewModel viewModel = new ControlMaestroConfiguracionMontoCompraViewModel();
            
            viewModel.ControlMaestroConfiguracionMontoCompra = new ARtblControlMaestroConfiguracionMontoCompra();
            viewModel.ControlMaestroConfiguracionMontoCompra.EstatusId = ControlMaestroMapeo.EstatusRegistro.ACTIVO;
            
            GetDatosFicha(ref viewModel);
            
            return View("ConfiguracionMontoCompra", viewModel);
        }

        public override ActionResult Nuevo()
        {
            throw new NotImplementedException();
        }

        protected override void GetDatosFicha(ref ControlMaestroConfiguracionMontoCompraViewModel viewModel)
        {
            viewModel.ListConfiguracionMontoCompra = new ControlMaestroConfiguracionMontoCompraService().BuscaTodos();

            viewModel.ListTipoCompra = new ControlMaestroService().BuscaControl("TipoCompra");
        }
    }
}