using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RequisicionesAlmacenBL.Services
{
    public class ControlMaestroUnidadMedidaDimensionService : BaseService<MItblControlMaestroUnidadMedidaDimension>
    {
        public override bool Actualiza(MItblControlMaestroUnidadMedidaDimension entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad que vamos a actualizar al Context
                Context.MItblControlMaestroUnidadMedidaDimension.Add(entidad);
                Context.Entry(entidad).State = EntityState.Modified;

                // Marcar todas las propiedades que no se pueden actualizar como FALSE
                // para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in MItblControlMaestroUnidadMedidaDimension.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos true o false si se realizo correctamente la operacion
                return true;
            }
        }

        public override MItblControlMaestroUnidadMedidaDimension BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.MItblControlMaestroUnidadMedidaDimension.Find(id);
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override MItblControlMaestroUnidadMedidaDimension Inserta(MItblControlMaestroUnidadMedidaDimension entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad el Context
                MItblControlMaestroUnidadMedidaDimension controlMaestroUnidadMedidaDimension = Context.MItblControlMaestroUnidadMedidaDimension.Add(entidad);

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos la entidad que se acaba de guardar en la Base de Datos
                return controlMaestroUnidadMedidaDimension;
            }
        }

        public IEnumerable<MItblControlMaestroUnidadMedidaDimension> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblControlMaestroUnidadMedidaDimension.Where(umd => umd.Borrado == false).ToList();
            }
        }

        public Boolean EsDimensionExiste(MItblControlMaestroUnidadMedidaDimension controlMaestroUnidadMedidaDimension)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblControlMaestroUnidadMedidaDimension.Any(umd => umd.DimensionId == controlMaestroUnidadMedidaDimension.DimensionId && umd.UnidadMedidaId == controlMaestroUnidadMedidaDimension.UnidadMedidaId && umd.UnidadMedidaDimensionId != controlMaestroUnidadMedidaDimension.UnidadMedidaDimensionId && umd.Borrado == false);
            }
        }
    }
}
