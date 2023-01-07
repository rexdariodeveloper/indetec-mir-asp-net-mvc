using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using static RequisicionesAlmacenBL.Models.Mapeos.ControlMaestroMapeo;

namespace RequisicionesAlmacenBL.Services
{
    public class InventarioAjusteService : BaseService<ARtblInventarioAjuste>
    {
        public override ARtblInventarioAjuste Inserta(ARtblInventarioAjuste entidad)
        {          
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Asignamos el autonumerico
                entidad.CodigoAjusteInventario = new AutonumericoService().GetSiguienteAutonumerico("Ajuste de Inventario");

                //Agregamos la entidad el Context
                ARtblInventarioAjuste inventarioAjuste = Context.ARtblInventarioAjuste.Add(entidad);

                //Guardamos cambios
                Context.SaveChanges();

                //Retornamos el objeto registrado
                return inventarioAjuste;
            }
        }

        public override bool Actualiza(ARtblInventarioAjuste entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad el Context
                Context.ARtblInventarioAjuste.Add(entidad);

                //Marcamos la entidad como modificada
                Context.Entry(entidad).State = EntityState.Modified;

                //Marcar todas las propiedades que no se pueden actualizar como FALSE
                //para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in ARtblInventarioAjuste.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                //Guardamos cambios
                Context.SaveChanges();
                
                //Retornamos true o false si se realizo correctamente la operacion
                return true;
            }
        }

        public ARtblInventarioAjusteDetalle GuardaDetalle(ARtblInventarioAjusteDetalle entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos los detalles al Context
                ARtblInventarioAjusteDetalle detalle = Context.ARtblInventarioAjusteDetalle.Add(entidad);

                //Si es un registro que se va actualizar
                if (detalle.InventarioAjusteDetalleId > 0)
                {
                    Context.Entry(detalle).State = EntityState.Modified;

                    //Marcar todas las propiedades que no se pueden actualizar como FALSE
                    //para que no se actualice su informacion en Base de Datos
                    foreach (string propertyName in ARtblInventarioAjusteDetalle.PropiedadesNoActualizables)
                    {
                        Context.Entry(detalle).Property(propertyName).IsModified = false;
                    }
                }

                //Guardamos cambios
                Context.SaveChanges();

                return detalle;
            }
        }

        public void GuardaDetalles(int inventarioAjusteId, List<ARtblInventarioAjusteDetalle> detalles)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                foreach (ARtblInventarioAjusteDetalle detalleTemp in detalles)
                {
                    //Asignamos el Id de la cabecera
                    detalleTemp.InventarioAjusteId = inventarioAjusteId;

                    //Guardamos el detalle
                    ARtblInventarioAjusteDetalle detalle = GuardaDetalle(detalleTemp);

                    detalle.NombreArchivoTmp = detalleTemp.NombreArchivoTmp;

                    if (detalle.NombreArchivoTmp != null)
                    {
                        GuardaArchivo(detalle);
                    }
                }
            }
        }

        public int GuardaCambios(ARtblInventarioAjuste entidad, List<ARtblInventarioAjusteDetalle> detalles)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    //Creamos el return
                    int inventarioAjusteId = entidad.InventarioAjusteId;

                    //Si es un registro nuevo
                    if (entidad.InventarioAjusteId == 0)
                    {
                        inventarioAjusteId = Inserta(entidad).InventarioAjusteId;
                    }
                    else
                    {
                        Actualiza(entidad);
                    }

                    //Guardamos los detalles
                    GuardaDetalles(inventarioAjusteId, detalles);

                    //Afectamos el inventario
                    AfectaInventario(inventarioAjusteId, entidad.CreadoPorId);

                    //Hacemos el Commit
                    SAACGContextHelper.Commit();

                    //Retornamos la entidad que se acaba de guardar en la Base de Datos
                    return inventarioAjusteId;
                }
                catch (DbEntityValidationException ex)
                {
                    //Hacemos el Rollback
                    SAACGContextHelper.Rollback();

                    throw new Exception(UserExceptionHelper.GetMessage(ex));
                }
                catch (Exception ex)
                {
                    //Hacemos el Rollback
                    SAACGContextHelper.Rollback();

                    throw new Exception(UserExceptionHelper.GetMessage(ex));
                }
            }
        }

        public void AfectaInventario(int inventarioId, int usuarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                Context.ARspAfectaInventarioAjuste(inventarioId, usuarioId);
            }
        }

        private void GuardaArchivo(ARtblInventarioAjusteDetalle inventarioAjusteDetalle)
        {
            ArchivoService archivoService = new ArchivoService();

            string nombreArchivoTemporal = inventarioAjusteDetalle.NombreArchivoTmp;
            string extensionArchivo = nombreArchivoTemporal.Substring(nombreArchivoTemporal.LastIndexOf("."));
            int tipoArchivo = archivoService.ObtenerTipoArchivo(extensionArchivo);            
            int id = inventarioAjusteDetalle.InventarioAjusteId;

            Guid archivoId = archivoService.GuardaArchivo(inventarioAjusteDetalle.CreadoPorId,
                                                            null,
                                                            nombreArchivoTemporal,
                                                            null,
                                                            ListadoCMOA.EVIDENCIA_AJUSTE_INVENTARIO,
                                                            new List<string>() { id.ToString() },
                                                            id,
                                                            tipoArchivo);

            inventarioAjusteDetalle.ArchivoId = archivoId;

            GuardaDetalle(inventarioAjusteDetalle);
        }        

        public override ARtblInventarioAjuste BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.ARtblInventarioAjuste.Where(m => m.InventarioAjusteId == id).FirstOrDefault();
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public List<ARvwListadoInventarioAjuste> BuscaListado()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARvwListadoInventarioAjuste.ToList();
            }
        }

        public List<ARspConsultaExistenciaAlmacen_Result> ConsultaExistenciaAlmacen(string almacenId, string productosIds)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaExistenciaAlmacen(almacenId, productosIds).ToList();
            }
        }

        public List<ARspConsultaInventarioAjusteDetalles_Result> BuscaDetallesPorInventarioAjusteId(int inventarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaInventarioAjusteDetalles(inventarioId).ToList();
            }
        }

        public ARtblInventarioAjusteDetalle BuscaDetallePorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.ARtblInventarioAjusteDetalle.Find(id);
            }
        }
    }
}