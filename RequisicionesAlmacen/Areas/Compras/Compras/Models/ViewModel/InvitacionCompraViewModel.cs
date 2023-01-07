using RequisicionesAlmacenBL.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RequisicionesAlmacen.Areas.Compras.Compras.Models.ViewModel
{
    public class InvitacionCompraViewModel
    {
        public ARtblInvitacionCompra InvitacionCompra { get; set; }
        
        public IEnumerable<ARvwListadoInvitacionCompra> ListInvitacionCompra { get; set; }
        
        public IEnumerable<ARspConsultaInvitacionCompraDetalles_Result> ListInvitacionCompraDetalles { get; set; }

        public IEnumerable<ARspConsultaInvitacionCompraListadoProveedores_Result> ListProveedores { get; set; }
        
        public IEnumerable<ARspConsultaInvitacionCompraDetallePreciosProveedores_Result> ListPreciosProveedores { get; set; }

        public IEnumerable<ARspConsultaInvitacionCompraProveedorCotizaciones_Result> ListProveedoresCotizaciones { get; set; }
        public IEnumerable<ARspConsultaInvitacionCompraOrdenesCompra_Result> ListOrdenesCompra { get; set; }

        public IEnumerable<tblAlmacen> ListAlmacenes { get; set; }

        public IEnumerable<tblTarifaImpuesto> ListTarifasImpuesto { get; set; }

        public bool SoloLectura { get; set ; } = false;
    }
}