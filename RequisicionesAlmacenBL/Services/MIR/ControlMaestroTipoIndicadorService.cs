using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RequisicionesAlmacenBL.Services
{
    public class ControlMaestroTipoIndicadorService : BaseService<MItblControlMaestroTipoIndicador>
    {
        public override bool Actualiza(MItblControlMaestroTipoIndicador entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                using (var ContextTransaction = Context.Database.BeginTransaction())
                {
                    try
                    {
                        // Agregamos la entidad que vamos a actualizar al Context
                        Context.MItblControlMaestroTipoIndicador.Add(entidad);
                        Context.Entry(entidad).State = EntityState.Modified;

                        // Marcar todas las propiedades que no se pueden actualizar como FALSE
                        // para que no se actualice su informacion en Base de Datos
                        foreach (string propertyName in MItblControlMaestroTipoIndicador.PropiedadesNoActualizables)
                        {
                            Context.Entry(entidad).Property(propertyName).IsModified = false;
                        }

                        // Guardamos cambios
                        Context.SaveChanges();

                        // Hacemos el Commit
                        ContextTransaction.Commit();

                        // Retornamos true o false si se realizo correctamente la operacion
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Hacemos el Rollback
                        ContextTransaction.Rollback();

                        throw new Exception("Error al guardar: " + ex);
                    }
                }
            }
        }

        public override MItblControlMaestroTipoIndicador BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.MItblControlMaestroTipoIndicador.Find(id);
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override MItblControlMaestroTipoIndicador Inserta(MItblControlMaestroTipoIndicador entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                using (var ContextTransaction = Context.Database.BeginTransaction())
                {
                    try
                    {
                        // Agregamos la entidad el Context
                        MItblControlMaestroTipoIndicador controlMaestroTipoIndicador = Context.MItblControlMaestroTipoIndicador.Add(entidad);

                        // Guardamos cambios
                        Context.SaveChanges();

                        // Hacemos el Commit
                        ContextTransaction.Commit();

                        // Retornamos la entidad que se acaba de guardar en la Base de Datos
                        return controlMaestroTipoIndicador;
                    }
                    catch (Exception ex)
                    {
                        // Hacemos el Rollback
                        ContextTransaction.Rollback();

                        throw new Exception("Error al guardar: " + ex);
                    }
                }
            }
        }

        public IEnumerable<MItblControlMaestroTipoIndicador> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblControlMaestroTipoIndicador.Where(ti => ti.Borrado == false).ToList();
            }
        }

        public Boolean EsDescripcionExiste(MItblControlMaestroTipoIndicador controlMaestroTipoIndicador)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblControlMaestroTipoIndicador.Any(ti => ti.Descripcion == controlMaestroTipoIndicador.Descripcion && ti.TipoIndicadorId != controlMaestroTipoIndicador.TipoIndicadorId && ti.Borrado == false);
            }
        }

        public IEnumerable<MIspConsultaTipoIndicadorConNivel_Result> BuscaTodosConNivel()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MIspConsultaTipoIndicadorConNivel().ToList();
            }
        }
    }
}
