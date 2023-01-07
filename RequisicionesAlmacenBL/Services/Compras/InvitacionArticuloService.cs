using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using RequisicionesAlmacenBL.Models.Mapeos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace RequisicionesAlmacenBL.Services.Compras
{
    public class InvitacionArticuloService : BaseService<ARtblInvitacionArticulo>
    {
        public override ARtblInvitacionArticulo BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARtblInvitacionArticulo.Where(m => m.InvitacionArticuloId == id).FirstOrDefault();
            }
        }

        public ARtblInvitacionArticuloDetalle BuscaDetallePorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARtblInvitacionArticuloDetalle.Where(m => m.InvitacionArticuloDetalleId == id).FirstOrDefault();
            }
        }

        public List<ARtblInvitacionArticulo> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARtblInvitacionArticulo.ToList();
            }
        }

        public override ARtblInvitacionArticulo Inserta(ARtblInvitacionArticulo entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad el Context
                ARtblInvitacionArticulo invitacionArticulo = Context.ARtblInvitacionArticulo.Add(entidad);

                //Guardamos cambios
                Context.SaveChanges();

                //Retornamos si se guardó correctamente
                return invitacionArticulo;
            }
        }

        public override bool Actualiza(ARtblInvitacionArticulo entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad el Context
                ARtblInvitacionArticulo InvitacionArticulo = Context.ARtblInvitacionArticulo.Add(entidad);

                //Marcamos el modelo como modificado
                Context.Entry(entidad).State = EntityState.Modified;

                //Marcar todas las propiedades que no se pueden actualizar como FALSE
                //para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in ARtblInvitacionArticulo.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                //Guardamos cambios
                return Context.SaveChanges() > 0;
            }
        }

        public void GuardaDetalles(int invitacionArticuloId, List<ARtblInvitacionArticuloDetalle> detalles)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                foreach (ARtblInvitacionArticuloDetalle detalle in detalles)
                {
                    //Asignamos el Id de la cabecera
                    detalle.InvitacionArticuloId = invitacionArticuloId;

                    //Agregamos los detalles al Context
                    Context.ARtblInvitacionArticuloDetalle.Add(detalle);

                    if (detalle.InvitacionArticuloDetalleId > 0)
                    {
                        Context.Entry(detalle).State = EntityState.Modified;

                        //Marcar todas las propiedades que no se pueden actualizar como FALSE
                        //para que no se actualice su informacion en Base de Datos
                        foreach (string propertyName in ARtblInvitacionArticuloDetalle.PropiedadesNoActualizables)
                        {
                            Context.Entry(detalle).Property(propertyName).IsModified = false;
                        }
                    }
                }

                //Guardamos cambios
                Context.SaveChanges();
            }
        }

        public int GuardaCambios(ARtblInvitacionArticulo entidad, List<ARtblInvitacionArticuloDetalle> detalles)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    //Creamos el Id de la cabecera
                    int invitacionArticuloId = -1;

                    if (entidad != null)
                    {
                        invitacionArticuloId = entidad.InvitacionArticuloId;

                        //Validamos si es un registro nuevo
                        if (entidad.InvitacionArticuloId == 0)
                        {
                            invitacionArticuloId = Inserta(entidad).InvitacionArticuloId;
                        }
                        else
                        {
                            Actualiza(entidad);
                        }

                        //Guardamos cambios
                        Context.SaveChanges();
                    }

                    //Guardamos los detalles
                    if (detalles != null)
                    {
                        GuardaDetalles(invitacionArticuloId, detalles);
                    }

                    //Hacemos el Commit
                    SAACGContextHelper.Commit();

                    //Retornamos la entidad que se acaba de guardar en la Base de Datos
                    return invitacionArticuloId;
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

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public List<ARtblInvitacionArticuloDetalle> BuscaDetallesNoCancelados(int invitacionArticuloId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARtblInvitacionArticuloDetalle.Where(m => m.InvitacionArticuloId == invitacionArticuloId
                    && m.EstatusId != ControlMaestroMapeo.AREstatusInvitacionArticuloDetalle.CANCELADO).ToList();
            }
        }

        public List<ARspConsultaInvitacionArticulosPorConvertir_Result> BuscaInvitacionArticulosPorConvertir()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaInvitacionArticulosPorConvertir().ToList();
            }
        }
    }
}