﻿using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using RequisicionesAlmacenBL.Models.Mapeos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RequisicionesAlmacenBL.Services
{
    public class MatrizConfiguracionPresupuestalDetalleService : BaseService<MItblMatrizConfiguracionPresupuestalDetalle>
    {
        public override bool Actualiza(MItblMatrizConfiguracionPresupuestalDetalle entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad que vamos a actualizar al Context
                Context.MItblMatrizConfiguracionPresupuestalDetalle.Add(entidad);
                Context.Entry(entidad).State = EntityState.Modified;

                // Marcar todas las propiedades que no se pueden actualizar como FALSE
                // para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in MItblMatrizConfiguracionPresupuestalDetalle.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos true o false si se realizo correctamente la operacion
                return true;
            }
        }

        public override MItblMatrizConfiguracionPresupuestalDetalle BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.MItblMatrizConfiguracionPresupuestalDetalle.Find(id);
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override MItblMatrizConfiguracionPresupuestalDetalle Inserta(MItblMatrizConfiguracionPresupuestalDetalle entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad el Context
                MItblMatrizConfiguracionPresupuestalDetalle matrizConfiguracionPresupuestalDetalle = Context.MItblMatrizConfiguracionPresupuestalDetalle.Add(entidad);

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos la entidad que se acaba de guardar en la Base de Datos
                return matrizConfiguracionPresupuestalDetalle;
            }
        }

        public IEnumerable<MItblMatrizConfiguracionPresupuestalDetalle> BuscaPorConfiguracionPresupuestoId (int configuracionPresupuestoId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblMatrizConfiguracionPresupuestalDetalle.Where(mcpd => mcpd.ConfiguracionPresupuestoId == configuracionPresupuestoId && mcpd.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO).ToList();
            }
        }

        public IEnumerable<MItblMatrizConfiguracionPresupuestalDetalle> BuscaPorMIRIndicadorId(int mirIndicadorId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblMatrizConfiguracionPresupuestalDetalle.Where(mcpd => mcpd.MIRIndicadorId == mirIndicadorId && mcpd.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO).ToList();
            }
        }

        public Boolean ExistePorMIRI(int mirIndicadorId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblMatrizConfiguracionPresupuestalDetalle.Any(mcpd => mcpd.MIRIndicadorId == mirIndicadorId && mcpd.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO);
            }
        }

        public IEnumerable<MItblMatrizConfiguracionPresupuestalDetalle> BuscaPorConfiguracionPresupuestoIdYClasificadorId(int configuracionPresupuestoId, int clasificadorId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblMatrizConfiguracionPresupuestalDetalle.Where(mcpd => mcpd.ConfiguracionPresupuestoId == configuracionPresupuestoId && mcpd.ClasificadorId == clasificadorId && mcpd.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO).ToList();
            }
        }
        public IEnumerable<MIspConsultaRepProyecto_Result> BuscaDevengado(string proyectoIds, string anio)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MIspConsultaRepProyecto(proyectoIds, anio).ToList();
            }
        }

        public IEnumerable<MIspConsultaPresupuestoVigente_Result> BuscaDetalles(int mirId, string ejercicio)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MIspConsultaPresupuestoVigente(mirId, ejercicio).ToList();
            }
        }
    }
}
