using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using RequisicionesAlmacenBL.Models.Mapeos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RequisicionesAlmacenBL.Services
{
    public class MatrizIndicadorResultadoIndicadorMetaService : BaseService<MItblMatrizIndicadorResultadoIndicadorMeta>
    {
        public override bool Actualiza(MItblMatrizIndicadorResultadoIndicadorMeta entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad que vamos a actualizar al Context
                Context.MItblMatrizIndicadorResultadoIndicadorMeta.Add(entidad);
                Context.Entry(entidad).State = EntityState.Modified;

                // Marcar todas las propiedades que no se pueden actualizar como FALSE
                // para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in MItblMatrizIndicadorResultadoIndicadorMeta.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos true o false si se realizo correctamente la operacion
                return true;
            }
        }

        public override MItblMatrizIndicadorResultadoIndicadorMeta BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Retornamos la entidad con el ID que se envio como parametro
                return Context.MItblMatrizIndicadorResultadoIndicadorMeta.Where(meta => meta.MIRIndicadorMetaId == id).FirstOrDefault();
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override MItblMatrizIndicadorResultadoIndicadorMeta Inserta(MItblMatrizIndicadorResultadoIndicadorMeta entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad el Context
                MItblMatrizIndicadorResultadoIndicadorMeta matrizIndicadorResultadoIndicadorMeta = Context.MItblMatrizIndicadorResultadoIndicadorMeta.Add(entidad);

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos la entidad que se acaba de guardar en la Base de Datos
                return matrizIndicadorResultadoIndicadorMeta;
            }
        }

        public IEnumerable<MItblMatrizIndicadorResultadoIndicadorMeta> BuscaPorMIRIndicadorId(int mirIndicadorId)
        {
            using(var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblMatrizIndicadorResultadoIndicadorMeta.Where(mirim => mirim.MIRIndicadorId == mirIndicadorId && mirim.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO).ToList();
            }
        }
    }
}
