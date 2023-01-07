using RequisicionesAlmacenBL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RequisicionesAlmacen.Areas.MIR.MIR.Models.ViewModel
{
    public class MatrizPresupuestoDevengadoViewModel
    {
        public IEnumerable<MIvwListadoMatrizConfiguracionPresupuestal> ListadoMatrizConfiguracionPresupuestal { get; set; }
        public MIspConsultaMatrizIndicadorResultado_Result ConsultaMatrizIndicadorResultado { get; set; }
        public IEnumerable<MItblMatrizIndicadorResultadoIndicador> ListaMatrizIndicadorResultadoIndicador { get; set; }
        public MItblMatrizConfiguracionPresupuestal MatrizConfiguracionPresupuestal { get; set; }
        public MItblMatrizConfiguracionPresupuestal MatrizConfiguracionPresupuestalModel { get; set; }
        public MItblMatrizConfiguracionPresupuestalDetalle MatrizConfiguracionPresupuestalDetalleModel { get; set; }
        public IEnumerable<MItblMatrizConfiguracionPresupuestalDetalle> ListaMatrizConfiguracionPresupuestalDetalle { get; set; }
        public IEnumerable<DevengadoModel> ListaDevengado { get; set; }
        public IEnumerable<MItblControlMaestroControlPeriodo> ListaControlMaestroControlPeriodo { get; set; }
        public IEnumerable<MItblMatrizIndicadorResultadoIndicador> ListaMIRIComponente { get; set; }
    }

    public class DevengadoModel
    {
        public int MIRIndicadorId { get; set; }
        public decimal Enero { get; set; }
        public decimal Febrero { get; set; }
        public decimal Marzo { get; set; }
        public decimal Abril { get; set; }
        public decimal Mayo { get; set; }
        public decimal Junio { get; set; }
        public decimal Julio { get; set; }
        public decimal Agosto { get; set; }
        public decimal Septiembre { get; set; }
        public decimal Octubre { get; set; }
        public decimal Noviembre { get; set; }
        public decimal Diciembre { get; set; }
        public decimal Anual { get; set; }
    }
}