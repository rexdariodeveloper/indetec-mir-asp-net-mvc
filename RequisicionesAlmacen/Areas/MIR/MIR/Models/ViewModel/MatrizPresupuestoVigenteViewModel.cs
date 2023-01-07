using RequisicionesAlmacenBL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RequisicionesAlmacen.Areas.MIR.MIR.Models.ViewModel
{
    public class MatrizPresupuestoVigenteViewModel
    {
        public IEnumerable<MIvwListadoMatrizConfiguracionPresupuestal> ListadoMatrizConfiguracionPresupuestal { get; set; }
        public MIspConsultaMatrizIndicadorResultado_Result ConsultaMatrizIndicadorResultado { get; set; }
        public IEnumerable<MItblMatrizIndicadorResultadoIndicador> ListaMatrizIndicadorResultadoIndicador { get; set; }
        public MItblMatrizConfiguracionPresupuestal MatrizConfiguracionPresupuestal { get; set; }
        public MItblMatrizConfiguracionPresupuestal MatrizConfiguracionPresupuestalModel { get; set; }
        public MItblMatrizConfiguracionPresupuestalDetalle MatrizConfiguracionPresupuestalDetalleModel { get; set; }
        public IEnumerable<MItblMatrizConfiguracionPresupuestalDetalle> ListaMatrizConfiguracionPresupuestalDetalle { get; set; }
        public IEnumerable<MItblControlMaestroControlPeriodo> ListaControlMaestroControlPeriodo { get; set; }
        public IEnumerable<MItblMatrizIndicadorResultadoIndicador> ListaMIRIComponente { get; set; }
        public IEnumerable<MIspConsultaPresupuestoVigente_Result> ListaConsultaPresupuestoVigente { get; set; }
    }
}