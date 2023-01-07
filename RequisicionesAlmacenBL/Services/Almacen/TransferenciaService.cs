using RequisicionesAlmacenBL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using RequisicionesAlmacenBL.Helpers;

namespace RequisicionesAlmacenBL.Services.Almacen
{
    public class TransferenciaService : BaseService<ARtblTransferencia>
    {
        public override ARtblTransferencia BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARtblTransferencia.Where(m => m.TransferenciaId == id).FirstOrDefault();
            }
        }

        public List<ARtblTransferencia> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARtblTransferencia.ToList();
            }
        }

        public List<ARvwListadoTransferencias> BuscaListado()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARvwListadoTransferencias.ToList();
            }
        }

        public override ARtblTransferencia Inserta(ARtblTransferencia entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Asignamos el autonumerico
                entidad.Codigo = new AutonumericoService().GetSiguienteAutonumerico("Transferencias");

                //Agregamos la entidad el Context
                ARtblTransferencia modelo = Context.ARtblTransferencia.Add(entidad);

                //Guardamos cambios
                Context.SaveChanges();

                //Retornamos si guardó correctamente
                return modelo;
            }
        }

        public override bool Actualiza(ARtblTransferencia entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad el Context
                ARtblTransferencia modelo = Context.ARtblTransferencia.Add(entidad);

                //Marcamos el modelo como modificado
                Context.Entry(entidad).State = EntityState.Modified;

                //Marcar todas las propiedades que no se pueden actualizar como FALSE
                //para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in ARtblTransferencia.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                //Guardamos cambios
                return Context.SaveChanges() > 0;
            }
        }

        public int GuardaCambios(ARtblTransferencia entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    //Guardamos el modelo
                    ARtblTransferencia transferencia = Inserta(entidad);

                    //Guardamos cambios
                    Context.SaveChanges();

                    //Registramos los movimientos en el inventario
                    Context.ARspAfectaTransferencias(transferencia.TransferenciaId, transferencia.CreadoPorId);

                    //Hacemos el Commit
                    SAACGContextHelper.Commit();

                    //Retornamos la entidad que se acaba de guardar en la Base de Datos
                    return transferencia.TransferenciaId;
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

        public List<ARspConsultaTransferenciaDetalles_Result> BuscaMovtosPorTransferenciaId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaTransferenciaDetalles(id).ToList();
            }
        }

        public List<ARspConsultaTransferenciaProductos_Result> BuscaProductos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaTransferenciaProductos().ToList();
            }
        }

        public string GetFechaConFormato(DateTime date)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.GRspGetFechaConFormato(date, false).FirstOrDefault();
            }
        }
    }
}