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
    
    public partial class RHtblEmpleado
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RHtblEmpleado()
        {
            this.GRtblUsuario = new HashSet<GRtblUsuario>();
            this.ARtblInventarioFisico = new HashSet<ARtblInventarioFisico>();
            this.GRtblAlertaConfiguracion = new HashSet<GRtblAlertaConfiguracion>();
        }
    
        public int EmpleadoId { get; set; }
        public string NumeroEmpleado { get; set; }
        public string RFC { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public Nullable<int> TipoEmpleadoId { get; set; }
        public string AreaAdscripcionId { get; set; }
        public Nullable<int> PuestoId { get; set; }
        public Nullable<int> CargoId { get; set; }
        public string EmailInstitucional { get; set; }
        public string EmailPersonal { get; set; }
        public Nullable<System.Guid> Fotografia { get; set; }
        public bool Vigente { get; set; }
        public int EstatusId { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public Nullable<int> CreadoPorId { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
        public Nullable<int> ModificadoPorId { get; set; }
        public byte[] Timestamp { get; set; }
    
        public virtual tblDependencia tblDependencia { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GRtblUsuario> GRtblUsuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARtblInventarioFisico> ARtblInventarioFisico { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GRtblAlertaConfiguracion> GRtblAlertaConfiguracion { get; set; }
    }
}
