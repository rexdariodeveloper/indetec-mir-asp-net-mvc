using RequisicionesAlmacen.Areas.Inventarios.Catalogos.Models.ViewModel;
using RequisicionesAlmacen.Controllers;
using RequisicionesAlmacen.Helpers;
using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Models.Mapeos;
using RequisicionesAlmacenBL.Services;
using RequisicionesAlmacenBL.Services.SAACG;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RequisicionesAlmacen.Areas.Inventarios.Catalogos.Controllers
{
    [Authenticated(nodoMenuId = MenuPrincipalMapeo.ID.CONCEPTOS_AJUSTE_INVENTARIO)]
    public class ConceptoAjusteInventarioController : BaseController<List<ARtblControlMaestroConceptoAjusteInventario>, ControlMaestroConceptoAjusteInventarioViewModel>
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
        public override JsonResult Guardar(List<ARtblControlMaestroConceptoAjusteInventario> listControlMaestroConceptoAjusteInventario)
        {
            int usuarioId = SessionHelper.GetUsuario().UsuarioId;
            DateTime fecha = DateTime.Now;

            foreach (ARtblControlMaestroConceptoAjusteInventario concepto in listControlMaestroConceptoAjusteInventario)
            {
                if (concepto.ConceptoAjusteInventarioId > 0)
                {
                    concepto.ModificadoPorId = usuarioId;
                    concepto.FechaUltimaModificacion = fecha;
                }
                else
                {
                    concepto.CreadoPorId = usuarioId;
                }
            }

            new ControlMaestroConceptoAjusteInventarioService().GuardaCambios(listControlMaestroConceptoAjusteInventario);

            return Json("Registros con Exito!");
        }

        [HttpGet]
        public override ActionResult Listar()
        {
            ControlMaestroConceptoAjusteInventarioViewModel conceptoViewModel = new ControlMaestroConceptoAjusteInventarioViewModel();
            
            conceptoViewModel.ControlMaestroConceptoAjusteInventario = new ARtblControlMaestroConceptoAjusteInventario();

            conceptoViewModel.ControlMaestroConceptoAjusteInventario.EstatusId = ControlMaestroMapeo.EstatusRegistro.ACTIVO;

            GetDatosFicha(ref conceptoViewModel);

            return View("ConceptoAjusteInventario", conceptoViewModel);
        }

        public override ActionResult Nuevo()
        {
            throw new NotImplementedException();
        }

        protected override void GetDatosFicha(ref ControlMaestroConceptoAjusteInventarioViewModel conceptoViewModel)
        {
            conceptoViewModel.ListControlMaestroConceptoAjusteInventario = new ControlMaestroConceptoAjusteInventarioService().BuscaTodos();

            conceptoViewModel.ListTipoMovimiento = new ControlMaestroService().BuscaControl("TipoMovimiento");
        }
    }
}