using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace RequisicionesAlmacenBL.Services.Compras
{
    public class RequisicionMaterialService : BaseService<ARtblRequisicionMaterial>
    {
        public override ARtblRequisicionMaterial BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARtblRequisicionMaterial.Where(m => m.RequisicionMaterialId == id).FirstOrDefault();
            }
        }

        public List<ARtblRequisicionMaterial> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARtblRequisicionMaterial.ToList();
            }
        }

        public List<ARvwListadoRequisicionMaterial> BuscaListado()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARvwListadoRequisicionMaterial.ToList();
            }
        }

        public override ARtblRequisicionMaterial Inserta(ARtblRequisicionMaterial entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Asignamos el autonumerico
                entidad.CodigoRequisicion = new AutonumericoService().GetSiguienteAutonumerico("Solicitud de Materiales y Consumibles", DateTime.Now.Year);

                //Agregamos la entidad el Context
                ARtblRequisicionMaterial requisicionMaterial = Context.ARtblRequisicionMaterial.Add(entidad);

                //Guardamos cambios
                Context.SaveChanges();

                //Retornamos si guardó correctamente
                return requisicionMaterial;
            }
        }

        public override bool Actualiza(ARtblRequisicionMaterial entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad el Context
                ARtblRequisicionMaterial requisicionMaterial = Context.ARtblRequisicionMaterial.Add(entidad);

                //Marcamos el modelo como modificado
                Context.Entry(entidad).State = EntityState.Modified;

                //Marcar todas las propiedades que no se pueden actualizar como FALSE
                //para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in ARtblRequisicionMaterial.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                //Guardamos cambios
                return Context.SaveChanges() > 0;
            }
        }

        public void GuardaDetalles(List<ARtblRequisicionMaterialDetalle> detalles)
        {
            GuardaDetalles(null, detalles);
        }

        public void GuardaDetalles(Nullable<int> requisicionId, List<ARtblRequisicionMaterialDetalle> detalles)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                foreach (ARtblRequisicionMaterialDetalle detalle in detalles)
                {
                    //Asignamos el Id de la cebecera
                    if (requisicionId != null)
                    {
                        detalle.RequisicionMaterialId = requisicionId.GetValueOrDefault();
                    }

                    //Agregamos los detalles al Context
                    Context.ARtblRequisicionMaterialDetalle.Add(detalle);

                    if (detalle.RequisicionMaterialDetalleId > 0)
                    {
                        Context.Entry(detalle).State = EntityState.Modified;

                        //Marcar todas las propiedades que no se pueden actualizar como FALSE
                        //para que no se actualice su informacion en Base de Datos
                        foreach (string propertyName in ARtblRequisicionMaterialDetalle.PropiedadesNoActualizables)
                        {
                            Context.Entry(detalle).Property(propertyName).IsModified = false;
                        }
                    }
                }

                //Guardamos cambios
                Context.SaveChanges();
            }
        }

        public int GuardaCambios(ARtblRequisicionMaterial entidad, List<ARtblRequisicionMaterialDetalle> detalles)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    //Creamos el Id de la cabecera
                    int requisicionId = entidad.RequisicionMaterialId;

                    //Validamos si es un registro nuevo
                    if (entidad.RequisicionMaterialId == 0)
                    {
                        requisicionId = Inserta(entidad).RequisicionMaterialId;
                    }
                    else
                    {
                        Actualiza(entidad);
                    }

                    //Guardamos los detalles
                    if (detalles != null)
                    {
                        GuardaDetalles(requisicionId, detalles);
                    }

                    //Actualizamos el estatus de la Requisición
                    Context.ARspActualizaEstatusRequisicionMaterial(requisicionId);

                    //Hacemos el Commit
                    SAACGContextHelper.Commit();

                    //Retornamos la entidad que se acaba de guardar en la Base de Datos
                    return requisicionId;
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

        public List<ARspConsultaRequisicionMaterialProductos_Result> BuscaComboProductos(string areaId, string unidadAdministrativaId, string proyectoId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaRequisicionMaterialProductos(areaId, unidadAdministrativaId, proyectoId).ToList();
            }
        }

        public ARtblRequisicionMaterialDetalle BuscaDetallePorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARtblRequisicionMaterialDetalle.Where(m => m.RequisicionMaterialDetalleId == id).FirstOrDefault();
            }
        }

        public List<ARspConsultaRequisicionMaterialDetalles_Result> BuscaDetallesPorRequisicionMaterialId(int requisicionId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaRequisicionMaterialDetalles(requisicionId).ToList();
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

        public void Autorizar(int requisicionMaterialId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    Context.ARspRequisicionMaterialAutorizar(requisicionMaterialId);

                    //Guardamos cambios
                    Context.SaveChanges();

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
    }
}