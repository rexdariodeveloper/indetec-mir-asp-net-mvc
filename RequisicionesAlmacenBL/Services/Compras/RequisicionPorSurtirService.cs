using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using static RequisicionesAlmacenBL.Models.Mapeos.ControlMaestroMapeo;

namespace RequisicionesAlmacenBL.Services.Compras
{
    public class RequisicionPorSurtirService : BaseService<ARtblRequisicionMaterial>
    {
        public override ARtblRequisicionMaterial BuscaPorId(int id)
        {
            throw new NotImplementedException();
        }

        public List<ARtblRequisicionMaterial> BuscaTodos()
        {
            throw new NotImplementedException();
        }

        public List<ARvwListadoRequisicionPorSurtir> BuscaListado()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARvwListadoRequisicionPorSurtir.ToList();
            }
        }

        public override ARtblRequisicionMaterial Inserta(ARtblRequisicionMaterial entidad)
        {
            throw new NotImplementedException();
        }

        public override bool Actualiza(ARtblRequisicionMaterial entidad)
        {
            throw new NotImplementedException();
        }
        
        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public List<ARspConsultaRequisicionPorSurtirDetalles_Result> BuscaDetallesPorRequisicionMaterialId(int requisicionId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaRequisicionPorSurtirDetalles(requisicionId).ToList();
            }
        }

        public List<ARspConsultaRequisicionPorSurtirExistencias_Result> BuscaExistenciaProducto(int requisicionId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaRequisicionPorSurtirExistencias(requisicionId).ToList();
            }
        }

        public void GuardaCambios(int requisicionId,
                                  string codigo,
                                  List<RequisicionDetalleSurtirItem> movimientos,
                                  List<ARtblRequisicionMaterialDetalle> detalles,
                                  int usuarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    if (movimientos != null)
                    {
                        int tipoMovimientoId = TipoInventarioMovimiento.REQUISICION_MATERIAL_SURTIMIENTO;
                        string motivoMovto = "Surtimiento de Solicitud: " + codigo;
                        int creadoPorId = usuarioId;
                        int referenciaMovtoId = requisicionId;

                        List<ARudtInventarioMovimiento> movimientosUDT = new List<ARudtInventarioMovimiento>();

                        //Registramos los movimientos en el inventario
                        foreach (RequisicionDetalleSurtirItem movimiento in movimientos)
                        {
                            ARudtInventarioMovimiento udt = new ARudtInventarioMovimiento();
                            
                            udt.AlmacenProductoId = movimiento.AlmacenProductoId;
                            udt.CantidadMovimiento = -1 * movimiento.CantidadSurtir;
                            udt.ReferenciaMovtoId = movimiento.RequisicionMaterialDetalleId;

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

                    //Actualizamos los detalles en Revisión o Por Comprar
                    if (detalles != null)
                    {
                        new RequisicionMaterialService().GuardaDetalles(requisicionId, detalles);
                    }

                    //Actualizamos el estatus de la Requisición
                    Context.ARspActualizaEstatusRequisicionMaterial(requisicionId);

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