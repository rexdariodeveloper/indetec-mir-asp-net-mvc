﻿using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using RequisicionesAlmacenBL.Models.Mapeos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace RequisicionesAlmacenBL.Services
{
    public class MatrizIndicadorResultadoService : BaseService<MItblMatrizIndicadorResultado>
    {
        public override bool Actualiza(MItblMatrizIndicadorResultado entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad que vamos a actualizar al Context
                Context.MItblMatrizIndicadorResultado.Add(entidad);
                Context.Entry(entidad).State = EntityState.Modified;

                // Marcar todas las propiedades que no se pueden actualizar como FALSE
                // para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in MItblMatrizIndicadorResultado.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos true o false si se realizo correctamente la operacion
                return true;
            }
        }

        public override MItblMatrizIndicadorResultado BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Retornamos la entidad con el ID que se envio como parametro
                return Context.MItblMatrizIndicadorResultado.Where(mir => mir.MIRId == id).FirstOrDefault();
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override MItblMatrizIndicadorResultado Inserta(MItblMatrizIndicadorResultado entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad el Context
                MItblMatrizIndicadorResultado matrizIndicadorResultado = Context.MItblMatrizIndicadorResultado.Add(entidad);

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos la entidad que se acaba de guardar en la Base de Datos
                return matrizIndicadorResultado;
            }
        }

        public void GuardaCambios(MItblMatrizIndicadorResultado matrizIndicadorResultado, IEnumerable<MItblMatrizIndicadorResultadoIndicador> listaMatrizIndicadorResultadoIndicador, IEnumerable<MItblMatrizIndicadorResultadoIndicadorMeta> listaMatrizIndicadorResultadoIndicadorMeta, IEnumerable<MItblMatrizIndicadorResultadoIndicadorFormulaVariable> listaMatrizIndicadorResultadoIndicadorFormulaVariable)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    // Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    // Creamos el return
                    MItblMatrizIndicadorResultado _matrizIndicadorResultado = matrizIndicadorResultado;

                    if (_matrizIndicadorResultado != null)
                    {
                        // Si es un registro nuevo
                        if (_matrizIndicadorResultado.MIRId > 0)
                        {
                            Actualiza(_matrizIndicadorResultado);
                        }
                        else
                        {
                            _matrizIndicadorResultado = Inserta(_matrizIndicadorResultado);

                        }
                    }

                    // Existe la lista para guardar o actualizar
                    if (listaMatrizIndicadorResultadoIndicador != null)
                    {
                        GuardaListaMatrizIndicadorResultadoIndicador(_matrizIndicadorResultado, listaMatrizIndicadorResultadoIndicador, ref listaMatrizIndicadorResultadoIndicadorMeta, ref listaMatrizIndicadorResultadoIndicadorFormulaVariable);
                    }

                    // Existe la lista para guardar o actualizar
                    if (listaMatrizIndicadorResultadoIndicadorMeta != null)
                    {
                        GuardaListaMatrizIndicadorResultadoIndicadorMeta(listaMatrizIndicadorResultadoIndicadorMeta);
                    }

                    // Existe la lista para guardar o actualizar
                    if (listaMatrizIndicadorResultadoIndicadorFormulaVariable != null)
                    {
                        GuardaListaMatrizIndicadorResultadoIndicadorFormulaVariable(listaMatrizIndicadorResultadoIndicadorFormulaVariable);
                    }

                    // Hacemos el Commit
                    SAACGContextHelper.Commit();
                }
                catch (DbEntityValidationException ex)
                {
                    // Hacemos el Rollback
                    SAACGContextHelper.Rollback();

                    throw new Exception(UserExceptionHelper.GetMessage(ex));
                }
                catch (Exception ex)
                {
                    // Hacemos el Rollback
                    SAACGContextHelper.Rollback();

                    throw new Exception(UserExceptionHelper.GetMessage(ex));
                }
            }
        }

        public void GuardaListaMatrizIndicadorResultadoIndicador(MItblMatrizIndicadorResultado matrizIndicadorResultado, IEnumerable<MItblMatrizIndicadorResultadoIndicador> listaMatrizIndicadorResultadoIndicador, ref IEnumerable<MItblMatrizIndicadorResultadoIndicadorMeta> listaMatrizIndicadorResultadoIndicadorMeta, ref IEnumerable<MItblMatrizIndicadorResultadoIndicadorFormulaVariable> listaMatrizIndicadorResultadoIndicadorFormulaVariable)
        {
            // Service
            MatrizIndicadorResultadoIndicadorService matrizIndicadorResultadoIndicadorService = new MatrizIndicadorResultadoIndicadorService();

            for(int x = 0; x < 2; x++)
            {
                // (x = 0) = Sin Actividad y (x = 1) = Con Actividad
                int NivelIndicadorId = x == 0 ? ControlMaestroMapeo.Nivel.ACTIVIDAD : 0;
                foreach (MItblMatrizIndicadorResultadoIndicador matrizIndicadorResultadoIndicador in listaMatrizIndicadorResultadoIndicador.Where(miri => x == 0 ? miri.NivelIndicadorId != ControlMaestroMapeo.Nivel.ACTIVIDAD : miri.NivelIndicadorId == ControlMaestroMapeo.Nivel.ACTIVIDAD).ToList())
                {
                    int idPadre = matrizIndicadorResultadoIndicador.MIRIndicadorId;

                    // Creamos el return
                    MItblMatrizIndicadorResultadoIndicador _matrizIndicadorResultadoIndicador = matrizIndicadorResultadoIndicador;

                    // Si es un registro
                    if (_matrizIndicadorResultadoIndicador.MIRIndicadorId > 0)
                    {
                        // Actualizamos
                        matrizIndicadorResultadoIndicadorService.Actualiza(_matrizIndicadorResultadoIndicador);
                    }
                    else
                    {
                        if (matrizIndicadorResultado != null)
                        {
                            if (_matrizIndicadorResultadoIndicador.MIRId <= 0)
                            {
                                // Asignamos el Id de la cabecera
                                _matrizIndicadorResultadoIndicador.MIRId = matrizIndicadorResultado.MIRId;
                            }
                        }
                        // Guardamos
                        _matrizIndicadorResultadoIndicador = matrizIndicadorResultadoIndicadorService.Inserta(_matrizIndicadorResultadoIndicador);
                    }

                    // Sin Actividad
                    if(_matrizIndicadorResultadoIndicador.NivelIndicadorId != ControlMaestroMapeo.Nivel.ACTIVIDAD)
                    {
                        listaMatrizIndicadorResultadoIndicador.Where(miri => miri.NivelIndicadorId == ControlMaestroMapeo.Nivel.ACTIVIDAD && miri.MIRIndicadorComponenteId == idPadre).ToList().ForEach(miri =>
                        {
                            miri.MIRIndicadorComponenteId = _matrizIndicadorResultadoIndicador.MIRIndicadorId;
                        });
                    }

                    // Matriz Indicador Resultado Indicador Meta
                    // Para cambiar el ID padre de MIRI a Matriz Indicador Resultado Indicador Meta
                    if (listaMatrizIndicadorResultadoIndicadorMeta != null)
                    {
                        listaMatrizIndicadorResultadoIndicadorMeta.Where(meta => meta.MIRIndicadorId == idPadre).ToList().ForEach(meta =>
                        {
                            meta.MIRIndicadorId = _matrizIndicadorResultadoIndicador.MIRIndicadorId;
                        });
                    }

                    // Matriz Indicador Resultado Indicador Formula Variable
                    // Para cambiar el ID padre de MIRI a Matriz Indicador Resultado Indicador Formula Variable
                    if (listaMatrizIndicadorResultadoIndicadorFormulaVariable != null)
                    {
                        listaMatrizIndicadorResultadoIndicadorFormulaVariable.Where(fv => fv.MIRIndicadorId == idPadre).ToList().ForEach(fv => {
                            fv.MIRIndicadorId = _matrizIndicadorResultadoIndicador.MIRIndicadorId;
                        });
                    }
                }
            }

            
        }

        public void GuardaListaMatrizIndicadorResultadoIndicadorMeta(IEnumerable<MItblMatrizIndicadorResultadoIndicadorMeta> listaMatrizIndicadorResultadoIndicadorMeta)
        {
            // Service
            MatrizIndicadorResultadoIndicadorMetaService matrizIndicadorResultadoIndicadorMetaService = new MatrizIndicadorResultadoIndicadorMetaService();

            foreach (MItblMatrizIndicadorResultadoIndicadorMeta matrizIndicadorResultadoIndicadorMeta in listaMatrizIndicadorResultadoIndicadorMeta.ToList())
            {
                // Creamos el return
                MItblMatrizIndicadorResultadoIndicadorMeta _matrizIndicadorResultadoIndicadorMeta = matrizIndicadorResultadoIndicadorMeta;

                // Si es un registro
                if (_matrizIndicadorResultadoIndicadorMeta.MIRIndicadorMetaId > 0)
                {
                    // Actualizamos
                    matrizIndicadorResultadoIndicadorMetaService.Actualiza(_matrizIndicadorResultadoIndicadorMeta);
                }
                else
                {
                    // Guardamos
                    _matrizIndicadorResultadoIndicadorMeta = matrizIndicadorResultadoIndicadorMetaService.Inserta(_matrizIndicadorResultadoIndicadorMeta);
                }
            }
        }

        public void GuardaListaMatrizIndicadorResultadoIndicadorFormulaVariable(IEnumerable<MItblMatrizIndicadorResultadoIndicadorFormulaVariable> listaMatrizIndicadorResultadoIndicadorFormulaVariable)
        {
            // Service
            MatrizIndicadorResultadoIndicadorFormulaVariableService matrizIndicadorResultadoIndicadorFormulaVariableService = new MatrizIndicadorResultadoIndicadorFormulaVariableService();

            foreach (MItblMatrizIndicadorResultadoIndicadorFormulaVariable matrizIndicadorResultadoIndicadorFormulaVariable in listaMatrizIndicadorResultadoIndicadorFormulaVariable.ToList())
            {
                // Creamos el return
                MItblMatrizIndicadorResultadoIndicadorFormulaVariable _matrizIndicadorResultadoIndicadorFormulaVariable = matrizIndicadorResultadoIndicadorFormulaVariable;

                // Si es un registro
                if (_matrizIndicadorResultadoIndicadorFormulaVariable.MIRIndicadorFormulaVariableId > 0)
                {
                    // Actualizamos
                    matrizIndicadorResultadoIndicadorFormulaVariableService.Actualiza(_matrizIndicadorResultadoIndicadorFormulaVariable);
                }
                else
                {
                    // Guardamos
                    _matrizIndicadorResultadoIndicadorFormulaVariable = matrizIndicadorResultadoIndicadorFormulaVariableService.Inserta(_matrizIndicadorResultadoIndicadorFormulaVariable);
                }
            }
        }

        public IEnumerable<MIvwListadoMIR> BuscaListado()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MIvwListadoMIR.ToList();
            }
        }

        public MIspConsultaMatrizIndicadorResultado_Result ConsultaMatrizIndicadorResultado(int mirId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MIspConsultaMatrizIndicadorResultado(mirId).FirstOrDefault();
            }
        }

        public IEnumerable<MItblMatrizIndicadorResultado> BuscaPorEjercicioYProgramaPresupuestario(string ejercicio, string programaPresupuestarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblMatrizIndicadorResultado.Where(mir => mir.Ejercicio == ejercicio && mir.ProgramaPresupuestarioId == programaPresupuestarioId && mir.FechaFinConfiguracion <= DateTime.Now && mir.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO).ToList();
            }
        }

        public MIspRptLibroConsultaMatrizIndicadorResultado_Result BuscaReportePorMIRId(int mirId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MIspRptLibroConsultaMatrizIndicadorResultado(mirId).FirstOrDefault();
            }
        }

        public IEnumerable<MItblMatrizIndicadorResultado> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblMatrizIndicadorResultado.Include("MItblPlanDesarrollo").Where(mir => mir.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO).ToList();
            }
        }

        public IEnumerable<MIspRptLibroConsultaListaMatrizIndicadorResultado_Result> BuscaReporteListaMIRPorMIRId(int mirId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MIspRptLibroConsultaListaMatrizIndicadorResultado(mirId).ToList();
            }
        }

        public IEnumerable<MIvwComboListadoMIR> BuscaComboListadoMIR()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MIvwComboListadoMIR.ToList();
            }
        }

        public Boolean BuscaExisteProgramaPresupuestario(int mirId, string programaPresupuestarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                if(mirId > 0)
                {
                    return Context.MItblMatrizIndicadorResultado.Any(mir => mir.MIRId != mirId && mir.ProgramaPresupuestarioId == programaPresupuestarioId);
                }
                else
                {
                    return Context.MItblMatrizIndicadorResultado.Any(mir => mir.ProgramaPresupuestarioId == programaPresupuestarioId && mir.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO);
                }
            }
        }

        public spObtenerFechaDelServidor_Result BuscaFechaDelServidor()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.spObtenerFechaDelServidor().FirstOrDefault();
            }
        }
    }
}
