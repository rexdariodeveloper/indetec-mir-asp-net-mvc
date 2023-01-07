using RequisicionesAlmacenBL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using static RequisicionesAlmacenBL.Models.Mapeos.ControlMaestroMapeo;
using RequisicionesAlmacenBL.Helpers;

namespace RequisicionesAlmacenBL.Services
{
    public class EmpleadoService : BaseService<RHtblEmpleado>
    {
        public override RHtblEmpleado Inserta(RHtblEmpleado entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad el Context
                RHtblEmpleado empleado = Context.RHtblEmpleado.Add(entidad);

                //Guardamos cambios
                Context.SaveChanges();

                //Retornamos la entidad que se acaba de guardar en la Base de Datos
                return empleado;
            }
        }

        public override bool Actualiza(RHtblEmpleado entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Agregamos la entidad que vamos a actualizar al Context
                Context.RHtblEmpleado.Add(entidad);

                //Marcamos el modelo como modificado
                Context.Entry(entidad).State = EntityState.Modified;

                //Marcar todas las propiedades que no se pueden actualizar como FALSE
                //para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in RHtblEmpleado.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                //Retornamos true o false si se realizo correctamente la operacion
                return Context.SaveChanges() > 0;
            }
        }
        
        public bool GuardaCambios(RHtblEmpleado entidad, 
                                  string nombreFotografia, 
                                  Stream fotografia, 
                                  bool cambioImagen, 
                                  bool eliminaArchivo,
                                  Nullable<Guid> archivoAnteriorId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    //Validamos si es un registro nuevo
                    if (entidad.EmpleadoId == 0)
                    {
                        Inserta(entidad);
                    }
                    else
                    {
                        //Si es necesario eliminamos la fotografía
                        if (eliminaArchivo && archivoAnteriorId != null)
                        {
                            new ArchivoService().EliminaArchivo(archivoAnteriorId, entidad.ModificadoPorId);
                        }                        

                        Actualiza(entidad);
                    }

                    //Guardamos la imagen
                    if (cambioImagen && fotografia != null)
                    {
                        GuardaImagen(entidad, nombreFotografia, fotografia);
                    }

                    //Hacemos el Commit
                    SAACGContextHelper.Commit();

                    //Retornamos true o false si se realizo correctamente la operacion
                    return true;
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

        public bool Elimina(RHtblEmpleado entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    //Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    //Actualizamos el modelo
                    Actualiza(entidad);

                    //Hacemos el Commit
                    SAACGContextHelper.Commit();

                    //Retornamos true o false si se realizo correctamente la operacion
                    return true;
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

        public override RHtblEmpleado BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.RHtblEmpleado.Find(id);
            }
        }
        
        public RHtblEmpleado BuscaPorUsuarioId(int usuarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.GRtblUsuario.Where(m => m.UsuarioId == usuarioId)
                    .Join(Context.RHtblEmpleado, u => u.EmpleadoId, e => e.EmpleadoId, (u, e) => new { e = e, u = u })
                    .AsEnumerable().Select(m => new RHtblEmpleado {
                        EmpleadoId = m.e.EmpleadoId,
                        NumeroEmpleado = m.e.NumeroEmpleado,
                        RFC = m.e.RFC,
                        Nombre = m.e.Nombre,
                        PrimerApellido = m.e.PrimerApellido,
                        SegundoApellido = m.e.SegundoApellido,
                        TipoEmpleadoId = m.e.TipoEmpleadoId,
                        AreaAdscripcionId = m.e.AreaAdscripcionId
                    }).FirstOrDefault();
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RHtblEmpleado> BuscaActivos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.RHtblEmpleado.Where(m => m.EstatusId == EstatusRegistro.ACTIVO).ToList();
            }
        }

        private void GuardaImagen(RHtblEmpleado empleado, string nombreArchivoTemporal, Stream fotografia)
        {
            //Guardar Si se cambió la fotografía
            if (fotografia != null)
            {
                ArchivoService archivoService = new ArchivoService();

                string extensionArchivo = nombreArchivoTemporal.Substring(nombreArchivoTemporal.LastIndexOf("."));
                int tipoArchivo = archivoService.ObtenerTipoArchivo(extensionArchivo);

                //Buscamos que no haya un archivo asociado al empleado
                GRtblArchivo archivoAnterior = archivoService.BuscaPorReferenciaOrienTipo(empleado.EmpleadoId, ListadoCMOA.FOTOGRAFIA_EMPLEADO, tipoArchivo);

                Nullable<Guid> archivoAnteriorId = null;

                if (archivoAnterior != null)
                {
                    archivoAnteriorId = archivoAnterior.ArchivoId;
                }

                List<string> ids = new List<string>();
                ids.Add(empleado.EmpleadoId.ToString());

                Nullable<int> usuarioId = empleado.ModificadoPorId != null ? empleado.ModificadoPorId : empleado.CreadoPorId;

                Guid fotografiaId = archivoService.GuardaArchivo(usuarioId,
                                                                archivoAnteriorId,
                                                                nombreArchivoTemporal,
                                                                fotografia,
                                                                ListadoCMOA.FOTOGRAFIA_EMPLEADO,
                                                                ids,
                                                                empleado.EmpleadoId,
                                                                tipoArchivo);

                empleado.Fotografia = fotografiaId;

                Actualiza(empleado);
            }
        }

        public bool ExisteRFC(int empleadoId, string rfc)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.RHtblEmpleado.FirstOrDefault(m =>
                    m.EmpleadoId != empleadoId
                    && m.EstatusId == EstatusRegistro.ACTIVO
                    && m.RFC.ToLower() == rfc.ToLower()) != null;
            }
        }

        public bool ExisteNumeroEmpleado(int empleadoId, string numeroEmpleado)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.RHtblEmpleado.FirstOrDefault(m =>
                    m.EmpleadoId != empleadoId
                    && m.EstatusId == EstatusRegistro.ACTIVO
                    && m.NumeroEmpleado.ToLower() == numeroEmpleado.ToLower()) != null;
            }
        }

        public IEnumerable<RHspConsultaEmpleadoLigado_Result> BuscaLigadoTodos(int usuarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.RHspConsultaEmpleadoLigado(usuarioId).ToList();
            }
        }
    }
}
