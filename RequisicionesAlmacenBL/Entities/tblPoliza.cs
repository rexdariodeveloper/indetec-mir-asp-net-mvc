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
    
    public partial class tblPoliza
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPoliza()
        {
            this.ARtblInventarioMovimientoAgrupador = new HashSet<ARtblInventarioMovimientoAgrupador>();
        }
    
        public int PolizaId { get; set; }
        public int Ejercicio { get; set; }
        public string Poliza { get; set; }
        public System.DateTime Fecha { get; set; }
        public string Status { get; set; }
        public string Concepto { get; set; }
        public string NumeroCheque { get; set; }
        public string Beneficiario { get; set; }
        public Nullable<int> CantidadRenglones { get; set; }
        public Nullable<decimal> SumaTotal { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARtblInventarioMovimientoAgrupador> ARtblInventarioMovimientoAgrupador { get; set; }
    }
}
