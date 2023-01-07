using RequisicionesAlmacen.Areas.MIR.MIR.Models.ViewModel;
using RequisicionesAlmacen.Controllers;
using RequisicionesAlmacen.Helpers;
using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Models.Mapeos;
using RequisicionesAlmacenBL.Services;
using RequisicionesAlmacenBL.Services.MIR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RequisicionesAlmacen.Areas.MIR.MIR.Controllers
{
    [Authenticated(nodoMenuId = MenuPrincipalMapeo.ID.SEGUIMIENTO_A_VARIABLES)]
    public class MatrizConfiguracionPresupuestalSeguimientoVariableController : BaseController<MatrizConfiguracionPresupuestalSeguimientoVariableViewModel, MatrizConfiguracionPresupuestalSeguimientoVariableViewModel>
    {
        public override ActionResult Editar(int id)
        {
            // Services
            MatrizIndicadorResultadoService matrizIndicadorResultadoService = new MatrizIndicadorResultadoService();
            MatrizIndicadorResultadoIndicadorService matrizIndicadorResultadoIndicadorService = new MatrizIndicadorResultadoIndicadorService();
            MatrizIndicadorResultadoIndicadorFormulaVariableService matrizIndicadorResultadoIndicadorFormulaVariableService = new MatrizIndicadorResultadoIndicadorFormulaVariableService();
            // Crear los objetos nuevos
            MatrizConfiguracionPresupuestalSeguimientoVariableViewModel matrizConfiguracionPresupuestalSeguimientoVariableViewModel = new MatrizConfiguracionPresupuestalSeguimientoVariableViewModel();
            matrizConfiguracionPresupuestalSeguimientoVariableViewModel.MatrizConfiguracionPresupuestalSeguimientoVariableModel = new MItblMatrizConfiguracionPresupuestalSeguimientoVariable();
            // Buscamos los objetos por el ID de MIR que se envio como parametro
            matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ConsultaMatrizIndicadorResultado = matrizIndicadorResultadoService.ConsultaMatrizIndicadorResultado(id);
            if (matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ConsultaMatrizIndicadorResultado == null)
            {
                return new HttpNotFoundResult("La MIR no existe la solicitud.");
            }
            matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListMatrizIndicadorResultadoIndicador = matrizIndicadorResultadoIndicadorService.BuscaPorMIRId(id);
            List<MItblMatrizIndicadorResultadoIndicadorFormulaVariable> listMatrizIndicadorResultadoIndicadorFormulaVariable = new List<MItblMatrizIndicadorResultadoIndicadorFormulaVariable>();
            List<MItblMatrizConfiguracionPresupuestalSeguimientoVariable> listMatrizConfiguracionPresupuestalSeguimientoVariable = new List<MItblMatrizConfiguracionPresupuestalSeguimientoVariable>();
            foreach (MItblMatrizIndicadorResultadoIndicador matrizIndicadorResultadoIndicador in matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListMatrizIndicadorResultadoIndicador)
            {
                IEnumerable<MItblMatrizIndicadorResultadoIndicadorFormulaVariable> _listMatrizIndicadorResultadoIndicadorFormulaVariable = matrizIndicadorResultadoIndicadorFormulaVariableService.BuscaPorMIRIndicadorId(matrizIndicadorResultadoIndicador.MIRIndicadorId);
                _listMatrizIndicadorResultadoIndicadorFormulaVariable.ToList().ForEach(fv =>
                {
                    listMatrizIndicadorResultadoIndicadorFormulaVariable.Add(fv);
                    // Buscamos los guardados de Matriz Configuracion Presupuestal Seguimiento Variable
                    MItblMatrizConfiguracionPresupuestalSeguimientoVariable matrizConfiguracionPresupuestalSeguimientoVariable = new MatrizConfiguracionPresupuestalSeguimientoVariableService().BuscaPorMIRIndicadorFormulaVariableId(fv.MIRIndicadorFormulaVariableId);
                    if(matrizConfiguracionPresupuestalSeguimientoVariable != null)
                        listMatrizConfiguracionPresupuestalSeguimientoVariable.Add(matrizConfiguracionPresupuestalSeguimientoVariable);
                });
            }
            matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListMatrizIndicadorResultadoIndicadorFormulaVariable = listMatrizIndicadorResultadoIndicadorFormulaVariable;
            matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListMatrizConfiguracionPresupuestalSeguimientoVariable = listMatrizConfiguracionPresupuestalSeguimientoVariable;
            //como son los Listados para combos, tablas, arboles.
            GetDatosFicha(ref matrizConfiguracionPresupuestalSeguimientoVariableViewModel);
            // Crear un menu para los indicadores
            GetMenuSeguimientoVariable(ref matrizConfiguracionPresupuestalSeguimientoVariableViewModel);
            //Retornamos la vista junto con su Objeto Modelo
            return View("MatrizConfiguracionPresupuestalSeguimientoVariable", matrizConfiguracionPresupuestalSeguimientoVariableViewModel);
        }

        public override JsonResult Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        [JsonException]
        public override JsonResult Guardar(MatrizConfiguracionPresupuestalSeguimientoVariableViewModel matrizConfiguracionPresupuestalSeguimientoVariableViewModel)
        {
            // Usuario
            int usuarioId = SessionHelper.GetUsuario().UsuarioId;

            // Matriz Configuracion Presupuestal Seguimiento Variable
            if (matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListMatrizConfiguracionPresupuestalSeguimientoVariable != null)
            {
                // Service
                MatrizConfiguracionPresupuestalSeguimientoVariableService matrizConfiguracionPresupuestalSeguimientoVariableService = new MatrizConfiguracionPresupuestalSeguimientoVariableService();
                foreach (MItblMatrizConfiguracionPresupuestalSeguimientoVariable matrizConfiguracionPresupuestalSeguimientoVariable in matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListMatrizConfiguracionPresupuestalSeguimientoVariable)
                {
                    // Si el ID es nuevo para registrar o actualizar
                    if (matrizConfiguracionPresupuestalSeguimientoVariable.MIRSeguimientoVariableId > 0)
                    {
                        // Verificar si el usuario ha sido modificado en mismo MCPD y regresa mensaje de error
                        MItblMatrizConfiguracionPresupuestalSeguimientoVariable _matrizConfiguracionPresupuestalSeguimientoVariable = matrizConfiguracionPresupuestalSeguimientoVariableService.BuscaPorId(matrizConfiguracionPresupuestalSeguimientoVariable.MIRSeguimientoVariableId);
                        if (!StructuralComparisons.StructuralEqualityComparer.Equals(matrizConfiguracionPresupuestalSeguimientoVariable.Timestamp, _matrizConfiguracionPresupuestalSeguimientoVariable.Timestamp))
                        {
                            throw new Exception("La Matriz Configuración Presupuestal Seguimiento Variabe con el código [" + _matrizConfiguracionPresupuestalSeguimientoVariable.MIRSeguimientoVariableId + "] ha sido modificado por otro usuario. Favor de recargar la vista antes de guardar.");
                        }

                        matrizConfiguracionPresupuestalSeguimientoVariable.ModificadoPorId = usuarioId;
                        matrizConfiguracionPresupuestalSeguimientoVariable.FechaUltimaModificacion = DateTime.Now;
                    }
                    else
                    {

                        matrizConfiguracionPresupuestalSeguimientoVariable.CreadoPorId = usuarioId;
                    }
                }
            }

            new MatrizConfiguracionPresupuestalSeguimientoVariableService().GuardaCambios(matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListMatrizConfiguracionPresupuestalSeguimientoVariable);
            
            return Json("Registro guardado con Exito!");
        }

        // GET: MIR/MatrizConfiguracionPresupuestalSeguimientoVariable
        public ActionResult Index()
        {
            return View();
        }

        public override ActionResult Listar()
        {
            MatrizConfiguracionPresupuestalSeguimientoVariableViewModel matrizConfiguracionPresupuestalSeguimientoVariableViewModel = new MatrizConfiguracionPresupuestalSeguimientoVariableViewModel();

            matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListadoMatrizConfiguracionPresupuestalSeguimientoVariable = new MatrizConfiguracionPresupuestalSeguimientoVariableService().BuscaListado();

            return View("ListadoMatrizConfiguracionPresupuestalSeguimientoVariable", matrizConfiguracionPresupuestalSeguimientoVariableViewModel);
        }

        public override ActionResult Nuevo()
        {
            throw new NotImplementedException();
        }

        protected override void GetDatosFicha(ref MatrizConfiguracionPresupuestalSeguimientoVariableViewModel matrizConfiguracionPresupuestalSeguimientoVariableViewModel)
        {
            matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListUnidadMedida = new ControlMaestroUnidadMedidaService().BuscaTodos2();
            // Control Maestro
            matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListFrecuenciaMedicion = new ControlMaestroFrecuenciaMedicionService().BuscaTodos();
            matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListSentido = new ControlMaestroService().BuscaControl("Sentido");
            matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListaControlMaestroControlPeriodo = new ControlMaestroControlPeriodoService().BuscaTodos();
        }

        protected void GetMenuSeguimientoVariable(ref MatrizConfiguracionPresupuestalSeguimientoVariableViewModel matrizConfiguracionPresupuestalSeguimientoVariableViewModel)
        {
            // Variables
            int countIndicador = 1;
            List<MenuSeguimientoVariableModel> listMenuSeguimientoVariable = new List<MenuSeguimientoVariableModel>();
            // Nivel Fin
            MenuSeguimientoVariableModel menuSeguimientoVariableModel = new MenuSeguimientoVariableModel();
            List<MenuItemsSeguimientoVariableModel> listMenuItemsSeguimientoVariable = new List<MenuItemsSeguimientoVariableModel>();
            menuSeguimientoVariableModel.Id = -1;
            menuSeguimientoVariableModel.Text = "Fin";
            matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListMatrizIndicadorResultadoIndicador.Where(miri => miri.NivelIndicadorId == ControlMaestroMapeo.Nivel.FIN).ToList().ForEach(miri =>
            {
                MenuItemsSeguimientoVariableModel menuItemsSeguimientoVariableModel = new MenuItemsSeguimientoVariableModel();
                menuItemsSeguimientoVariableModel.Id = miri.MIRIndicadorId;
                menuItemsSeguimientoVariableModel.Text = "Fin " + countIndicador;
                listMenuItemsSeguimientoVariable.Add(menuItemsSeguimientoVariableModel);
                countIndicador++;
            });
            menuSeguimientoVariableModel.Items = listMenuItemsSeguimientoVariable;
            listMenuSeguimientoVariable.Add(menuSeguimientoVariableModel);
            // Nivel Proposito
            countIndicador = 1;
            menuSeguimientoVariableModel = new MenuSeguimientoVariableModel();
            listMenuItemsSeguimientoVariable = new List<MenuItemsSeguimientoVariableModel>();
            menuSeguimientoVariableModel.Id = -2;
            menuSeguimientoVariableModel.Text = "Propósito";
            matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListMatrizIndicadorResultadoIndicador.Where(miri => miri.NivelIndicadorId == ControlMaestroMapeo.Nivel.PROPOSITO).ToList().ForEach(miri =>
            {
                MenuItemsSeguimientoVariableModel menuItemsSeguimientoVariableModel = new MenuItemsSeguimientoVariableModel();
                menuItemsSeguimientoVariableModel.Id = miri.MIRIndicadorId;
                menuItemsSeguimientoVariableModel.Text = "Propósito " + countIndicador;
                listMenuItemsSeguimientoVariable.Add(menuItemsSeguimientoVariableModel);
                countIndicador++;
            });
            menuSeguimientoVariableModel.Items = listMenuItemsSeguimientoVariable;
            listMenuSeguimientoVariable.Add(menuSeguimientoVariableModel);
            // Nivel Componente
            countIndicador = 1;
            menuSeguimientoVariableModel = new MenuSeguimientoVariableModel();
            listMenuItemsSeguimientoVariable = new List<MenuItemsSeguimientoVariableModel>();
            menuSeguimientoVariableModel.Id = -3;
            menuSeguimientoVariableModel.Text = "Componente";
            matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListMatrizIndicadorResultadoIndicador.Where(miri => miri.NivelIndicadorId == ControlMaestroMapeo.Nivel.COMPONENTE).ToList().ForEach(miri =>
            {
                MenuItemsSeguimientoVariableModel menuItemsSeguimientoVariableModel = new MenuItemsSeguimientoVariableModel();
                menuItemsSeguimientoVariableModel.Id = miri.MIRIndicadorId;
                menuItemsSeguimientoVariableModel.Text = "Componente " + countIndicador;
                listMenuItemsSeguimientoVariable.Add(menuItemsSeguimientoVariableModel);
                countIndicador++;
            });
            menuSeguimientoVariableModel.Items = listMenuItemsSeguimientoVariable;
            listMenuSeguimientoVariable.Add(menuSeguimientoVariableModel);
            // Nivel Actividad
            countIndicador = 1;
            menuSeguimientoVariableModel = new MenuSeguimientoVariableModel();
            listMenuItemsSeguimientoVariable = new List<MenuItemsSeguimientoVariableModel>();
            menuSeguimientoVariableModel.Id = -4;
            menuSeguimientoVariableModel.Text = "Actividad";
            matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListMatrizIndicadorResultadoIndicador.Where(miri => miri.NivelIndicadorId == ControlMaestroMapeo.Nivel.ACTIVIDAD).ToList().ForEach(miri =>
            {
                MenuItemsSeguimientoVariableModel menuItemsSeguimientoVariableModel = new MenuItemsSeguimientoVariableModel();
                menuItemsSeguimientoVariableModel.Id = miri.MIRIndicadorId;
                menuItemsSeguimientoVariableModel.Text = "Actividad " + countIndicador;
                listMenuItemsSeguimientoVariable.Add(menuItemsSeguimientoVariableModel);
                countIndicador++;
            });
            menuSeguimientoVariableModel.Items = listMenuItemsSeguimientoVariable;
            listMenuSeguimientoVariable.Add(menuSeguimientoVariableModel);
            // Asigamos a lista de Menu Seguimiento Variable
            matrizConfiguracionPresupuestalSeguimientoVariableViewModel.ListMenuSeguimientoVariable = listMenuSeguimientoVariable;
        }
    }
}