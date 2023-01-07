using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RequisicionesAlmacenBL.Services.SAACG
{
    public class ProductoService : BaseService<tblProducto>
    {
        private static string INCLUDE_UM = "UnidadDeMedida";
        private static string INCLUDE_OBJETOS_GASTO = "ObjetoGasto";

        public override bool Actualiza(tblProducto entidad)
        {
            throw new NotImplementedException();
        }

        public override tblProducto BuscaPorId(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override tblProducto Inserta(tblProducto entidad)
        {
            throw new NotImplementedException();
        }

        public List<tblProducto> BuscaActivos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                Context.Configuration.LazyLoadingEnabled = false;

                return Context.tblProducto.Where(m => m.Status.Equals("A")).ToList();
            }
        }

        public List<tblProducto> BuscaProductosPorAlmacenObjetoGasto(string almacenId, string objtoGastoId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                Context.Configuration.LazyLoadingEnabled = false;

                return Context.ARspObjetoGastoAlmacenProductos(almacenId, objtoGastoId).ToList();
            }
        }

        public List<ARspListadoProductosPorAlmacenId_Result> BuscaProductosPorAlmacenId(string almacenId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspListadoProductosPorAlmacenId(almacenId).ToList();
            }
        }

        public tblProducto BuscaArticuloPorId(string id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                Context.Configuration.LazyLoadingEnabled = false;

                return Context.tblProducto.Include(INCLUDE_UM).Where(m => m.Status.Equals("A") && m.ProductoId == id).FirstOrDefault();
            }
        }
    }
}