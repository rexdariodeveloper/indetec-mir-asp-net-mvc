using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RequisicionesAlmacenBL.Services.SAACG
{
    public class ProgramaGobiernoService : BaseService<tblProgramaGobierno>
    {
        public override bool Actualiza(tblProgramaGobierno entidad)
        {
            throw new NotImplementedException();
        }

        public override tblProgramaGobierno BuscaPorId(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Elimina(int id, int eliminadoPorId)
        {
            throw new NotImplementedException();
        }

        public override tblProgramaGobierno Inserta(tblProgramaGobierno entidad)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<tblProgramaGobierno> BuscaTodos()
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.tblProgramaGobierno.ToList();
            }
        }

        public IEnumerable<spComboProgramaGobierno_Result> BuscaComboProgramaGobierno(string programaPresupuestarioId)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                return Context.spComboProgramaGobierno(programaPresupuestarioId).ToList();
            }
        }
    }
}
