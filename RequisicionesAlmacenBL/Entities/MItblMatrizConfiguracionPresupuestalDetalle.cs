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
    
    public partial class MItblMatrizConfiguracionPresupuestalDetalle
    {
        public int ConfiguracionPresupuestoDetalleId { get; set; }
        public int ConfiguracionPresupuestoId { get; set; }
        public int ClasificadorId { get; set; }
        public int MIRIndicadorId { get; set; }
        public Nullable<decimal> Enero { get; set; }
        public Nullable<decimal> Febrero { get; set; }
        public Nullable<decimal> Marzo { get; set; }
        public Nullable<decimal> Abril { get; set; }
        public Nullable<decimal> Mayo { get; set; }
        public Nullable<decimal> Junio { get; set; }
        public Nullable<decimal> Julio { get; set; }
        public Nullable<decimal> Agosto { get; set; }
        public Nullable<decimal> Septiembre { get; set; }
        public Nullable<decimal> Octubre { get; set; }
        public Nullable<decimal> Noviembre { get; set; }
        public Nullable<decimal> Diciembre { get; set; }
        public Nullable<decimal> Anual { get; set; }
        public decimal Porcentaje { get; set; }
        public int EstatusId { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public int CreadoPorId { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
        public Nullable<int> ModificadoPorId { get; set; }
        public byte[] Timestamp { get; set; }
    
        private GRtblControlMaestro GRtblControlMaestro { get; set; }
        private GRtblControlMaestro GRtblControlMaestro1 { get; set; }
        private GRtblUsuario GRtblUsuario { get; set; }
        private GRtblUsuario GRtblUsuario1 { get; set; }
        private MItblMatrizConfiguracionPresupuestal MItblMatrizConfiguracionPresupuestal { get; set; }
        private MItblMatrizIndicadorResultadoIndicador MItblMatrizIndicadorResultadoIndicador { get; set; }
    }
}
