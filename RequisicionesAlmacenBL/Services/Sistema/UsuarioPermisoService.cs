﻿using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using RequisicionesAlmacenBL.Models.Mapeos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RequisicionesAlmacenBL.Services.Sistema
{
    public class UsuarioPermisoService : BaseService<GRtblUsarioPermiso>
    {
        public override bool Actualiza(GRtblUsarioPermiso entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad que vamos a actualizar al Context
                Context.GRtblUsarioPermiso.Add(entidad);
                Context.Entry(entidad).State = EntityState.Modified;

                // Marcar todas las propiedades que no se pueden actualizar como FALSE
                // para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in GRtblUsarioPermiso.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos true o false si se realizo correctamente la operacion
                return true;
            }
        }

        public override GRtblUsarioPermiso BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.GRtblUsarioPermiso.Where(up => up.UsuarioPermisoId == id).FirstOrDefault();
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override GRtblUsarioPermiso Inserta(GRtblUsarioPermiso entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad el Context
                GRtblUsarioPermiso usuarioPermiso = Context.GRtblUsarioPermiso.Add(entidad);

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos la entidad que se acaba de guardar en la Base de Datos
                return usuarioPermiso;
            }
        }

        public IEnumerable<GRtblUsarioPermiso> BuscaPorUsuarioId(int usuarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.GRtblUsarioPermiso.Where(up => up.UsuarioId == usuarioId && up.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO).ToList();
            }
        }

        public Boolean BuscaUsuarioPermiso(int usuarioId, int permisoFichaId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.GRtblUsarioPermiso.Any(up => up.UsuarioId == usuarioId && up.PermisoFichaId == permisoFichaId && up.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO);
            }
        }
    }
}
