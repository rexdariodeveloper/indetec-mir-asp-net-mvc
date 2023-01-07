using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RequisicionesAlmacenBL.Services
{
    public class ControlMaestroDimensionService : BaseService<MItblControlMaestroDimension>
    {
        public override bool Actualiza(MItblControlMaestroDimension entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                using (var ContextTransaction = Context.Database.BeginTransaction())
                {
                    try
                    {
                        // Agregamos la entidad que vamos a actualizar al Context
                        Context.MItblControlMaestroDimension.Add(entidad);
                        Context.Entry(entidad).State = EntityState.Modified;

                        // Marcar todas las propiedades que no se pueden actualizar como FALSE
                        // para que no se actualice su informacion en Base de Datos
                        foreach (string propertyName in MItblControlMaestroDimension.PropiedadesNoActualizables)
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

        public override MItblControlMaestroDimension BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.MItblControlMaestroDimension.Find(id);
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override MItblControlMaestroDimension Inserta(MItblControlMaestroDimension entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                using (var ContextTransaction = Context.Database.BeginTransaction())
                {
                    try
                    {
                        // Agregamos la entidad el Context
                        MItblControlMaestroDimension controlMaestroDimension = Context.MItblControlMaestroDimension.Add(entidad);

                        // Guardamos cambios
                        Context.SaveChanges();

                        // Hacemos el Commit
                        ContextTransaction.Commit();

                        // Retornamos la entidad que se acaba de guardar en la Base de Datos
                        return controlMaestroDimension;
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

        public IEnumerable<MItblControlMaestroDimension> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblControlMaestroDimension.Where(d => d.Borrado == false).ToList();
            }
        }

        public Boolean EsDescripcionExiste(MItblControlMaestroDimension controlMaestroDimension)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblControlMaestroDimension.Any(d => d.Descripcion == controlMaestroDimension.Descripcion && d.DimensionId != controlMaestroDimension.DimensionId && d.Borrado == false);
            }
        }

        public IEnumerable<MIspConsultaDimensionConNivel_Result> BuscaTodosConNivel()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MIspConsultaDimensionConNivel().ToList();
            }
        }
    }
}
