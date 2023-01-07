using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RequisicionesAlmacenBL.Services
{
    public class CatalogoCuentaService : BaseService<tblCatalogoCuenta>
    {
        public override bool Actualiza(tblCatalogoCuenta entidad)
        {
            throw new NotImplementedException();
        }

        public override tblCatalogoCuenta BuscaPorId(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override tblCatalogoCuenta Inserta(tblCatalogoCuenta entidad)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<tblCatalogoCuenta> GetCatalogoCuentaWithTipoCuenta(string tipoCuenta)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.tblCatalogoCuenta.Where(cc => cc.TipoCuenta == tipoCuenta).ToList();
            }
        }
    }
}
