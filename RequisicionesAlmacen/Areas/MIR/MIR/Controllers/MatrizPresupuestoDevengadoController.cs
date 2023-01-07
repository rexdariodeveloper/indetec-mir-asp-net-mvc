using RequisicionesAlmacen.Areas.MIR.MIR.Models.ViewModel;
using RequisicionesAlmacen.Controllers;
using RequisicionesAlmacen.Helpers;
using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Models.Mapeos;
using RequisicionesAlmacenBL.Services;
using RequisicionesAlmacenBL.Services.SAACG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RequisicionesAlmacen.Areas.MIR.MIR.Controllers
{
    [Authenticated(nodoMenuId = MenuPrincipalMapeo.ID.MATRIZ_PRESUPUESTO_DEVENGADO)]
    public class MatrizPresupuestoDevengadoController : BaseController<MatrizPresupuestoDevengadoViewModel, MatrizPresupuestoDevengadoViewModel>
    {
        public override ActionResult Editar(int id)
        {
            // Crear los objetos nuevos
            MatrizPresupuestoDevengadoViewModel matrizPresupuestoDevengadoViewModel = new MatrizPresupuestoDevengadoViewModel();
            matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestalModel = new MItblMatrizConfiguracionPresupuestal();
            matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestalDetalleModel = new MItblMatrizConfiguracionPresupuestalDetalle();
            // Buscamos los objetos por el ID de MIR que se envio como parametro
            matrizPresupuestoDevengadoViewModel.ConsultaMatrizIndicadorResultado = new MatrizIndicadorResultadoService().ConsultaMatrizIndicadorResultado(id);
            if (matrizPresupuestoDevengadoViewModel.ConsultaMatrizIndicadorResultado == null)
            {
                return new HttpNotFoundResult("La MIR no existe la solicitud.");
            }
            matrizPresupuestoDevengadoViewModel.ListaMIRIComponente = new MatrizIndicadorResultadoIndicadorService().BuscaListaPorMIRIndicadorComponenteId(id);
            matrizPresupuestoDevengadoViewModel.ListaMatrizIndicadorResultadoIndicador = new MatrizIndicadorResultadoIndicadorService().BuscaPorMIRIdYNivelActividadPorComponente(id);
            // Buscamos el objeto de Matriz Configuracion Presupuestal por el ID de MIR
            matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestal = new MatrizConfiguracionPresupuestalService().BuscaPorMIRId(id);
            if (matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestal == null)
            {
                matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestal = new MItblMatrizConfiguracionPresupuestal();
                matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestal.ConfiguracionPresupuestoId = -1;
                matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestal.MIRId = id;
                matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestal.EstatusId = ControlMaestroMapeo.EstatusRegistro.ACTIVO;

                matrizPresupuestoDevengadoViewModel.ListaMatrizConfiguracionPresupuestalDetalle = new List<MItblMatrizConfiguracionPresupuestalDetalle>();
            }
            else
            {
                matrizPresupuestoDevengadoViewModel.ListaMatrizConfiguracionPresupuestalDetalle = new MatrizConfiguracionPresupuestalDetalleService().BuscaPorConfiguracionPresupuestoIdYClasificadorId(matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestal.ConfiguracionPresupuestoId, ControlMaestroMapeo.TipoPresupuesto.DEVENGADO);
            }

            // Devengado
            // Service
            ProyectoService proyectoService = new ProyectoService();
            // 12 Meses: Enero a Diciembre
            List<DevengadoModel> listaDevegado = new List<DevengadoModel>();
            List<int> listaMIRIndicadorComponenteId = new List<int>();
            // Relacion Componente - Proyecto
            matrizPresupuestoDevengadoViewModel.ListaMatrizIndicadorResultadoIndicador.Where(miri => miri.TipoComponenteId == ControlMaestroMapeo.TipoComponente.RELACION_COMPONENTE).GroupBy(miri => miri.MIRIndicadorComponenteId).Select(miri => miri.Key).ToList().ForEach(miri =>
            {
                listaMIRIndicadorComponenteId.Add(miri.Value);
            });
            // Relacion Actividad - Proyecto
            matrizPresupuestoDevengadoViewModel.ListaMatrizIndicadorResultadoIndicador.Where(miri => miri.TipoComponenteId == ControlMaestroMapeo.TipoComponente.RELACION_ACTIVIDAD).ToList().ForEach(miri =>
            {
                listaMIRIndicadorComponenteId.Add(miri.MIRIndicadorId);
            });
            string listaProyectoId = new MatrizIndicadorResultadoIndicadorService().BuscaPorProyecto(listaMIRIndicadorComponenteId);
            List<MIspConsultaRepProyecto_Result> listaConsultaRepProyecto = new List<MIspConsultaRepProyecto_Result>();
            listaConsultaRepProyecto.AddRange(new MatrizConfiguracionPresupuestalDetalleService().BuscaDevengado(listaProyectoId, matrizPresupuestoDevengadoViewModel.ConsultaMatrizIndicadorResultado.Ejercicio));
            // Agregan dos IDs de Tipo Componente -> Relacion Componente y Relacion Actividad
            List<int> listaTipoComponente = new List<int> { ControlMaestroMapeo.TipoComponente.RELACION_COMPONENTE, ControlMaestroMapeo.TipoComponente.RELACION_ACTIVIDAD };
            listaMIRIndicadorComponenteId.ForEach(mirIndicadorComponenteId => {
                matrizPresupuestoDevengadoViewModel.ListaMatrizIndicadorResultadoIndicador.Where(miri => listaTipoComponente.Contains(miri.TipoComponenteId.Value) && (miri.MIRIndicadorComponenteId == mirIndicadorComponenteId || miri.MIRIndicadorId == mirIndicadorComponenteId)).ToList().ForEach(miri => {
                    // Porcentaje 
                    decimal? porcentaje = miri.TipoComponenteId == ControlMaestroMapeo.TipoComponente.RELACION_COMPONENTE ? miri.PorcentajeActividad : miri.PorcentajeProyecto;
                    // Crear los objetos
                    DevengadoModel devengadoModel = new DevengadoModel();
                    // Asignamos el ID de MIRIndicador
                    devengadoModel.MIRIndicadorId = miri.MIRIndicadorId;

                    string proyectoId = new MatrizIndicadorResultadoIndicadorService().BuscaPorMIRIndicadorComponenteId(mirIndicadorComponenteId).ProyectoId;
                    MIspConsultaRepProyecto_Result consultaRepProyecto = listaConsultaRepProyecto.Where(proyecto => proyecto.ProyectoId == proyectoId).FirstOrDefault();
                    devengadoModel.Enero = consultaRepProyecto.Enero.Value * (porcentaje.Value / 100);
                    devengadoModel.Febrero = consultaRepProyecto.Febrero.Value * (porcentaje.Value / 100);
                    devengadoModel.Marzo = consultaRepProyecto.Marzo.Value * (porcentaje.Value / 100);
                    devengadoModel.Abril = consultaRepProyecto.Abril.Value * (porcentaje.Value / 100);
                    devengadoModel.Mayo = consultaRepProyecto.Mayo.Value * (porcentaje.Value / 100);
                    devengadoModel.Junio = consultaRepProyecto.Junio.Value * (porcentaje.Value / 100);
                    devengadoModel.Julio = consultaRepProyecto.Julio.Value * (porcentaje.Value / 100);
                    devengadoModel.Agosto = consultaRepProyecto.Agosto.Value * (porcentaje.Value / 100);
                    devengadoModel.Septiembre = consultaRepProyecto.Septiembre.Value * (porcentaje.Value / 100);
                    devengadoModel.Octubre = consultaRepProyecto.Octubre.Value * (porcentaje.Value / 100);
                    devengadoModel.Noviembre = consultaRepProyecto.Noviembre.Value * (porcentaje.Value / 100);
                    devengadoModel.Diciembre = consultaRepProyecto.Diciembre.Value * (porcentaje.Value / 100);
                    devengadoModel.Anual = devengadoModel.Enero + devengadoModel.Febrero + devengadoModel.Marzo + devengadoModel.Abril + devengadoModel.Mayo + devengadoModel.Junio + devengadoModel.Julio + devengadoModel.Agosto + devengadoModel.Septiembre + devengadoModel.Octubre + devengadoModel.Noviembre + devengadoModel.Diciembre;
                    listaDevegado.Add(devengadoModel);

                });
            });
            matrizPresupuestoDevengadoViewModel.ListaDevengado = listaDevegado;
            //Agregamos todos los datos necesarios para el funcionamiento de la ficha
            //como son los Listados para combos, tablas, arboles.
            GetDatosFicha(ref matrizPresupuestoDevengadoViewModel);
            //Retornamos la vista junto con su Objeto Modelo
            return View("MatrizPresupuestoDevengado", matrizPresupuestoDevengadoViewModel);
        }

        public override JsonResult Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        [JsonException]
        public override JsonResult Guardar(MatrizPresupuestoDevengadoViewModel matrizPresupuestoDevengadoViewModel)
        {
            // Usuario
            int usuarioId = SessionHelper.GetUsuario().UsuarioId;

            // Matriz Indicador Resultado
            if (matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestal != null)
            {
                // Si el ID es nuevo para registrar o actualizar
                if (matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestal.ConfiguracionPresupuestoId > 0)
                {
                    // Verificar si el usuario ha sido modificado en mismo MCP y regresa mensaje de error
                    MItblMatrizConfiguracionPresupuestal matrizConfiguracionPresupuestal = new MatrizConfiguracionPresupuestalService().BuscaPorId(matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestal.ConfiguracionPresupuestoId);
                    if (!StructuralComparisons.StructuralEqualityComparer.Equals(matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestal.Timestamp, matrizConfiguracionPresupuestal.Timestamp))
                    {
                        throw new Exception("La Matriz Configuración Presupuestal con el código [" + matrizConfiguracionPresupuestal.ConfiguracionPresupuestoId + "] ha sido modificado por otro usuario. Favor de recargar la vista antes de guardar.");
                    }

                    matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestal.ModificadoPorId = usuarioId;
                    matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestal.FechaUltimaModificacion = DateTime.Now;
                }
                else
                {
                    matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestal.CreadoPorId = usuarioId;
                }
            }
            // Matriz Configuracion Presupuestal Detalle
            if (matrizPresupuestoDevengadoViewModel.ListaMatrizConfiguracionPresupuestalDetalle != null)
            {
                // Sin Actividad es para cuando guardar MIRI para poner el MIRIndicadorId a MIRIndicadorComponenteId
                foreach (MItblMatrizConfiguracionPresupuestalDetalle matrizConfiguracionPresupuestalDetalle in matrizPresupuestoDevengadoViewModel.ListaMatrizConfiguracionPresupuestalDetalle)
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
            new MatrizConfiguracionPresupuestalService().GuardaCambios(matrizPresupuestoDevengadoViewModel.MatrizConfiguracionPresupuestal, matrizPresupuestoDevengadoViewModel.ListaMatrizConfiguracionPresupuestalDetalle);

            return Json("Registro guardado con Exito!");
        }

        // GET: MIR/MatrizPresupuestoDevengado
        public ActionResult Index()
        {
            return View();
        }

        public override ActionResult Listar()
        {
            MatrizPresupuestoDevengadoViewModel matrizPresupuestoDevengadoViewModel = new MatrizPresupuestoDevengadoViewModel();

            matrizPresupuestoDevengadoViewModel.ListadoMatrizConfiguracionPresupuestal = new MatrizConfiguracionPresupuestalService().BuscaListado();

            return View("ListadoMatrizPresupuestoDevengado", matrizPresupuestoDevengadoViewModel);
        }

        public override ActionResult Nuevo()
        {
            throw new NotImplementedException();
        }

        protected override void GetDatosFicha(ref MatrizPresupuestoDevengadoViewModel matrizPresupuestoDevengadoViewModel)
        {
            matrizPresupuestoDevengadoViewModel.ListaControlMaestroControlPeriodo = new ControlMaestroControlPeriodoService().BuscaTodos();
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