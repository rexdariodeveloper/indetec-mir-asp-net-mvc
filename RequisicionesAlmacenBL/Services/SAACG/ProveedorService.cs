using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RequisicionesAlmacenBL.Services.SAACG
{
    public class ProveedorService : BaseService<tblProveedor>
    {
        public override bool Actualiza(tblProveedor entidad)
        {
            throw new NotImplementedException();
        }

        public override tblProveedor BuscaPorId(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override tblProveedor Inserta(tblProveedor entidad)
        {
            throw new NotImplementedException();
        }

        public List<tblProveedor> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.tblProveedor.AsEnumerable().Select(m => new tblProveedor
                {
                    ProveedorId = m.ProveedorId,
                    TipoProveedorId = m.TipoProveedorId,
                    RazonSocial = m.RazonSocial,
                    RFC = m.RFC,
                    Status = m.Status,
                    TipoOperacionId = m.TipoOperacionId,
                    TipoComprobanteFiscalId = m.TipoComprobanteFiscalId
                }).Where(m => m.Status == "A").OrderBy(m => m.RazonSocial).ToList();
            }
        }
    }
}
