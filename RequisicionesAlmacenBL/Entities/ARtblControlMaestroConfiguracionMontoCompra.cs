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
    
    public partial class ARtblControlMaestroConfiguracionMontoCompra
    {
        public byte ConfiguracionMontoId { get; set; }
        public int TipoCompraId { get; set; }
        public decimal MontoMinimo { get; set; }
        public Nullable<decimal> MontoMaximo { get; set; }
        public Nullable<byte> NumeroMinimoProveedores { get; set; }
        public int EstatusId { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public int CreadoPorId { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
        public Nullable<int> ModificadoPorId { get; set; }
        public byte[] Timestamp { get; set; }
    
        public virtual GRtblControlMaestro GRtblControlMaestro { get; set; }
        public virtual GRtblControlMaestro GRtblControlMaestro1 { get; set; }
        public virtual GRtblUsuario GRtblUsuario { get; set; }
        public virtual GRtblUsuario GRtblUsuario1 { get; set; }
    }
}