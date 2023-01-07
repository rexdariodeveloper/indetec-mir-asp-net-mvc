using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Data.Entity;

namespace RequisicionesAlmacenBL.Services
{
    public class InvitacionCompraProveedorService : BaseService<ARtblInvitacionCompraProveedor>
    {
        public override ARtblInvitacionCompraProveedor Inserta(ARtblInvitacionCompraProveedor entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad el Context
                ARtblInvitacionCompraProveedor modelo = Context.ARtblInvitacionCompraProveedor.Add(entidad);

                //Guardamos cambios
                Context.SaveChanges();

                //Guardamos las cotizaciones
                if (entidad.Cotizaciones != null)
                {
                    new InvitacionCompraProveedorCotizacionService().GuardaCambios(modelo.InvitacionCompraProveedorId, entidad.Cotizaciones);
                }

                //Retornamos la entidad que se acaba de guardar en la Base de Datos
                return modelo;
            }
        }

        public override bool Actualiza(ARtblInvitacionCompraProveedor entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad que vamos a actualizar al Context
                Context.ARtblInvitacionCompraProveedor.Add(entidad);

                //Marcamos el modelo como modificado
                Context.Entry(entidad).State = EntityState.Modified;

                //Marcar todas las propiedades que no se pueden actualizar como FALSE
                //para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in ARtblInvitacionCompraProveedor.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                //Guardamos cambios
                Context.SaveChanges();

                //Guardamos las cotizaciones
                if (entidad.Cotizaciones != null)
                {
                    new InvitacionCompraProveedorCotizacionService().GuardaCambios(entidad.InvitacionCompraProveedorId, entidad.Cotizaciones);
                }

                //Guardamos cambios
                return true;
            }            
        }

        public override ARtblInvitacionCompraProveedor BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.ARtblInvitacionCompraProveedor.Find(id);
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }
    }
}