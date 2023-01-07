using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using static RequisicionesAlmacenBL.Models.Mapeos.ControlMaestroMapeo;

namespace RequisicionesAlmacenBL.Services.Sistema
{
    public class RolService : BaseService<GRtblRol>
    {
        public override bool Actualiza(GRtblRol entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad que vamos a actualizar al Context
                Context.GRtblRol.Add(entidad);
                Context.Entry(entidad).State = EntityState.Modified;

                // Marcar todas las propiedades que no se pueden actualizar como FALSE
                // para que no se actualice su informacion en Base de Datos
                foreach (string propertyName in GRtblRol.PropiedadesNoActualizables)
                {
                    Context.Entry(entidad).Property(propertyName).IsModified = false;
                }

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos true o false si se realizo correctamente la operacion
                return true;
            }
        }

        public override GRtblRol BuscaPorId(int id)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                //Retornamos la entidad con el ID que se envio como parametro
                return Context.GRtblRol.Where(rol => rol.RolId == id && rol.EstatusId == EstatusRegistro.ACTIVO).FirstOrDefault();
            }
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override GRtblRol Inserta(GRtblRol entidad)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                // Agregamos la entidad el Context
                GRtblRol rol = Context.GRtblRol.Add(entidad);

                // Guardamos cambios
                Context.SaveChanges();

                // Retornamos la entidad que se acaba de guardar en la Base de Datos
                return rol;
            }
        }

        public void GuardaCambios(GRtblRol rol, IEnumerable<GRtblRolMenu> listaRolMenu)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                try
                {
                    // Iniciamos la transacción
                    SAACGContextHelper.BeginTransaction();

                    // Creamos el return
                    GRtblRol _rol = rol;

                    if (_rol != null)
                    {
                        // Si es un registro nuevo
                        if (_rol.RolId > 0)
                        {
                            Actualiza(_rol);
                        }
                        else
                        {
                            _rol = Inserta(rol);

                            listaRolMenu.ToList().ForEach(r =>
                            {
                                r.RolId = _rol.RolId;
                            });

                        }
                    }

                    // Existe la lista para guardar o actualizar
                    if (listaRolMenu != null)
                    {
                        GuardaListaRolMenu(listaRolMenu);
                    }

                    // Hacemos el Commit
                    SAACGContextHelper.Commit();
                }
                catch (DbEntityValidationException ex)
                {
                    // Hacemos el Rollback
                    SAACGContextHelper.Rollback();

                    throw new Exception(UserExceptionHelper.GetMessage(ex));
                }
                catch (Exception ex)
                {
                    // Hacemos el Rollback
                    SAACGContextHelper.Rollback();

                    throw new Exception(UserExceptionHelper.GetMessage(ex));
                }
            }
        }

        public void GuardaListaRolMenu(IEnumerable<GRtblRolMenu> listaRolMenu)
        {
            // Service
            RolMenuService rolMenuService = new RolMenuService();

            foreach (GRtblRolMenu rolMenu in listaRolMenu.ToList())
            {
                // Creamos el return
                GRtblRolMenu _rolMenu = rolMenu;

                if (_rolMenu.RolMenuId > 0)
                {
                    // Actualizamos
                    rolMenuService.Actualiza(_rolMenu);
                }
                else
                {
                    // Guardamos
                    _rolMenu = rolMenuService.Inserta(_rolMenu);
                }
            }
        }

        public IEnumerable<GRvwListadoRol> BuscaListado()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.GRvwListadoRol.ToList();
            }
        }

        public IEnumerable<GRtblRol> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.GRtblRol.Where(rol => rol.EstatusId == 1).ToList();
            }
        }
    }
}
