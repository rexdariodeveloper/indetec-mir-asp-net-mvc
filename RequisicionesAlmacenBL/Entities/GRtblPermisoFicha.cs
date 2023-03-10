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
    
    public partial class GRtblPermisoFicha
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GRtblPermisoFicha()
        {
            this.GRtblUsarioPermiso = new HashSet<GRtblUsarioPermiso>();
        }
    
        public int PermisoFichaId { get; set; }
        public string Etiqueta { get; set; }
        public string Descripcion { get; set; }
        public int NodoMenuId { get; set; }
        public int EstatusId { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public int CreadoPorId { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
        public Nullable<int> ModificadoPorId { get; set; }
        public byte[] Timestamp { get; set; }
    
        public virtual GRtblControlMaestro GRtblControlMaestro { get; set; }
        public virtual GRtblMenuPrincipal GRtblMenuPrincipal { get; set; }
        public virtual GRtblUsuario GRtblUsuario { get; set; }
        public virtual GRtblUsuario GRtblUsuario1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GRtblUsarioPermiso> GRtblUsarioPermiso { get; set; }
    }
}
