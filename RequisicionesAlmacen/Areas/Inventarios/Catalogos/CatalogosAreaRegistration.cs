using System.Web.Mvc;

namespace RequisicionesAlmacen.Areas.Inventarios.Catalogos
{
    public class CatalogosAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "inventarios/catalogos";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Catalogos",
                "inventarios/catalogos/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "RequisicionesAlmacen.Areas.Inventarios.Catalogos.Controllers" }
            );
        }
    }
}