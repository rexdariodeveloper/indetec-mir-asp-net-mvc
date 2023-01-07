using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using RequisicionesAlmacenBL.Models.Mapeos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace RequisicionesAlmacenBL.Services
{
    public class PlanDesarrolloEstructuraService : BaseService<MItblPlanDesarrolloEstructura>
    {
        public override bool Actualiza(MItblPlanDesarrolloEstructura entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad que vamos a actualizar al Context
                Context.MItblPlanDesarrolloEstructura.Add(entidad);

                // Marcamos la entidad como modificada
                Context.Entry(entidad).State = EntityState.Modified;

                // Marcar todas las propiedades que no se pueden actualizar como FALSE
                // para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in MItblPlanDesarrolloEstructura.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos true o false si se realizo correctamente la operacion
                return true;
            }
        }

        public override MItblPlanDesarrolloEstructura BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.MItblPlanDesarrolloEstructura.Find(id);
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override MItblPlanDesarrolloEstructura Inserta(MItblPlanDesarrolloEstructura entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad el Context
                MItblPlanDesarrolloEstructura planNacionalDesarrolloEstructura = Context.MItblPlanDesarrolloEstructura.Add(entidad);

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos la entidad que se acaba de guardar en la Base de Datos
                return planNacionalDesarrolloEstructura;
            }
        }

        public MItblPlanDesarrolloEstructura GuardaCambios(MItblPlanDesarrolloEstructura entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    // Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    // Creamos el return
                    MItblPlanDesarrolloEstructura planDesarrolloEstructura = entidad;

                    // Si es un registro nuevo
                    if (entidad.PlanDesarrolloEstructuraId <= 0)
                    {
                        planDesarrolloEstructura = Inserta(entidad);
                    }
                    else
                    {
                        Actualiza(entidad);
                    }

                    // Hacemos el Commit
                    SAACGContextHelper.Commit();

                    // Retornamos la entidad que se acaba de guardar en la Base de Datos
                    return planDesarrolloEstructura;
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

        public MItblPlanDesarrolloEstructura GuardaPlanDesarrolloEstructura(MItblPlanDesarrolloEstructura entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos los detalles al Context
                MItblPlanDesarrolloEstructura planDesarrolloEstructura = Context.MItblPlanDesarrolloEstructura.Add(entidad);

                // Si es un registro que se va actualizar
                if (planDesarrolloEstructura.PlanDesarrolloEstructuraId > 0)
                {
                    Context.Entry(planDesarrolloEstructura).State = EntityState.Modified;

                    // Marcar todas las propiedades que no se pueden actualizar como FALSE
                    // para que no se actualice su informacion en Base de Datos
                    foreach (string propertyName in MItblPlanDesarrolloEstructura.PropiedadesNoActualizables)
                    {
                        Context.Entry(planDesarrolloEstructura).Property(propertyName).IsModified = false;
                    }
                }

                // Guardamos cambios
                Context.SaveChanges();

                return planDesarrolloEstructura;
            }
        }

        public IEnumerable<MItblPlanDesarrolloEstructura> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblPlanDesarrolloEstructura.Where(pde => pde.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO).ToList();
            }
        }

        public IEnumerable<MItblPlanDesarrolloEstructura> BuscaPorPlanDesarrolloId(int planId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblPlanDesarrolloEstructura.Where(pde => pde.PlanDesarrolloId == planId && pde.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO).ToList();
            }
        }

        public IEnumerable<MIspRptLibroConsultaPlanDesarrolloEstructura_Result> BuscaReportePorPlanDesarrolloEstructuraId(int planDesarrolloEstructuraId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MIspRptLibroConsultaPlanDesarrolloEstructura(planDesarrolloEstructuraId).ToList();
            }
        }
    }
}
