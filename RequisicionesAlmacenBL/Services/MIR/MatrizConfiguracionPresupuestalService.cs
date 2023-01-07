using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Models.Mapeos;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace RequisicionesAlmacenBL.Services
{
    public class MatrizConfiguracionPresupuestalService : BaseService<MItblMatrizConfiguracionPresupuestal>
    {
        public override bool Actualiza(MItblMatrizConfiguracionPresupuestal entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad que vamos a actualizar al Context
                Context.MItblMatrizConfiguracionPresupuestal.Add(entidad);
                Context.Entry(entidad).State = EntityState.Modified;

                // Marcar todas las propiedades que no se pueden actualizar como FALSE
                // para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in MItblMatrizConfiguracionPresupuestal.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos true o false si se realizo correctamente la operacion
                return true;
            }
        }

        public override MItblMatrizConfiguracionPresupuestal BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.MItblMatrizConfiguracionPresupuestal.Find(id);
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override MItblMatrizConfiguracionPresupuestal Inserta(MItblMatrizConfiguracionPresupuestal entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad el Context
                MItblMatrizConfiguracionPresupuestal matrizConfiguracionPresupuestal = Context.MItblMatrizConfiguracionPresupuestal.Add(entidad);

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos la entidad que se acaba de guardar en la Base de Datos
                return matrizConfiguracionPresupuestal;
            }
        }

        public void GuardaCambios(MItblMatrizConfiguracionPresupuestal matrizConfiguracionPresupuestal, IEnumerable<MItblMatrizConfiguracionPresupuestalDetalle> listaMatrizConfiguracionPresupuestalDetalle)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    // Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    // Creamos el return
                    MItblMatrizConfiguracionPresupuestal _matrizConfiguracionPresupuestal = matrizConfiguracionPresupuestal;

                    if (_matrizConfiguracionPresupuestal != null)
                    {
                        // Si es un registro nuevo
                        if (_matrizConfiguracionPresupuestal.ConfiguracionPresupuestoId > 0)
                        {
                            Actualiza(_matrizConfiguracionPresupuestal);
                        }
                        else
                        {
                            _matrizConfiguracionPresupuestal = Inserta(_matrizConfiguracionPresupuestal);
                            
                        }
                    }

                    // Existe la lista para guardar o actualizar
                    if (listaMatrizConfiguracionPresupuestalDetalle != null)
                    {
                        GuardaListaMatrizConfiguracionPresupuestalDetalle(_matrizConfiguracionPresupuestal, listaMatrizConfiguracionPresupuestalDetalle);   
                    }

                    // Hacemos el Commit
                    SAACGContextHelper.Commit();
                }
                catch (DbEntityValidationException ex)
                {
                    // Hacemos el Rollback
                    SAACGContextHelper.Rollback();

                    throw new Exception(UserExceptionHelper.GetMessage(ex));
                }
                catch (Exception ex)
                {
                    // Hacemos el Rollback
                    SAACGContextHelper.Rollback();

                    throw new Exception(UserExceptionHelper.GetMessage(ex));
                }
            }
        }

        public void GuardaListaMatrizConfiguracionPresupuestalDetalle(MItblMatrizConfiguracionPresupuestal matrizConfiguracionPresupuestal, IEnumerable<MItblMatrizConfiguracionPresupuestalDetalle> listaMatrizConfiguracionPresupuestalDetalle)
        {
            
            // Service
            MatrizConfiguracionPresupuestalDetalleService matrizConfiguracionPresupuestalDetalleService = new MatrizConfiguracionPresupuestalDetalleService();

            foreach (MItblMatrizConfiguracionPresupuestalDetalle matrizConfiguracionPresupuestalDetalle in listaMatrizConfiguracionPresupuestalDetalle.ToList())
            {
                // Creamos el return
                MItblMatrizConfiguracionPresupuestalDetalle _matrizConfiguracionPresupuestalDetalle = matrizConfiguracionPresupuestalDetalle;

                if (_matrizConfiguracionPresupuestalDetalle.ConfiguracionPresupuestoDetalleId > 0)
                {
                    // Actualizamos
                    matrizConfiguracionPresupuestalDetalleService.Actualiza(_matrizConfiguracionPresupuestalDetalle);
                }
                else
                {
                    if (matrizConfiguracionPresupuestal != null)
                    {
                        if(_matrizConfiguracionPresupuestalDetalle.ConfiguracionPresupuestoId <= 0)
                        {
                            // Asignamos el Id de la cabecera
                            _matrizConfiguracionPresupuestalDetalle.ConfiguracionPresupuestoId = matrizConfiguracionPresupuestal.ConfiguracionPresupuestoId;
                        }
                    }
                    // Guardamos
                    _matrizConfiguracionPresupuestalDetalle = matrizConfiguracionPresupuestalDetalleService.Inserta(_matrizConfiguracionPresupuestalDetalle);
                }
            }
            
        }

        public IEnumerable<MIvwListadoMatrizConfiguracionPresupuestal> BuscaListado()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MIvwListadoMatrizConfiguracionPresupuestal.ToList();
            }
        }

        public MItblMatrizConfiguracionPresupuestal BuscaPorMIRId(int mirId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblMatrizConfiguracionPresupuestal.Where(mcp => mcp.MIRId == mirId && mcp.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO).FirstOrDefault();
            }
        }

        public Boolean ExistePorMIR(int mirId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MItblMatrizConfiguracionPresupuestal.Any(mcp => mcp.MIRId == mirId && mcp.EstatusId == ControlMaestroMapeo.EstatusRegistro.ACTIVO);
            }
        }

    }
}
