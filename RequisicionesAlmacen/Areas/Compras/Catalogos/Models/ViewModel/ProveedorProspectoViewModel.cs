using RequisicionesAlmacenBL.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RequisicionesAlmacen.Areas.Compras.Catalogos.Models.ViewModel
{
    public class ProveedorProspectoViewModel
    {
        public ARtblProveedorProspecto ProveedorProspecto { get; set; }

        public IEnumerable<ARtblProveedorProspecto> ListProveedorProspecto { get; set; }
    }
}