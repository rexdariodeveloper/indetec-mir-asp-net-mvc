using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using FastReport.Web;
using RequisicionesAlmacen.Helpers;

namespace RequisicionesAlmacen.Areas.Compras.Requisiciones.Controllers
{
    public class RptLibroAlmacenController : Controller
    {
        // GET: Compras/RptLibroAlmacen
        public ActionResult RptLibroAlmacenPorArticulo()
        {
            ReportHelper reportHelper = new ReportHelper();

            Dictionary<string, object> parametros = new Dictionary<string, object>();
            parametros.Add("@pEnte", "El ente de aqui de Jalisco");
            parametros.Add("@pAlmacen", "Almacen General");
            parametros.Add("@pUsuario", "Alonso Soto");
            parametros.Add("@pAlmacenId", "0");
            parametros.Add("@pEstado", "Jalisco");
            
            WebReport webReport = reportHelper.GetWebReport("Almacen/InventarioFisico/ARrptInventarioFisico_por_articulo.frx", parametros);
            ViewBag.WebReport = webReport;
            return View("rptLibroAlmacen");
        }

        public ActionResult RptLibroAlmacenPorClave()
        {
            ReportHelper reportHelper = new ReportHelper();

            Dictionary<string, object> parametros = new Dictionary<string, object>();
            parametros.Add("@pEnte", "El ente de aqui de Jalisco");
            parametros.Add("@pAlmacen", "Almacen General");
            parametros.Add("@pUsuario", "Alonso Soto");
            parametros.Add("@pAlmacenId", "0");
            parametros.Add("@pEstado", "Jalisco");

            WebReport webReport = reportHelper.GetWebReport("Almacen/InventarioFisico/ARrptInventarioFisico_por_clave.frx", parametros);
            ViewBag.WebReport = webReport;
            return View("rptLibroAlmacen");
        }
    }
}