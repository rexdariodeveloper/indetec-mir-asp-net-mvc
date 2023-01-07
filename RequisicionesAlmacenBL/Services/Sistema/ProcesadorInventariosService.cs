using EntityFrameworkExtras.EF6;
using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Helpers;
using System;
using System.Collections.Generic;

namespace RequisicionesAlmacenBL.Services
{
    public class ProcesadorInventariosService
    {
        public void Execute(int tipoMovimientoId,
                            string motivoMovto,
                            int creadoPorId,
                            bool insertarAgrupador,
                            int referenciaMovtoId,
                            Nullable<int> polizaId,
                            List<ARudtInventarioMovimiento> movimientos)
        {
            using (var Context = SAACGContextHelper.GetContext())
            {
                var procedure = new ARspProcesadorInventarios()
                {
                    TipoMovimientoId = tipoMovimientoId,
                    MotivoMovto = motivoMovto,
                    CreadoPorId = creadoPorId,
                    InsertarAgrupador = insertarAgrupador,
                    ReferenciaMovtoId = referenciaMovtoId,
                    PolizaId = polizaId,
                    Movimientos = movimientos
                };

                Context.Database.ExecuteStoredProcedure(procedure);
            }
        }
    }
}