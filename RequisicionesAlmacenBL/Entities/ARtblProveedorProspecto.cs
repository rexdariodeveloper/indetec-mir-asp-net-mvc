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
    
    public partial class ARtblProveedorProspecto
    {
        public int ProveedorProspectoId { get; set; }
        public string CodigoProspecto { get; set; }
        public string RFC { get; set; }
        public string RazonSocial { get; set; }
        public string NombreContacto { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Telefono { get; set; }
        public string Extension { get; set; }
        public string CorreoElectronico { get; set; }
        public string Comentarios { get; set; }
        public bool Convertido { get; set; }
        public int EstatusId { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public int CreadoPorId { get; set; }
        public Nullable<System.DateTime> FechaUltimaModificacion { get; set; }
        public Nullable<int> ModificadoPorId { get; set; }
        public byte[] Timestamp { get; set; }
    
        public virtual GRtblControlMaestro GRtblControlMaestro { get; set; }
        public virtual GRtblUsuario GRtblUsuario { get; set; }
        public virtual GRtblUsuario GRtblUsuario1 { get; set; }
    }
}
