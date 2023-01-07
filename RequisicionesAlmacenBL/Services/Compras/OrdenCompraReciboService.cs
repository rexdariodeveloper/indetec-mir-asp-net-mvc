using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using static RequisicionesAlmacenBL.Models.Mapeos.ControlMaestroMapeo;

namespace RequisicionesAlmacenBL.Services.Compras
{
    public class OrdenCompraReciboService : BaseService<tblCompra>
    {
        public override tblCompra BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.tblCompra.Where(m => m.CompraId == id).FirstOrDefault();
            }
        }

        public List<tblCompra> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.tblCompra.ToList();
            }
        }

        public List<ARvwListadoOrdenCompraRecibo> BuscaListado()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARvwListadoOrdenCompraRecibo.ToList();
            }
        }

        public override tblCompra Inserta(tblCompra entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad el Context
                tblCompra compra = Context.tblCompra.Add(entidad);

                //Guardamos cambios
                Context.SaveChanges();

                //Retornamos si se guardó correctamente
                return compra;
            }
        }

        public override bool Actualiza(tblCompra entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad el Context
                tblCompra ordenCompra = Context.tblCompra.Add(entidad);

                //Marcamos el modelo como modificado
                Context.Entry(entidad).State = EntityState.Modified;

                //Marcar todas las propiedades que no se pueden actualizar como FALSE
                //para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in tblCompra.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                //Guardamos cambios
                return Context.SaveChanges() > 0;
            }
        }

        public int GuardaCambios(int ordenCompraId,
                                       int usuarioId,
                                       tblCompra entidad,
                                       List<tblCompraDet> detalles,
                                       List<int> requisicionesIds,
                                       List<ARtblRequisicionMaterialDetalle> detallesRecibidos)
        {
            return GuardaCambios(ordenCompraId,
                                 usuarioId,
                                 entidad,
                                 detalles,
                                 requisicionesIds,
                                 detallesRecibidos,
                                 false,
                                 null);
        }

        public int GuardaCambios(int ordenCompraId, 
                                       int usuarioId, 
                                       tblCompra entidad, 
                                       List<tblCompraDet> detalles,
                                       List<int> requisicionesIds,
                                       List<ARtblRequisicionMaterialDetalle> detallesRecibidos,
                                       bool cancelacion,
                                       ARtblCompraCancelInfo cancelInfo)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    //Creamos el Id de la cabecera
                    int compraId = entidad.CompraId;

                    //Validamos si es registro nuevo
                    if (entidad.CompraId == 0)
                    {
                        compraId = Inserta(entidad).CompraId;
                    }
                    else
                    {
                        Actualiza(entidad);
                    }

                    //Guardamos cambios
                    Context.SaveChanges();

                    if (detalles != null)
                    {
                        int tipoMovimientoId = !cancelacion ? TipoInventarioMovimiento.ORDEN_COMPRA_RECIBO : TipoInventarioMovimiento.CANCELACION_RECIBO_OC;
                        string motivoMovto = (cancelacion ? "Cancelación de " : "") + "Recibo de OC: " + ordenCompraId;
                        int creadoPorId = usuarioId;
                        int referenciaMovtoId = compraId;

                        List<ARudtInventarioMovimiento> movimientosUDT = new List<ARudtInventarioMovimiento>();

                        //Registramos los movimientos en el inventario
                        foreach (tblCompraDet detalle in detalles)
                        {
                            tblCompraDet detalleTemp;

                            if (!cancelacion)
                            {
                                //Agregamos el id de la cabecera
                                detalle.CompraId = compraId;

                                //Agregamos los detalles al Context
                                detalleTemp = Context.tblCompraDet.Add(detalle);

                                if (detalle.CompraDetId > 0)
                                {
                                    Context.Entry(detalle).State = EntityState.Modified;

                                    //Marcar todas las propiedades que no se pueden actualizar como FALSE
                                    //para que no se actualice su informacion en Base de Datos
                                    foreach (string propertyName in tblCompraDet.PropiedadesNoActualizables)
                                    {
                                        Context.Entry(detalle).Property(propertyName).IsModified = false;
                                    }
                                }

                                //Guardamos cambios
                                Context.SaveChanges();
                            } 
                            else
                            {
                                detalle.Cantidad = detalle.Cantidad * -1;
                                detalleTemp = detalle;
                            }

                            ARudtInventarioMovimiento udt = new ARudtInventarioMovimiento();

                            udt.AlmacenProductoId = detalle.AlmacenProductoId;
                            udt.CantidadMovimiento = Convert.ToDecimal(detalle.Cantidad);
                            udt.CostoUnitario = detalle.Costo;
                            udt.ReferenciaMovtoId = detalleTemp.CompraDetId;

                            movimientosUDT.Add(udt);
                        }

                        new ProcesadorInventariosService().Execute(tipoMovimientoId,
                                                                   motivoMovto,
                                                                   creadoPorId,
                                                                   true,
                                                                   referenciaMovtoId,
                                                                   null,
                                                                   movimientosUDT);
                    }

                    //Actualizamos el estatus de la OC
                    Context.ARspActualizaEstatusOrdenCompra(ordenCompraId);

                    //Actualizamos los detalles recibidos
                    if (detallesRecibidos != null)
                    {
                        new RequisicionMaterialService().GuardaDetalles(detallesRecibidos);
                    }

                    if (requisicionesIds != null)
                    {
                        foreach (int requisicionId in requisicionesIds)
                        {
                            //Actualizamos el estatus de la Requisición
                            Context.ARspActualizaEstatusRequisicionMaterial(requisicionId);
                        }
                    }

                    if (cancelacion && cancelInfo != null)
                    {
                        //Agregamos la entidad el Context
                        ARtblCompraCancelInfo cancelInfoTemp = Context.ARtblCompraCancelInfo.Add(cancelInfo);

                        //Guardamos cambios
                        Context.SaveChanges();
                    }

                    //Hacemos el Commit
                    SAACGContextHelper.Commit();

                    //Retornamos la entidad que se acaba de guardar en la Base de Datos
                    return compraId;
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

        public List<ARspConsultaOCPorRecibir_Result> BuscaComboOrdenesCompra()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaOCPorRecibir().ToList();
            }
        }


        public List<ARspConsultaOCPorRecibirDetalles_Result> BuscaOrdenesCompraDetalles()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaOCPorRecibirDetalles().ToList();
            }
        }

        public List<ARspConsultaOrdenCompraReciboDetalles_Result> BuscaDetallesPorCompraId(int compraId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaOrdenCompraReciboDetalles(compraId).ToList();
            }
        }

        public ARspConsultaDatosOCReciboPorId_Result BuscaDatosOCReciboPorId(int ordenCompraId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaDatosOCReciboPorId(ordenCompraId).FirstOrDefault();
            }
        }
    }
}