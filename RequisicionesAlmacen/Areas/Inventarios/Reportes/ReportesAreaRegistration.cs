using System.Web.Mvc;

namespace RequisicionesAlmacen.Areas.Inventarios.Reportes
{
    public class ReportesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "inventarios/reportes";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Reportes",
                "inventarios/reportes/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "RequisicionesAlmacen.Areas.Inventarios.Reportes.Controllers" }
            );
        }
    }
}