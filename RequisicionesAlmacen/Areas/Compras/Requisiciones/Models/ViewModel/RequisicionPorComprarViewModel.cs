using RequisicionesAlmacenBL.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RequisicionesAlmacen.Areas.Compras.Requisiciones.Models.ViewModel
{
    public class RequisicionPorComprarViewModel
    {
        public IEnumerable<ARspConsultaRequisicionPorComprarDetalles_Result> ListRequisicionMaterialDetalles { get; set; }

        public IEnumerable<tblTarifaImpuesto> ListTarifasImpuesto { get; set; }
        
        public IEnumerable<ARspConsultaRequisicionPorComprarFuentesFinanciamiento_Result> ListFuentesFinanciamiento { get; set; }

        public IEnumerable<tblProveedor> ListProveedores { get; set; }

        public IEnumerable<ARtblControlMaestroConfiguracionMontoCompra> ListMontosCompra { get; set; }

        public IEnumerable<RequisicionOrdenCompraItem> ListOrdenesCompra { get; set; }
    }
}