using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RequisicionesAlmacenBL.Services.SAACG
{
    public class ProyectoService : BaseService<tblProyecto>
    {
        public override bool Actualiza(tblProyecto entidad)
        {
            throw new NotImplementedException();
        }

        public override tblProyecto BuscaPorId(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override tblProyecto Inserta(tblProyecto entidad)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<tblProyecto> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.tblProyecto.AsEnumerable().Select(proyecto => new tblProyecto { 
                    ProyectoId = proyecto.ProyectoId, 
                    ClasificadorFuncionalId = proyecto.ClasificadorFuncionalId,
                    SubProgramaGobiernoId = proyecto.SubProgramaGobiernoId,
                    Nombre = proyecto.Nombre,
                    Editable = proyecto.Editable,
                    ClasificacionGeograficaId = proyecto.ClasificacionGeograficaId,
                    Capitulos = proyecto.Capitulos
                }).ToList().OrderBy(m => m.ProyectoId);
            }
        }

        public List<tblProyecto> BuscaProyectosPorRamoId(string ramoId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.tblProyecto.Where(m => m.tblRamo.Where(r => r.RamoId == ramoId).Any()).ToList();
            }
        }

        public List<MIspRep_Proyecto_Result> BuscaFechaInicioYFechaFin(DateTime fechaInicio, DateTime fechaFin)
        {
            return EjecutarRepProyecto(fechaInicio, fechaFin, "");
        }

        private List<MIspRep_Proyecto_Result> EjecutarRepProyecto(DateTime fehcaInicio, DateTime fechaFin, string where)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.MIspRep_Proyecto(fehcaInicio, fechaFin, where).ToList();
            }
        }

        public List<string> BuscaRelacionProyectoYProgramaGobierno(string programaGobiernoId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.Database.SqlQuery<string>("SELECT p.ProyectoId FROM tblProyecto p " +
                    "INNER JOIN tblSubProgramaGobierno spg ON p.SubProgramaGobiernoId = spg.SubProgramaGobiernoId " +
                    "INNER JOIN tblProgramaGobierno pg ON spg.ProgramaGobiernoId = pg.ProgramaGobiernoId " +
                    "WHERE pg.ProgramaGobiernoId = " + (programaGobiernoId != "" ? programaGobiernoId : "'0'")).ToList();
            }
        }
    }
}
