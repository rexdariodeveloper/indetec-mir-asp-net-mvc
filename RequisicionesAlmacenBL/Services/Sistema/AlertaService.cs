using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using RequisicionesAlmacenBL.Models.Mapeos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RequisicionesAlmacenBL.Models.Mapeos.ControlMaestroMapeo;
using System.Data.Entity.Core.Objects;

namespace RequisicionesAlmacenBL.Services.Sistema
{
    public class AlertaService : BaseService<GRtblAlerta>
    {
        public override GRtblAlerta Inserta(GRtblAlerta entidad)
        {
            throw new NotImplementedException();
        }

        public override bool Actualiza(GRtblAlerta entidad)
        {
            throw new NotImplementedException();
        }

        public override GRtblAlerta BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.GRtblAlerta.Where(m => m.AlertaId == id).FirstOrDefault();
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GRspGetListadoAlertasPorUsuario_Result> GetListadoAlertas(int usuarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.GRspGetListadoAlertasPorUsuario(usuarioId).ToList();
            }
        }

        public int IniciarAlerta(int alertaDefinicionId, int referenciaProcesoId, string codigoTramite, string textoRepresentativo, int alertaCreadaPorId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    return Context.GRspIniciarAlerta(AlertaAccion.INICIAR, 
                                                     alertaDefinicionId, 
                                                     referenciaProcesoId, 
                                                     codigoTramite, 
                                                     textoRepresentativo, 
                                                     alertaCreadaPorId,
                                                     new ObjectParameter("valorSalida", ""));
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

        public int AutorizarAlerta(long alertaId, int usuarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    return Context.GRspAutorizarAlerta(AlertaAccion.AUTORIZAR, usuarioId, alertaId, new ObjectParameter("valorSalida", ""));
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

        public int RevisionAlerta(int alertaId, string motivo, int usuarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    return Context.GRspRevisionRechazarAlerta(AlertaAccion.AUTORIZAR, usuarioId, alertaId, motivo, new ObjectParameter("valorSalida", ""));
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

        public int RechazarAlerta(int alertaId, string motivo, int usuarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    return Context.GRspRevisionRechazarAlerta(AlertaAccion.RECHAZAR, usuarioId, alertaId, motivo, new ObjectParameter("valorSalida", ""));
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

        public int OcultarAlertas(string alertasId, int usuarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    return Context.GRspOcultarAlertas(AlertaAccion.OCULTAR, usuarioId, alertasId, new ObjectParameter("valorSalida", ""));
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