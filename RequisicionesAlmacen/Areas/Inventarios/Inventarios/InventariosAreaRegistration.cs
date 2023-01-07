using System.Web.Mvc;

namespace RequisicionesAlmacen.Areas.Inventarios.Inventarios
{
    public class InventariosAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "inventarios/inventarios";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Inventarios",
                "inventarios/inventarios/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "RequisicionesAlmacen.Areas.Inventarios.Inventarios.Controllers" }
            );
        }
    }
}