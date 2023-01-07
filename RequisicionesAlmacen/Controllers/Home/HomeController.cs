using RequisicionesAlmacen.Helpers;
using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace RequisicionesAlmacen.Controllers.Home {

    [Authenticated(nodoMenuId = 0)]
    public class HomeController : Controller {

        public ActionResult Index() {
            return View();
        }

        public ActionResult MenuPrincipal()
        {
            //IList<MenuPrincipal> menuPrincipal = SessionHelper.GetMenuPrincipal();
            try
            {
                //List<MenuPrincipal> menu = new List<MenuPrincipal>();

                return View("_MenuPrincipal");
            }
            catch (Exception ex) {

                return Content("Error");
            }
        }

        [HttpGet]
        [JsonException]
        public JsonResult GetAlert()
        {
            List<AlertaModel> listaAlerta = new List<AlertaModel>();
            AlertaModel nuevoAlerta = new AlertaModel();
            nuevoAlerta.AlertaId = 1;
            nuevoAlerta.Nombre = "Nuevo Solicitud";
            nuevoAlerta.Fecha = "6 de Enero del 2022";
            listaAlerta.Add(nuevoAlerta);

            nuevoAlerta = new AlertaModel();
            nuevoAlerta.AlertaId = 2;
            nuevoAlerta.Nombre = "Reporte MIR MIr00023";
            nuevoAlerta.Fecha = "3 de Enero del 2022";
            listaAlerta.Add(nuevoAlerta);

            nuevoAlerta = new AlertaModel();
            nuevoAlerta.AlertaId = 3;
            nuevoAlerta.Nombre = "Modificar ROL";
            nuevoAlerta.Fecha = "2 de Enero del 2022";
            listaAlerta.Add(nuevoAlerta);

            return Json(listaAlerta, JsonRequestBehavior.AllowGet);
        }
    }

    public class AlertaModel
    {
        public int AlertaId { get; set; }
        public string Nombre { get; set; }
        public string Fecha { get; set; }
    }

    //[JsonException]
    //public async Task<JsonResult> GetDataAsync()
    //{
    //    return await Json("Registro guardado con Exito!");
    //} 

}