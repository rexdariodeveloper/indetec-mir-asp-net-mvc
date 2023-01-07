using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RequisicionesAlmacenBL.Services.SAACG
{
    public class PolizaService : BaseService<tblPoliza>
    {
        public override bool Actualiza(tblPoliza entidad)
        {
            throw new NotImplementedException();
        }

        public override tblPoliza BuscaPorId(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override tblPoliza Inserta(tblPoliza entidad)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<tblPoliza> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.tblPoliza.AsEnumerable().Select(m => new tblPoliza {
                    PolizaId = m.PolizaId,
                    Ejercicio = m.Ejercicio,
                    Poliza = m.Poliza
                }).ToList().OrderBy(m => m.Poliza);
            }
        }
    }
}