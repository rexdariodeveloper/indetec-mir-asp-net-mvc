using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using RequisicionesAlmacenBL.Models.Mapeos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RequisicionesAlmacenBL.Services.Sistema
{
    public class RolMenuService : BaseService<GRtblRolMenu>
    {
        public override bool Actualiza(GRtblRolMenu entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad que vamos a actualizar al Context
                Context.GRtblRolMenu.Add(entidad);
                Context.Entry(entidad).State = EntityState.Modified;

                // Marcar todas las propiedades que no se pueden actualizar como FALSE
                // para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in GRtblRolMenu.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos true o false si se realizo correctamente la operacion
                return true;
            }
        }

        public override GRtblRolMenu BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.GRtblRolMenu.Where(rm => rm.RolMenuId == id).FirstOrDefault();
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override GRtblRolMenu Inserta(GRtblRolMenu entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad el Context
                GRtblRolMenu rolMenu = Context.GRtblRolMenu.Add(entidad);

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos la entidad que se acaba de guardar en la Base de Datos
                return rolMenu;
            }
        }

        public IEnumerable<GRtblRolMenu> BuscaPorRolId(int rolId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.GRtblRolMenu.Where(rm => rm.RolId == rolId && rm.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO).ToList();
            }
        }
    }
}
