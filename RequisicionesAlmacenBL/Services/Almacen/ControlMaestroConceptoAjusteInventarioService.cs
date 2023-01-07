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
    public class ControlMaestroConceptoAjusteInventarioService : BaseService<ARtblControlMaestroConceptoAjusteInventario>
    {
        public override ARtblControlMaestroConceptoAjusteInventario Inserta(ARtblControlMaestroConceptoAjusteInventario entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad el Context
                ARtblControlMaestroConceptoAjusteInventario controlMaestroConceptoAjusteInventario = Context.ARtblControlMaestroConceptoAjusteInventario.Add(entidad);

                //Guardamos cambios
                Context.SaveChanges();

                //Retornamos la entidad que se acaba de guardar en la Base de Datos
                return controlMaestroConceptoAjusteInventario;
            }
        }

        public override bool Actualiza(ARtblControlMaestroConceptoAjusteInventario entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad que vamos a actualizar al Context
                Context.ARtblControlMaestroConceptoAjusteInventario.Add(entidad);
                Context.Entry(entidad).State = EntityState.Modified;

                //Marcar todas las propiedades que no se pueden actualizar como FALSE
                //para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in ARtblControlMaestroConceptoAjusteInventario.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                //Retornamos true o false si se realizo correctamente la operacion
                return Context.SaveChanges() > 0;
            }            
        }

        public override ARtblControlMaestroConceptoAjusteInventario BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.ARtblControlMaestroConceptoAjusteInventario.Find(id);
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ARtblControlMaestroConceptoAjusteInventario> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Obtenemos todos los registros con activo
                return Context.ARtblControlMaestroConceptoAjusteInventario.Where(cai => cai.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO).ToList();
            }
        }

        public List<ARtblControlMaestroConceptoAjusteInventario> BuscaConceptosAjustePorTipoMovimientoId(int tipoMovimientoId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARtblControlMaestroConceptoAjusteInventario.Where(m => m.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO && m.TipoMovimientoId == tipoMovimientoId).ToList();
            }
        }

        public void GuardaCambios(List<ARtblControlMaestroConceptoAjusteInventario> listConceptos)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    foreach (ARtblControlMaestroConceptoAjusteInventario concepto in listConceptos)
                    {
                        if (concepto.ConceptoAjusteInventarioId > 0)
                        {
                            Actualiza(concepto);
                        }
                        else
                        {
                            Inserta(concepto);
                        }
                    }

                    //Hacemos el Commit
                    SAACGContextHelper.Commit();
                }
                catch (DbEntityValidationException ex)
                {
                    //Hacemos el Rollback
                    SAACGContextHelper.Rollback();

                    throw new Exception(UserExceptionHelper.GetMessage(ex));
                }
                catch (Exception ex)
                {
                    //Hacemos el Rollback
                    SAACGContextHelper.Rollback();

                    throw new Exception(UserExceptionHelper.GetMessage(ex));
                }
            }
        }
    }
}