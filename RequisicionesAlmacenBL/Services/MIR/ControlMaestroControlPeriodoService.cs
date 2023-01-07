﻿using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RequisicionesAlmacenBL.Services
{
    public class ControlMaestroControlPeriodoService : BaseService<MItblControlMaestroControlPeriodo>
    {
        public override bool Actualiza(MItblControlMaestroControlPeriodo entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                using (var ContextTransaction = Context.Database.BeginTransaction())
                {
                    try
                    {
                        // Agregamos la entidad que vamos a actualizar al Context
                        Context.MItblControlMaestroControlPeriodo.Add(entidad);
                        Context.Entry(entidad).State = EntityState.Modified;

                        // Marcar todas las propiedades que no se pueden actualizar como FALSE
                        // para que no se actualice su informacion en Base de Datos
                        foreach (string propertyName in MItblControlMaestroControlPeriodo.PropiedadesNoActualizables)
                        {
                            Context.Entry(entidad).Property(propertyName).IsModified = false;
                        }

                        // Guardamos cambios
                        Context.SaveChanges();

                        // Hacemos el Commit
                        ContextTransaction.Commit();

                        // Retornamos true o false si se realizo correctamente la operacion
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Hacemos el Rollback
                        ContextTransaction.Rollback();

                        throw new Exception("Error al guardar: " + ex);
                    }
                }
            }
        }

        public override MItblControlMaestroControlPeriodo BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.MItblControlMaestroControlPeriodo.Find(id);
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override MItblControlMaestroControlPeriodo Inserta(MItblControlMaestroControlPeriodo entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                using (var ContextTransaction = Context.Database.BeginTransaction())
                {
                    try
                    {
                        // Agregamos la entidad el Context
                        MItblControlMaestroControlPeriodo controlMaestroControlPeriodo = Context.MItblControlMaestroControlPeriodo.Add(entidad);

                        // Guardamos cambios
                        Context.SaveChanges();

                        // Hacemos el Commit
                        ContextTransaction.Commit();

                        // Retornamos la entidad que se acaba de guardar en la Base de Datos
                        return controlMaestroControlPeriodo;
                    }
                    catch (Exception ex)
                    {
                        // Hacemos el Rollback
                        ContextTransaction.Rollback();

                        throw new Exception("Error al guardar: " + ex);
                    }
                }
            }
        }

        public IEnumerable<MItblControlMaestroControlPeriodo> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblControlMaestroControlPeriodo.ToList();
            }
        }
    }
}
