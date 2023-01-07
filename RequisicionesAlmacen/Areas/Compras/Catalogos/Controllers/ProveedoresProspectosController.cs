﻿using RequisicionesAlmacen.Areas.Compras.Catalogos.Models.ViewModel;
using RequisicionesAlmacen.Controllers;
using RequisicionesAlmacen.Helpers;
using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Models.Mapeos;
using RequisicionesAlmacenBL.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web.Mvc;
using static RequisicionesAlmacenBL.Models.Mapeos.ControlMaestroMapeo;

namespace RequisicionesAlmacen.Areas.Compras.Catalogos.Controllers
{
    [Authenticated(nodoMenuId = MenuPrincipalMapeo.ID.PROSPECTOS_PROVEEDOR)]
    public class ProveedoresProspectosController : BaseController<List<ARtblProveedorProspecto>, ProveedorProspectoViewModel>
    {
        /** Variables Estaticas **/
        int ESTATUS_NUEVO = 0;
        int ESTATUS_CARGADO = 1;
        int ESTATUS_EDITADO = 2;
        int ESTATUS_ELIMINADO = 3;

        public ActionResult Index()
        {
            ProveedorProspectoViewModel proveedorProspectoViewModel = new ProveedorProspectoViewModel();            

            GetDatosFicha(ref proveedorProspectoViewModel);

            return View("ProveedorProspecto", proveedorProspectoViewModel);
        }

        public override ActionResult Nuevo()
        {
            throw new NotImplementedException();
        }

        public override ActionResult Editar(int id)
        {
            throw new NotImplementedException();
        }

        [JsonException]
        public override JsonResult Guardar(List<ARtblProveedorProspecto> prospectosProveedor)
        {
            int usuarioId = SessionHelper.GetUsuario().UsuarioId;

            ProveedorProspectoService service = new ProveedorProspectoService();

            foreach (ARtblProveedorProspecto proveedorProspecto in prospectosProveedor)
            {
                if (proveedorProspecto.EstatusId != ESTATUS_CARGADO) // Editados, nuevos, eliminados
                {
                    if (proveedorProspecto.EstatusId != ESTATUS_ELIMINADO && service.ExisteRFCRazonSocial(proveedorProspecto.ProveedorProspectoId, proveedorProspecto.RFC, proveedorProspecto.RazonSocial))
                    {
                        throw new Exception("Ya existe un registro con el mismo RFC: " + proveedorProspecto.RFC 
                            + " y Razón Social: " + proveedorProspecto.RazonSocial
                            + ". Favor de verificar.");
                    }

                    if (proveedorProspecto.EstatusId != ESTATUS_ELIMINADO)
                    {
                        proveedorProspecto.EstatusId = EstatusRegistro.ACTIVO;
                    }

                    // Si es un registro nuevo
                    if (proveedorProspecto.ProveedorProspectoId <= 0)
                    {
                        proveedorProspecto.CreadoPorId = usuarioId;
                    }
                    // Si un registro que se editó o eliminó
                    else
                    {
                        ARtblProveedorProspecto temp = service.BuscaPorId(proveedorProspecto.ProveedorProspectoId);

                        if (!StructuralComparisons.StructuralEqualityComparer.Equals(proveedorProspecto.Timestamp, temp.Timestamp))
                        {
                            throw new Exception("El prospecto a proveedor con código [" + proveedorProspecto.CodigoProspecto + "] ha sido modificado por otro usuario. Favor de recargar la vista antes de guardar.");
                        }

                        proveedorProspecto.FechaUltimaModificacion = DateTime.Now;
                        proveedorProspecto.ModificadoPorId = usuarioId;
                    }
                }
            }

            return Json(service.Guarda(prospectosProveedor));
        }

        public override JsonResult Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        public override ActionResult Listar()
        {
            throw new NotImplementedException();
        }

        protected override void GetDatosFicha(ref ProveedorProspectoViewModel proveedorProspectoViewModel)
        {
            proveedorProspectoViewModel.ListProveedorProspecto = new ProveedorProspectoService().BuscaNoConvertidos();
            proveedorProspectoViewModel.ProveedorProspecto = new ARtblProveedorProspecto();
        }

        public JsonResult GetListadoProveedoresProspecto()
        {
            return Json(new ProveedorProspectoService().BuscaNoConvertidos().ToList());
        }
    }
}