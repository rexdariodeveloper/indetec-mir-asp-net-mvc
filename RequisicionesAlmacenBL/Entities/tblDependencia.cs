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
    
    public partial class tblDependencia
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblDependencia()
        {
            this.ARtblControlMaestroConfiguracionArea = new HashSet<ARtblControlMaestroConfiguracionArea>();
            this.ARtblInventarioFisicoDetalle = new HashSet<ARtblInventarioFisicoDetalle>();
            this.ARtblRequisicionMaterial = new HashSet<ARtblRequisicionMaterial>();
            this.ARtblRequisicionMaterial1 = new HashSet<ARtblRequisicionMaterial>();
            this.ARtblRequisicionMaterialDetalle = new HashSet<ARtblRequisicionMaterialDetalle>();
            this.tblCuentaPresupuestalEgr = new HashSet<tblCuentaPresupuestalEgr>();
            this.tblProyecto_Dependencia = new HashSet<tblProyecto_Dependencia>();
            this.RHtblEmpleado = new HashSet<RHtblEmpleado>();
            this.ARtblInventarioAjusteDetalle = new HashSet<ARtblInventarioAjusteDetalle>();
        }
    
        public string DependenciaId { get; set; }
        public string Nombre { get; set; }
        public bool CuentaDeRegistro { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARtblControlMaestroConfiguracionArea> ARtblControlMaestroConfiguracionArea { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARtblInventarioFisicoDetalle> ARtblInventarioFisicoDetalle { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARtblRequisicionMaterial> ARtblRequisicionMaterial { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARtblRequisicionMaterial> ARtblRequisicionMaterial1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARtblRequisicionMaterialDetalle> ARtblRequisicionMaterialDetalle { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCuentaPresupuestalEgr> tblCuentaPresupuestalEgr { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblProyecto_Dependencia> tblProyecto_Dependencia { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RHtblEmpleado> RHtblEmpleado { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARtblInventarioAjusteDetalle> ARtblInventarioAjusteDetalle { get; set; }
    }
}
