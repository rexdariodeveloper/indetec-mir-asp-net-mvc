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
    
    public partial class MItblControlMaestroTipoIndicador
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MItblControlMaestroTipoIndicador()
        {
            this.MItblControlMaestroTipoIndicadorNivel = new HashSet<MItblControlMaestroTipoIndicadorNivel>();
            this.MItblMatrizIndicadorResultadoIndicador = new HashSet<MItblMatrizIndicadorResultadoIndicador>();
        }
    
        public int TipoIndicadorId { get; set; }
        public string Descripcion { get; set; }
        public bool Borrado { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public int CreadoPorId { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
        public Nullable<int> ModificadoPorId { get; set; }
        public byte[] Timestamp { get; set; }
    
        public virtual GRtblUsuario GRtblUsuario { get; set; }
        public virtual GRtblUsuario GRtblUsuario1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MItblControlMaestroTipoIndicadorNivel> MItblControlMaestroTipoIndicadorNivel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MItblMatrizIndicadorResultadoIndicador> MItblMatrizIndicadorResultadoIndicador { get; set; }
    }
}
