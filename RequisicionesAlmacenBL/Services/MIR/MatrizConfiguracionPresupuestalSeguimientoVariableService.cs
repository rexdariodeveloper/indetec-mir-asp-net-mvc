using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace RequisicionesAlmacenBL.Services.MIR
{
    public class MatrizConfiguracionPresupuestalSeguimientoVariableService : BaseService<MItblMatrizConfiguracionPresupuestalSeguimientoVariable>
    {
        public override bool Actualiza(MItblMatrizConfiguracionPresupuestalSeguimientoVariable entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad que vamos a actualizar al Context
                Context.MItblMatrizConfiguracionPresupuestalSeguimientoVariable.Add(entidad);
                Context.Entry(entidad).State = EntityState.Modified;

                // Marcar todas las propiedades que no se pueden actualizar como FALSE
                // para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in MItblMatrizConfiguracionPresupuestalSeguimientoVariable.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos true o false si se realizo correctamente la operacion
                return true;
            }
        }

        public override MItblMatrizConfiguracionPresupuestalSeguimientoVariable BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.MItblMatrizConfiguracionPresupuestalSeguimientoVariable.Find(id);
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override MItblMatrizConfiguracionPresupuestalSeguimientoVariable Inserta(MItblMatrizConfiguracionPresupuestalSeguimientoVariable entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad el Context
                MItblMatrizConfiguracionPresupuestalSeguimientoVariable matrizConfiguracionPresupuestalSeguimientoVariable = Context.MItblMatrizConfiguracionPresupuestalSeguimientoVariable.Add(entidad);

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos la entidad que se acaba de guardar en la Base de Datos
                return matrizConfiguracionPresupuestalSeguimientoVariable;
            }
        }

        public void GuardaCambios(IEnumerable<MItblMatrizConfiguracionPresupuestalSeguimientoVariable> listaMatrizConfiguracionPresupuestalSeguimientoVariable)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    // Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    // Existe la lista para guardar o actualizar
                    if (listaMatrizConfiguracionPresupuestalSeguimientoVariable != null)
                    {
                        listaMatrizConfiguracionPresupuestalSeguimientoVariable.ToList().ForEach(sv =>
                        {
                            // Creamos el return
                            MItblMatrizConfiguracionPresupuestalSeguimientoVariable matrizConfiguracionPresupuestalSeguimientoVariable = sv;

                            // Si es un registro nuevo
                            if (matrizConfiguracionPresupuestalSeguimientoVariable.MIRSeguimientoVariableId > 0)
                            {
                                Actualiza(matrizConfiguracionPresupuestalSeguimientoVariable);
                            }
                            else
                            {
                                matrizConfiguracionPresupuestalSeguimientoVariable = Inserta(matrizConfiguracionPresupuestalSeguimientoVariable);

                            }
                            
                        });
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

        public IEnumerable<MIvwListadoMatrizConfiguracionPresupuestalSeguimientoVariable> BuscaListado()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MIvwListadoMatrizConfiguracionPresupuestalSeguimientoVariable.ToList();
            }
        }

        public MItblMatrizConfiguracionPresupuestalSeguimientoVariable BuscaPorMIRIndicadorFormulaVariableId(int MIRIndicadorFormulaVariableId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblMatrizConfiguracionPresupuestalSeguimientoVariable.Where(sv => sv.MIRIndicadorFormulaVariableId == MIRIndicadorFormulaVariableId).FirstOrDefault();
            }
        }

        public IEnumerable<MIspConsultaListaVariableIndicador_Result> BuscaReportePorMIRId(int mirId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MIspConsultaListaVariableIndicador(mirId).ToList();
            }
        }

        public IEnumerable<MIfnObtenerSeguimientoIndicadorDesempenio_Result> BuscaReporteSIDPorMIRIndicadorId(int mirIndicadorId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MIfnObtenerSeguimientoIndicadorDesempenio(mirIndicadorId).ToList();
            }
        }
    }
}
