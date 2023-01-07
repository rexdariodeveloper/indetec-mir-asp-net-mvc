using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RequisicionesAlmacenBL.Services
{
    public class InvitacionCompraDetallePrecioProveedorService : BaseService<ArtblInvitacionCompraDetallePrecioProveedor>
    {
        public override ArtblInvitacionCompraDetallePrecioProveedor Inserta(ArtblInvitacionCompraDetallePrecioProveedor entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad el Context
                ArtblInvitacionCompraDetallePrecioProveedor modelo = Context.ArtblInvitacionCompraDetallePrecioProveedor.Add(entidad);

                //Guardamos cambios
                Context.SaveChanges();

                //Retornamos la entidad que se acaba de guardar en la Base de Datos
                return modelo;
            }
        }

        public override bool Actualiza(ArtblInvitacionCompraDetallePrecioProveedor entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad que vamos a actualizar al Context
                Context.ArtblInvitacionCompraDetallePrecioProveedor.Add(entidad);

                //Marcamos el modelo como modificado
                Context.Entry(entidad).State = EntityState.Modified;

                //Marcar todas las propiedades que no se pueden actualizar como FALSE
                //para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in ArtblInvitacionCompraDetallePrecioProveedor.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                //Guardamos cambios
                return Context.SaveChanges() > 0;
            }            
        }

        public override ArtblInvitacionCompraDetallePrecioProveedor BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.ArtblInvitacionCompraDetallePrecioProveedor.Find(id);
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public List<ARspConsultaInvitacionCompraDetallePreciosProveedores_Result> BuscaInvitacionCompraListadoPreciosProveedores(int invitacionCompraId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaInvitacionCompraDetallePreciosProveedores(invitacionCompraId).ToList();
            }
        }
    }
}