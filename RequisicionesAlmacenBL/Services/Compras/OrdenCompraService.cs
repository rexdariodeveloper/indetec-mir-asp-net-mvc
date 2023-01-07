using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using RequisicionesAlmacenBL.Services.Sistema;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace RequisicionesAlmacenBL.Services.Compras
{
    public class OrdenCompraService : BaseService<tblOrdenCompra>
    {
        public override tblOrdenCompra BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.tblOrdenCompra.Where(m => m.OrdenCompraId == id).FirstOrDefault();
            }
        }

        public List<tblOrdenCompra> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.tblOrdenCompra.ToList();
            }
        }

        public List<ARvwListadoOrdenCompra> BuscaListado()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARvwListadoOrdenCompra.ToList();
            }
        }

        public override tblOrdenCompra Inserta(tblOrdenCompra entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad el Context
                tblOrdenCompra ordenCompra = Context.tblOrdenCompra.Add(entidad);

                //Validamos que exista un periodo abierto para la OC
                if (!new SistemaService().RevisarPeriodoAbierto(entidad.Fecha.Year, entidad.Fecha.Month, "P"))
                {
                    throw new Exception("No existe un periodo abierto para la OC. Favor de revisar.");
                }

                //Guardamos cambios
                Context.SaveChanges();

                //Retornamos si se guardó correctamente
                return ordenCompra;
            }
        }

        public override bool Actualiza(tblOrdenCompra entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad el Context
                tblOrdenCompra ordenCompra = Context.tblOrdenCompra.Add(entidad);

                //Validamos que exista un periodo abierto para la OC
                if (!new SistemaService().RevisarPeriodoAbierto(entidad.Fecha.Year, entidad.Fecha.Month, "P"))
                {
                    throw new Exception("No existe un periodo abierto para la OC. Favor de revisar.");
                }

                //Marcamos el modelo como modificado
                Context.Entry(entidad).State = EntityState.Modified;

                //Marcar todas las propiedades que no se pueden actualizar como FALSE
                //para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in tblOrdenCompra.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                //Guardamos cambios
                return Context.SaveChanges() > 0;
            }
        }

        public void GuardaDetalles(int ordenCompraId, List<tblOrdenCompraDet> detalles)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                foreach (tblOrdenCompraDet detalle in detalles)
                {
                    //Asignamos el Id de la cabecera
                    detalle.OrdenCompraId = ordenCompraId;

                    //Agregamos los detalles al Context
                    Context.tblOrdenCompraDet.Add(detalle);

                    if (detalle.OrdenCompraDetId > 0)
                    {
                        Context.Entry(detalle).State = EntityState.Modified;

                        //Marcar todas las propiedades que no se pueden actualizar como FALSE
                        //para que no se actualice su informacion en Base de Datos
                        foreach (string propertyName in tblOrdenCompraDet.PropiedadesNoActualizables)
                        {
                            Context.Entry(detalle).Property(propertyName).IsModified = false;
                        }
                    }
                }

                //Guardamos cambios
                Context.SaveChanges();
            }
        }

        public int GuardaCambios(tblOrdenCompra entidad, List<tblOrdenCompraDet> detalles)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    //Creamos el Id de la cabecera
                    int ordenCompraId = entidad.OrdenCompraId;

                    //Validamos si es un registro nuevo
                    if (entidad.OrdenCompraId == 0)
                    {
                        ordenCompraId = Inserta(entidad).OrdenCompraId;
                    }
                    else
                    {
                        Actualiza(entidad);
                    }

                    //Guardamos cambios
                    Context.SaveChanges();

                    //Guardamos los detalles
                    if (detalles != null)
                    {
                        GuardaDetalles(ordenCompraId, detalles);
                    }

                    //Actualizamos el estatus de la OC
                    Context.ARspActualizaEstatusOrdenCompra(ordenCompraId);

                    //Hacemos el Commit
                    SAACGContextHelper.Commit();

                    //Retornamos la entidad que se acaba de guardar en la Base de Datos
                    return ordenCompraId;
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

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public List<ARspConsultaOrdenCompraProductos_Result> BuscaComboProductos(string almacenId, string dependenciaId, string proyectoId, string ramoId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaOrdenCompraProductos(almacenId, dependenciaId, proyectoId, ramoId).ToList();
            }
        }

        public List<ARspConsultaOrdenCompraDetalles_Result> BuscaDetallesPorOrdenCompraId(int ordenCompraId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaOrdenCompraDetalles(ordenCompraId).ToList();
            }
        }

        public List<tblOrdenCompraDet> BuscaDetallesNoCancelados(int ordenCompraId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.tblOrdenCompraDet.Where(m => m.OrdenCompraId == ordenCompraId).ToList();
            }
        }

        public string GetFechaConFormato(DateTime date)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.GRspGetFechaConFormato(date, false).FirstOrDefault();
            }
        }

        public string GetNombreCompletoEmpleado(int empleadoId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.RHspGetNombreCompletoEmpleado(empleadoId).FirstOrDefault();
            }
        }

        public ARspConsultaDatosFinanciamientoOrdenCompra_Result GetDatosFinanciamiento(int ordenCompraId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaDatosFinanciamientoOrdenCompra(ordenCompraId).FirstOrDefault();
            }
        }

        public bool ARspValidarRequisicionOC(int ordenCompraId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspValidarRequisicionOC(ordenCompraId).FirstOrDefault() != null;
            }
        }
    }
}