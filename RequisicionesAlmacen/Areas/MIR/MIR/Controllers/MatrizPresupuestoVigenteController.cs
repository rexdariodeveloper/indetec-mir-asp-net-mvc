using RequisicionesAlmacen.Areas.MIR.MIR.Models.ViewModel;
using RequisicionesAlmacen.Controllers;
using RequisicionesAlmacen.Helpers;
using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Models.Mapeos;
using RequisicionesAlmacenBL.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RequisicionesAlmacen.Areas.MIR.MIR.Controllers
{
    [Authenticated(nodoMenuId = MenuPrincipalMapeo.ID.MATRIZ_PRESUPUESTO_VIGENTE)]
    public class MatrizPresupuestoVigenteController : BaseController<MatrizPresupuestoVigenteViewModel, MatrizPresupuestoVigenteViewModel>
    {
        public override ActionResult Editar(int id)
        {
            // Ejercicio desde sesion (iniciar sesion)
            string ejercicio = SessionHelper.GetUsuario().Ejercicio;

            // Crear los objetos nuevos
            MatrizPresupuestoVigenteViewModel matrizPresupuestoVigenteViewModel = new MatrizPresupuestoVigenteViewModel();
            matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestalModel = new MItblMatrizConfiguracionPresupuestal();
            matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestalDetalleModel = new MItblMatrizConfiguracionPresupuestalDetalle();

            // Buscamos los objetos por el ID de MIR que se envio como parametro
            matrizPresupuestoVigenteViewModel.ConsultaMatrizIndicadorResultado = new MatrizIndicadorResultadoService().ConsultaMatrizIndicadorResultado(id);
            if (matrizPresupuestoVigenteViewModel.ConsultaMatrizIndicadorResultado == null)
            {
                return new HttpNotFoundResult("La MIR no existe la solicitud.");
            }
            matrizPresupuestoVigenteViewModel.ListaMIRIComponente = new MatrizIndicadorResultadoIndicadorService().BuscaListaPorMIRIndicadorComponenteId(id);
            matrizPresupuestoVigenteViewModel.ListaMatrizIndicadorResultadoIndicador = new MatrizIndicadorResultadoIndicadorService().BuscaPorMIRIdYNivelActividadPorComponente(id);

            // Buscamos el objeto de Matriz Configuracion Presupuestal por el ID de MIR
            matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestal = new MatrizConfiguracionPresupuestalService().BuscaPorMIRId(id);
            if (matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestal == null)
            {
                matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestal = new MItblMatrizConfiguracionPresupuestal();
                matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestal.ConfiguracionPresupuestoId = -1;
                matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestal.MIRId = id;
                matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestal.EstatusId = ControlMaestroMapeo.EstatusRegistro.ACTIVO;

                matrizPresupuestoVigenteViewModel.ListaMatrizConfiguracionPresupuestalDetalle = new List<MItblMatrizConfiguracionPresupuestalDetalle>();
            }
            else
            {
                matrizPresupuestoVigenteViewModel.ListaMatrizConfiguracionPresupuestalDetalle = new MatrizConfiguracionPresupuestalDetalleService().BuscaPorConfiguracionPresupuestoIdYClasificadorId(matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestal.ConfiguracionPresupuestoId, ControlMaestroMapeo.TipoPresupuesto.POR_EJERCER);
            }

            matrizPresupuestoVigenteViewModel.ListaConsultaPresupuestoVigente = new MatrizConfiguracionPresupuestalDetalleService().BuscaDetalles(id, ejercicio);


            //Agregamos todos los datos necesarios para el funcionamiento de la ficha
            //como son los Listados para combos, tablas, arboles.
            GetDatosFicha(ref matrizPresupuestoVigenteViewModel);

            //Retornamos la vista junto con su Objeto Modelo
            return View("MatrizPresupuestoVigente", matrizPresupuestoVigenteViewModel);
        }

        public override JsonResult Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        [JsonException]
        public override JsonResult Guardar(MatrizPresupuestoVigenteViewModel matrizPresupuestoVigenteViewModel)
        {
            // Usuario
            int usuarioId = SessionHelper.GetUsuario().UsuarioId;

            // Matriz Indicador Resultado
            if (matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestal != null)
            {
                // Si el ID es nuevo para registrar o actualizar
                if (matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestal.ConfiguracionPresupuestoId > 0)
                {
                    // Verificar si el usuario ha sido modificado en mismo MCP y regresa mensaje de error
                    MItblMatrizConfiguracionPresupuestal matrizConfiguracionPresupuestal = new MatrizConfiguracionPresupuestalService().BuscaPorId(matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestal.ConfiguracionPresupuestoId);
                    if (!StructuralComparisons.StructuralEqualityComparer.Equals(matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestal.Timestamp, matrizConfiguracionPresupuestal.Timestamp))
                    {
                        throw new Exception("La Matriz Configuración Presupuestal con el código [" + matrizConfiguracionPresupuestal.ConfiguracionPresupuestoId + "] ha sido modificado por otro usuario. Favor de recargar la vista antes de guardar.");
                    }

                    matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestal.ModificadoPorId = usuarioId;
                    matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestal.FechaUltimaModificacion = DateTime.Now;
                }
                else
                {
                    matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestal.CreadoPorId = usuarioId;
                }
            }
            // Matriz Configuracion Presupuestal Detalle
            if (matrizPresupuestoVigenteViewModel.ListaMatrizConfiguracionPresupuestalDetalle != null)
            {
                // Sin Actividad es para cuando guardar MIRI para poner el MIRIndicadorId a MIRIndicadorComponenteId
                foreach (MItblMatrizConfiguracionPresupuestalDetalle matrizConfiguracionPresupuestalDetalle in matrizPresupuestoVigenteViewModel.ListaMatrizConfiguracionPresupuestalDetalle)
                {
                    // Si el ID es nuevo para registrar o actualizar
                    if (matrizConfiguracionPresupuestalDetalle.ConfiguracionPresupuestoDetalleId > 0)
                    {
                        // Verificar si el usuario ha sido modificado en mismo MCPD y regresa mensaje de error
                        MItblMatrizConfiguracionPresupuestalDetalle _matrizConfiguracionPresupuestalDetalle = new MatrizConfiguracionPresupuestalDetalleService().BuscaPorId(matrizConfiguracionPresupuestalDetalle.ConfiguracionPresupuestoDetalleId);
                        if (!StructuralComparisons.StructuralEqualityComparer.Equals(matrizConfiguracionPresupuestalDetalle.Timestamp, _matrizConfiguracionPresupuestalDetalle.Timestamp))
                        {
                            throw new Exception("La Matriz Configuración Presupuestal Detalle con el código [" + _matrizConfiguracionPresupuestalDetalle.ConfiguracionPresupuestoDetalleId + "] ha sido modificado por otro usuario. Favor de recargar la vista antes de guardar.");
                        }

                        matrizConfiguracionPresupuestalDetalle.ModificadoPorId = usuarioId;
                        matrizConfiguracionPresupuestalDetalle.FechaUltimaModificacion = DateTime.Now;
                    }
                    else
                    {
                        matrizConfiguracionPresupuestalDetalle.CreadoPorId = usuarioId;
                    }
                }
            }
            new MatrizConfiguracionPresupuestalService().GuardaCambios(matrizPresupuestoVigenteViewModel.MatrizConfiguracionPresupuestal, matrizPresupuestoVigenteViewModel.ListaMatrizConfiguracionPresupuestalDetalle);

            return Json("Registro guardado con Exito!");
        }

        // GET: MIR/MatrizPresupuestoVigente
        public ActionResult Index()
        {
            return View();
        }

        public override ActionResult Listar()
        {
            MatrizPresupuestoVigenteViewModel matrizPresupuestoVigenteViewModel = new MatrizPresupuestoVigenteViewModel();

            matrizPresupuestoVigenteViewModel.ListadoMatrizConfiguracionPresupuestal = new MatrizConfiguracionPresupuestalService().BuscaListado();

            return View("ListadoMatrizPresupuestoVigente", matrizPresupuestoVigenteViewModel);
        }

        public override ActionResult Nuevo()
        {
            throw new NotImplementedException();
        }

        protected override void GetDatosFicha(ref MatrizPresupuestoVigenteViewModel matrizPresupuestoVigenteViewModel)
        {
            matrizPresupuestoVigenteViewModel.ListaControlMaestroControlPeriodo = new ControlMaestroControlPeriodoService().BuscaTodos();
        }

        [JsonException]
        public JsonResult ExisteActividad(int mirId)
        {
            if (new MatrizIndicadorResultadoIndicadorService().BuscaPorMIRIdYNivelActividad(mirId).Count() == 0)
                throw new Exception("La MIR no tiene las actividades, necesita agregar unos componentes para seguir.");

            return Json("");
        }
    }
}