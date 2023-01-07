using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Data.Entity.Core.Objects;

namespace RequisicionesAlmacenBL.Services
{
    public class AutonumericoService : BaseService<GRtblAutonumerico>
    {
        public override bool Actualiza(GRtblAutonumerico entidad)
        {
            throw new NotImplementedException();
        }

        public override GRtblAutonumerico BuscaPorId(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override GRtblAutonumerico Inserta(GRtblAutonumerico entidad)
        {
            throw new NotImplementedException();
        }

        public string GetSiguienteAutonumerico(string nombreAutonumerico)
        {
            return GetSiguienteAutonumerico(nombreAutonumerico, null);
        }

        public string GetSiguienteAutonumerico(string nombreAutonumerico, Nullable<int> ejercicio)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                var valorSalida = new ObjectParameter("valorSalida", "");

                Context.GRspAutonumericoSigIncr(nombreAutonumerico, ejercicio, valorSalida);

                return valorSalida.Value.ToString();
            }
        }
    }
}