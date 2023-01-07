using EntityFrameworkExtras.EF6;
using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RequisicionesAlmacenBL.Models.Mapeos.ControlMaestroMapeo;

namespace RequisicionesAlmacenBL.Services
{
    public class InventarioFisicoService : BaseService<ARtblInventarioFisico>
    {
        public override ARtblInventarioFisico Inserta(ARtblInventarioFisico entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Asignamos el autonumerico
                entidad.Codigo = new AutonumericoService().GetSiguienteAutonumerico("Inventarios Físicos");

                //Agregamos la entidad el Context
                ARtblInventarioFisico inventarioFisico = Context.ARtblInventarioFisico.Add(entidad);

                //Guardamos cambios
                Context.SaveChanges();

                //Retornamos el objeto registrado
                return inventarioFisico;
            }
        }

        public override bool Actualiza(ARtblInventarioFisico entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad el Context
                ARtblInventarioFisico inventarioFisico = Context.ARtblInventarioFisico.Add(entidad);

                //Marcamos la entidad como modificada
                Context.Entry(entidad).State = EntityState.Modified;

                //Marcar todas las propiedades que no se pueden actualizar como FALSE
                //para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in ARtblInventarioFisico.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                //Guardamos cambios
                Context.SaveChanges();

                //Retornamos true o false si se realizo correctamente la operacion
                return true;
            }
        }

        public void GuardaDetalles(int inventarioFisicoId, List<ARtblInventarioFisicoDetalle> detalles)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                foreach (ARtblInventarioFisicoDetalle detalle in detalles)
                {
                    //Asignamos el Id de la cabecera
                    detalle.InventarioFisicoId = inventarioFisicoId;

                    //Agregamos los detalles al Context
                    Context.ARtblInventarioFisicoDetalle.Add(detalle);

                    //Si es un registro que se va actualizar
                    if (detalle.InventarioFisicoDetalleId > 0)
                    {
                        Context.Entry(detalle).State = EntityState.Modified;

                        //Marcar todas las propiedades que no se pueden actualizar como FALSE
                        //para que no se actualice su informacion en Base de Datos
                        foreach (string propertyName in ARtblInventarioFisicoDetalle.PropiedadesNoActualizables)
                        {
                            Context.Entry(detalle).Property(propertyName).IsModified = false;
                        }
                    }
                }

                //Guardamos cambios
                Context.SaveChanges();
            }
        }

        public int GuardaCambios(ARtblInventarioFisico entidad, List<ARtblInventarioFisicoDetalle> detalles)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    //Creamos el return
                    int inventarioFisicoId = entidad.InventarioFisicoId;

                    //Si es un registro nuevo
                    if (entidad.InventarioFisicoId == 0)
                    {
                        inventarioFisicoId = Inserta(entidad).InventarioFisicoId;
                    }
                    else
                    {
                        Actualiza(entidad);
                    }

                    //Guardamos los detalles
                    GuardaDetalles(inventarioFisicoId, detalles);

                    //Hacemos el Commit
                    SAACGContextHelper.Commit();

                    //Retornamos la entidad que se acaba de guardar en la Base de Datos
                    return inventarioFisicoId;
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

        public override ARtblInventarioFisico BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.ARtblInventarioFisico.Where(m => 
                    m.InventarioFisicoId == id && 
                    (m.EstatusId == EstatusInventarioFisico.EN_PROCESO 
                        || m.EstatusId == EstatusInventarioFisico.TERMINADO)
                    ).FirstOrDefault();
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public void Elimina(ARtblInventarioFisico entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    Actualiza(entidad);

                    //Hacemos el Commit
                    SAACGContextHelper.Commit();
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

        public List<ARvwListadoInventarioFisico> BuscaListado()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARvwListadoInventarioFisico.ToList();
            }
        }

        public List<ARspConsultaExistenciaAlmacen_Result> ConsultaExistenciaAlmacen(string almacenId, string productosIds)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaExistenciaAlmacen(almacenId, productosIds).ToList();
            }
        }

        public List<ARspConsultaInventarioFisicoDetalles_Result> BuscaDetallesPorInventarioFisicoId(int inventarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaInventarioFisicoDetalles(inventarioId).ToList();
            }
        }

        public ARtblInventarioFisicoDetalle BuscaDetallePorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.ARtblInventarioFisicoDetalle.Find(id);
            }
        }

        public bool ExisteInventarioIniciado(string almacenId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARtblInventarioFisico.FirstOrDefault(m => 
                    m.AlmacenId == almacenId && 
                    m.EstatusId == EstatusInventarioFisico.EN_PROCESO) != null;
            }
        }

        public void AfectaInventario(int inventarioId, int usuarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    Context.ARspAfectaInventarioFisico(inventarioId, usuarioId);

                    //Hacemos el Commit
                    SAACGContextHelper.Commit();
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

        public void CargaInventarioInicial(List<ARudtImportarAlmacenProducto> importarAlmacenProducto, int usuarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    //Cargamos el inventario
                    var procedure = new ARspCargaInventarioInicial()
                    {
                        ImportarAlmacenProducto = importarAlmacenProducto,
                        UsuarioId = usuarioId
                    };

                    //Ejecutamos el procedure
                    Context.Database.ExecuteStoredProcedure(procedure);

                    //Hacemos el Commit
                    SAACGContextHelper.Commit();
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

        public string InventarioInicialValidarFila(string productoId, 
                                                   string almacenId, 
                                                   string fuenteFinanciamientoId, 
                                                   string proyectoId, 
                                                   string unidadAdministrativaId, 
                                                   string tipoGastoId, 
                                                   Nullable<decimal> cantidad, 
                                                   Nullable<decimal> costoUnitario)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspCargaInventarioInicialValidarFila(productoId,
                                                                     almacenId,
                                                                     fuenteFinanciamientoId,
                                                                     proyectoId,
                                                                     unidadAdministrativaId,
                                                                     tipoGastoId,
                                                                     cantidad,
                                                                     costoUnitario).FirstOrDefault();
            }
        }
    }
}