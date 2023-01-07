﻿using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RequisicionesAlmacenBL.Services.SAACG
{
    public class AlmacenService : BaseService<tblAlmacen>
    {
        public override bool Actualiza(tblAlmacen entidad)
        {
            throw new NotImplementedException();
        }

        public override tblAlmacen BuscaPorId(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override tblAlmacen Inserta(tblAlmacen entidad)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<tblAlmacen> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.tblAlmacen.ToList();
            }
        }
    }
}
