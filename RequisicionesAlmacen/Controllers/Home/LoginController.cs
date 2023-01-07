﻿using RequisicionesAlmacen.Helpers;
using RequisicionesAlmacenBL.Models;
using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Services;
using SACG.GDALib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;
using System.Web.Mvc;
using SACG.sysSacg.Services;
using SACG.sysSacg.Entities;
using RequisicionesAlmacen.Models.ViewModel;

namespace RequisicionesAlmacen.Controllers.Home
{
    [NoneAuthenticated]
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            //Cargamos las entidades
            IEnumerable<Entidad> listEntidades = new EntidadService().ObtenerEntidades();

            //Creamos el ViewModel
            UsuarioSesionViewModel usuarioSesionViewModel = new UsuarioSesionViewModel();

            //Seteamos el usuario
            usuarioSesionViewModel.UsuarioSesion = new UsuarioSesion();

            //Seteamos las Entidades
            usuarioSesionViewModel.ListEntes = listEntidades;
            usuarioSesionViewModel.ListEntes.ToList().ForEach(e => e.Password = null);

            //Retornamos el objeto Usuario
            return View(usuarioSesionViewModel);
        }

        [HttpPost]
        [JsonException]
        public JsonResult IniciarSesion(UsuarioSesion usuario)
        {
            try
            {
                //Validamos que el usuario exista
                GRtblUsuario usuarioT = new UsuarioService().ValidaUsuarioLogin(usuario.NombreUsuario, SACG.GDALib.cSecurity.EncryptINI(usuario.Contrasenia), usuario.EnteId);

                if (usuarioT != null)
                {
                    //Creamos un objeto de tipo UsuarioSesion
                    UsuarioSesion usuarioSesion = new UsuarioSesion();
                    usuarioSesion.UsuarioId = usuarioT.UsuarioId;
                    usuarioSesion.NombreUsuario = usuarioT.NombreUsuario;                    
                    usuarioSesion.EnteId = usuario.EnteId;
                    usuarioSesion.Ejercicio = usuario.Ejercicio;
                    // Obtenemos los menus con permiso de usuario por el ROL
                    IEnumerable<GRspConsultaMenuPrincipalPermiso_Result> listaMenuPrinicpal = new MenuPrincipalService().BuscaTodosPorRolLoginId(Convert.ToInt32(usuarioT.RolId), usuario.EnteId);
                    List<int> listaNodoMenuId = new List<int>();
                    // Asignamos el ID de Menu por HOME (0)
                    listaNodoMenuId.Add(0);
                    listaMenuPrinicpal.ToList().ForEach(mp => listaNodoMenuId.Add(mp.NodoMenuId));

                    if (SessionHelper.ExisteSesion())
                        SessionHelper.CierraSesion();
                    else
                        SessionHelper.CreaSesion(usuarioSesion, listaNodoMenuId);

                    return Json(new { listaMenuPrinicpal = JsonSerializer.Serialize(listaMenuPrinicpal), usuario = usuarioSesion.NombreUsuario });
                }
                else
                {
                    throw new Exception("No se puede iniciar Sesion");
                }
            }
            catch(Exception ex)
            {
                 return new ExceptionHelper(ex.Message);
                //throw new Exception(ex.Message);
                //throw new Exception(Json(new JsonResponseException(ex.Message, 500)).Data.ToString());
                //throw new JsonException(JsonSerializer.Serialize(new JsonResponseException(ex.Message, 500)));
            }

        }
    }
}