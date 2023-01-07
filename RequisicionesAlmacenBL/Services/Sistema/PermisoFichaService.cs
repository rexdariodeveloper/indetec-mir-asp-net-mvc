using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RequisicionesAlmacenBL.Services.Sistema
{
    public class PermisoFichaService : BaseService<GRtblPermisoFicha>
    {
        public override bool Actualiza(GRtblPermisoFicha entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad que vamos a actualizar al Context
                Context.GRtblPermisoFicha.Add(entidad);
                Context.Entry(entidad).State = EntityState.Modified;

                // Marcar todas las propiedades que no se pueden actualizar como FALSE
                // para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in GRtblPermisoFicha.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos true o false si se realizo correctamente la operacion
                return true;
            }
        }

        public override GRtblPermisoFicha BuscaPorId(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override GRtblPermisoFicha Inserta(GRtblPermisoFicha entidad)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GRspArbolPermisoFicha_Result> BuscaArbolPermisoFicha()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.GRspArbolPermisoFicha().ToList();
            }
        }
    }
}
