using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RequisicionesAlmacenBL.Services.SAACG
{
    public class UnidadDeMedidaService : BaseService<tblUnidadDeMedida>
    {
        public override bool Actualiza(tblUnidadDeMedida entidad)
        {
            throw new NotImplementedException();
        }

        public override tblUnidadDeMedida BuscaPorId(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override tblUnidadDeMedida Inserta(tblUnidadDeMedida entidad)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<tblUnidadDeMedida> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.tblUnidadDeMedida.ToList();
            }
        }
    }
}
