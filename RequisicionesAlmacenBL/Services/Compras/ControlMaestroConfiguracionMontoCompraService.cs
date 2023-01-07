using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Models.Mapeos;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace RequisicionesAlmacenBL.Services
{
    public class ControlMaestroConfiguracionMontoCompraService : BaseService<ARtblControlMaestroConfiguracionMontoCompra>
    {
        public override bool Actualiza(ARtblControlMaestroConfiguracionMontoCompra entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad que vamos a actualizar al Context
                Context.ARtblControlMaestroConfiguracionMontoCompra.Add(entidad);

                //Marcamos el modelo como modificado
                Context.Entry(entidad).State = EntityState.Modified;

                //Marcar todas las propiedades que no se pueden actualizar como FALSE
                //para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in ARtblControlMaestroConfiguracionMontoCompra.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                //Retornamos true o false si se realizo correctamente la operacion
                return Context.SaveChanges() > 0;
            }
        }

        public override ARtblControlMaestroConfiguracionMontoCompra Inserta(ARtblControlMaestroConfiguracionMontoCompra entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad el Context
                ARtblControlMaestroConfiguracionMontoCompra controlMaestroConfiguracionMontoCompra = Context.ARtblControlMaestroConfiguracionMontoCompra.Add(entidad);

                //Guardamos cambios
                Context.SaveChanges();

                //Retornamos la entidad que se acaba de guardar en la Base de Datos
                return controlMaestroConfiguracionMontoCompra;
            }
        }

        public override ARtblControlMaestroConfiguracionMontoCompra BuscaPorId(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }        

        public void GuardaCambios(List<ARtblControlMaestroConfiguracionMontoCompra> listConfiguracionMontosCompra)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    foreach (ARtblControlMaestroConfiguracionMontoCompra configuracion in listConfiguracionMontosCompra)
                    {
                        if (configuracion.ConfiguracionMontoId > 0)
                        {
                            Actualiza(configuracion);
                        }
                        else
                        {
                            Inserta(configuracion);
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

        public IEnumerable<ARtblControlMaestroConfiguracionMontoCompra> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Obtenemos todos los registros con activo
                return Context.ARtblControlMaestroConfiguracionMontoCompra.Where(cmc => cmc.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO).ToList();
            }
        }
    }
}
