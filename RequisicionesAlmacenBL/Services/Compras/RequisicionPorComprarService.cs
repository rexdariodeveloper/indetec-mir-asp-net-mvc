﻿using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace RequisicionesAlmacenBL.Services.Compras
{
    public class RequisicionPorComprarService : BaseService<ARtblRequisicionMaterial>
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

        public List<ARspConsultaRequisicionPorComprarDetalles_Result> BuscaRequisicionMaterialPorComprarDetalles()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaRequisicionPorComprarDetalles().ToList();
            }
        }

        public List<ARspConsultaRequisicionPorComprarFuentesFinanciamiento_Result> BuscaFuentesFinanciamientoProducto()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.ARspConsultaRequisicionPorComprarFuentesFinanciamiento().ToList();
            }
        }

        public void GuardaCambios(List<tblOrdenCompra> ordenesCompra,
                                  List<ARtblInvitacionArticulo> invitacionesArticulo,
                                  List<int> requisicionesIds, 
                                  List<ARtblRequisicionMaterialDetalle> detallesConvertidos,
                                  List<ARtblRequisicionMaterialDetalle> detallesRevision,
                                  int usuarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    if (ordenesCompra != null)
                    {
                        List<tblOrdenCompra> ordenesComprasTemp = new List<tblOrdenCompra>();

                        //Registramos los movimientos
                        foreach (tblOrdenCompra ordenCompra in ordenesCompra)
                        {
                            //Agregamos la OC para la tabla ARtblOrdenCompraRequisicionDet
                            ordenesComprasTemp.Add(new OrdenCompraService().Inserta(ordenCompra));
                        }

                        if (ordenesComprasTemp.Count > 0)
                        {
                            foreach (tblOrdenCompra ocTemp in ordenesComprasTemp)
                            {
                                foreach (tblOrdenCompraDet ocDetalleTemp in ocTemp.tblOrdenCompraDet)
                                {
                                    ARtblOrdenCompraRequisicionDet ocRequisicionDetalle = new ARtblOrdenCompraRequisicionDet();
                                    ocRequisicionDetalle.OrdenCompraDetId = ocDetalleTemp.OrdenCompraDetId;
                                    ocRequisicionDetalle.RequisicionMaterialDetalleId = ocDetalleTemp.RequisicionMaterialDetalleId;
                                    ocRequisicionDetalle.Cantidad = Convert.ToDecimal(ocDetalleTemp.Cantidad);
                                    ocRequisicionDetalle.CreadoPorId = usuarioId;

                                    //Agregamos los detalles al Context
                                    Context.ARtblOrdenCompraRequisicionDet.Add(ocRequisicionDetalle);
                                }
                            }

                            //Guardamos cambios
                            Context.SaveChanges();
                        }
                    }

                    if (invitacionesArticulo != null)
                    {
                        //Registramos los movimientos
                        foreach (ARtblInvitacionArticulo invitacionArticulo in invitacionesArticulo)
                        {
                            //Guardamos el modelo para la tabla ARtblInvitacionArticulo
                            new InvitacionArticuloService().Inserta(invitacionArticulo);
                        }
                    }

                    //Actualizamos Requisición detalles convertidos
                    if (detallesConvertidos != null)
                    {
                        new RequisicionMaterialService().GuardaDetalles(detallesConvertidos);
                    }

                    //Actualizamos Requisición detalles en revisión
                    if (detallesRevision != null)
                    {
                        new RequisicionMaterialService().GuardaDetalles(detallesRevision);
                    }

                    if (requisicionesIds != null)
                    {
                        foreach (int requisicionId in requisicionesIds)
                        {
                            //Actualizamos el estatus de la Requisición
                            Context.ARspActualizaEstatusRequisicionMaterial(requisicionId);
                        }
                    }

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