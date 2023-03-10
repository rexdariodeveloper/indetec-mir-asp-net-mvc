using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using RequisicionesAlmacenBL.Models.Mapeos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RequisicionesAlmacenBL.Services
{
    public class MatrizIndicadorResultadoIndicadorFormulaVariableService : BaseService<MItblMatrizIndicadorResultadoIndicadorFormulaVariable>
    {
        public override bool Actualiza(MItblMatrizIndicadorResultadoIndicadorFormulaVariable entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad que vamos a actualizar al Context
                Context.MItblMatrizIndicadorResultadoIndicadorFormulaVariable.Add(entidad);
                Context.Entry(entidad).State = EntityState.Modified;

                // Marcar todas las propiedades que no se pueden actualizar como FALSE
                // para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in MItblMatrizIndicadorResultadoIndicadorFormulaVariable.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos true o false si se realizo correctamente la operacion
                return true;
            }
        }

        public override MItblMatrizIndicadorResultadoIndicadorFormulaVariable BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Retornamos la entidad con el ID que se envio como parametro
                return Context.MItblMatrizIndicadorResultadoIndicadorFormulaVariable.Where(fv => fv.MIRIndicadorFormulaVariableId == id).FirstOrDefault();
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override MItblMatrizIndicadorResultadoIndicadorFormulaVariable Inserta(MItblMatrizIndicadorResultadoIndicadorFormulaVariable entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad el Context
                MItblMatrizIndicadorResultadoIndicadorFormulaVariable matrizIndicadorResultadoIndicadorFormulaVariable = Context.MItblMatrizIndicadorResultadoIndicadorFormulaVariable.Add(entidad);

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos la entidad que se acaba de guardar en la Base de Datos
                return matrizIndicadorResultadoIndicadorFormulaVariable;
            }
        }

        public IEnumerable<MItblMatrizIndicadorResultadoIndicadorFormulaVariable> BuscaPorMIRIndicadorId(int mirIndicadorId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblMatrizIndicadorResultadoIndicadorFormulaVariable.Where(mirim => mirim.MIRIndicadorId == mirIndicadorId && mirim.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO).ToList();
            }
        }

        public MItblMatrizIndicadorResultadoIndicador BuscaPorMIRIndicadorComponenteId(int? mirIndicadorComponenteId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblMatrizIndicadorResultadoIndicador.Where(miri => miri.MIRIndicadorId == mirIndicadorComponenteId && miri.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO).FirstOrDefault();
            }
        }
    }
}
