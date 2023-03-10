//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RequisicionesAlmacenBL.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class ARtblInventarioFisicoDetalle
    {
        public int InventarioFisicoDetalleId { get; set; }
        public int InventarioFisicoId { get; set; }
        public int AlmacenProductoId { get; set; }
        public decimal CostoPromedio { get; set; }
        public string ProyectoId { get; set; }
        public string Proyecto { get; set; }
        public string FuenteFinanciamientoId { get; set; }
        public string FuenteFinanciamiento { get; set; }
        public string UnidadAdministrativaId { get; set; }
        public string UnidadAdministrativa { get; set; }
        public string TipoGastoId { get; set; }
        public string TipoGasto { get; set; }
        public string ProductoId { get; set; }
        public string Producto { get; set; }
        public int UnidadDeMedidaId { get; set; }
        public string UnidadDeMedida { get; set; }
        public decimal Existencia { get; set; }
        public Nullable<decimal> Conteo { get; set; }
        public string MotivoAjuste { get; set; }
        public bool Borrado { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public int CreadoPorId { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
        public Nullable<int> ModificadoPorId { get; set; }
        public byte[] Timestamp { get; set; }
    
        public virtual ARtblAlmacenProducto ARtblAlmacenProducto { get; set; }
        public virtual GRtblUsuario GRtblUsuario { get; set; }
        public virtual GRtblUsuario GRtblUsuario1 { get; set; }
        public virtual tblRamo tblRamo { get; set; }
        public virtual tblProducto tblProducto { get; set; }
        public virtual tblUnidadDeMedida tblUnidadDeMedida { get; set; }
        public virtual tblTipoGasto tblTipoGasto { get; set; }
        public virtual tblDependencia tblDependencia { get; set; }
        public virtual ARtblInventarioFisico ARtblInventarioFisico { get; set; }
    }
}
