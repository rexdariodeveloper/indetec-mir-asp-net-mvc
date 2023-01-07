using NCalc;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using RequisicionesAlmacen.Areas.MIR.Reportes.Models.ViewModel;
using RequisicionesAlmacen.Controllers;
using RequisicionesAlmacen.Helpers;
using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Models.Mapeos;
using RequisicionesAlmacenBL.Services;
using RequisicionesAlmacenBL.Services.MIR;
using RequisicionesAlmacenBL.Services.SAACG;
using SACG.sysSacg.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static RequisicionesAlmacen.Areas.MIR.MIR.Controllers.MatrizIndicadorResultadoController;

namespace RequisicionesAlmacen.Areas.MIR.Reportes.Controllers
{
    [Authenticated(nodoMenuId = MenuPrincipalMapeo.ID.REPORTE_SID)]
    public class ReporteSeguimientoIndicadorDesempenioController : BaseController<ReporteSeguimientoIndicadorDesempenioViewModel, ReporteSeguimientoIndicadorDesempenioViewModel>
    {
        public override ActionResult Editar(int id)
        {
            throw new NotImplementedException();
        }

        public override JsonResult Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        public override JsonResult Guardar(ReporteSeguimientoIndicadorDesempenioViewModel modelView)
        {
            throw new NotImplementedException();
        }

        // GET: MIR/ReporteSeguimientoIndicadorDesempenio
        public ActionResult Index()
        {
            // Creamos el objecto nuevo
            ReporteSeguimientoIndicadorDesempenioViewModel reporteSeguimientoIndicadorDesempenioViewModel = new ReporteSeguimientoIndicadorDesempenioViewModel();
            //como son los Listados para combos, tablas, arboles.
            GetDatosFicha(ref reporteSeguimientoIndicadorDesempenioViewModel);
            // Retornamos la vista junto con su Objeto Modelo
            return View("ReporteSeguimientoIndicadorDesempenio", reporteSeguimientoIndicadorDesempenioViewModel);
        }

        public override ActionResult Listar()
        {
            throw new NotImplementedException();
        }

        public override ActionResult Nuevo()
        {
            throw new NotImplementedException();
        }

        protected override void GetDatosFicha(ref ReporteSeguimientoIndicadorDesempenioViewModel reporteSeguimientoIndicadorDesempenioViewModel)
        {
            reporteSeguimientoIndicadorDesempenioViewModel.ComboListadoMIR = new MatrizIndicadorResultadoService().BuscaComboListadoMIR();
            reporteSeguimientoIndicadorDesempenioViewModel.ComboListaPeriodo = new List<PeriodoModel>();
            MesMapeo mesMapeo = new MesMapeo();
            for(int x = 1; x <= 7; x++)
            {
                PeriodoModel periodoModel = new PeriodoModel();
                periodoModel.PeriodoId = x;
                periodoModel.NombrePeriodo = mesMapeo.BuscaNombrePeriodo(x);
                reporteSeguimientoIndicadorDesempenioViewModel.ComboListaPeriodo.Add(periodoModel);
            }
        }

        [JsonException]
        public ActionResult BuscarReporte(int mirId, int periodoId)
        {

            MItblMatrizIndicadorResultado matrizIndicadorResultado = new MatrizIndicadorResultadoService().BuscaPorId(mirId);
            if (matrizIndicadorResultado != null)
            {
                FileInfo logotipo = new ArchivoService().GetImagenLogotipo("saacg-net.png");
                var memoryStream = new MemoryStream();
                using (var excelPackage = new ExcelPackage(memoryStream))
                {
                    // Obtener la columna fin con el ID de Periodo 
                    string columnaFin = obtenerColumnaJaP(periodoId);
                    // Reporte Lirbo Consulta Matriz Indicador Resultado 
                    MIspRptLibroConsultaMatrizIndicadorResultado_Result rptLibroConsultaMatrizIndicadorResultado = new MatrizIndicadorResultadoService().BuscaReportePorMIRId(matrizIndicadorResultado.MIRId);
                    // Reporte Lirbo Consulta Plan Desarrollo Estructura
                    List<MIspRptLibroConsultaPlanDesarrolloEstructura_Result> listRptLibroConsultaPlanDesarrolloEstructura = new PlanDesarrolloEstructuraService().BuscaReportePorPlanDesarrolloEstructuraId(matrizIndicadorResultado.PlanDesarrolloEstructuraId).ToList();
                    // Reporte Libro Consulta Lista de MIR
                    List<MIspRptLibroConsultaListaMatrizIndicadorResultado_Result> listRptLibroConsultaListaMatrizIndicadorResultado = new MatrizIndicadorResultadoService().BuscaReporteListaMIRPorMIRId(matrizIndicadorResultado.MIRId).ToList();

                    ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add(matrizIndicadorResultado.Codigo);
                    excelWorksheet.PrinterSettings.Orientation = eOrientation.Landscape;
                    excelWorksheet.PrinterSettings.PaperSize = ePaperSize.Legal;
                    // Modificar el tamaño de una celda por una celda (para sirve el papel oficio)
                    excelWorksheet.Column(1).Width = Convert.ToDouble(14.41);
                    excelWorksheet.Column(2).Width = Convert.ToDouble(12.73);
                    excelWorksheet.Column(3).Width = Convert.ToDouble(11.30);
                    excelWorksheet.Column(4).Width = Convert.ToDouble(12.30);
                    excelWorksheet.Column(5).Width = Convert.ToDouble(9.16);
                    excelWorksheet.Column(6).Width = Convert.ToDouble(9.02);
                    excelWorksheet.Column(7).Width = Convert.ToDouble(10.73);
                    excelWorksheet.Column(8).Width = Convert.ToDouble(11.73);
                    excelWorksheet.Column(9).Width = Convert.ToDouble(7.73);
                    // Ciclo para saber cual es el periodo de fin
                    double columnaDefault = 61.11;
                    double columnaAncho = Math.Round((columnaDefault / periodoId), 2);
                    double columnaAnchoFin = columnaAncho;
                    if (columnaDefault != (columnaAnchoFin * periodoId))
                    {
                        if (columnaDefault > (columnaAnchoFin * periodoId))
                        {
                            columnaAnchoFin = Math.Round(columnaAnchoFin - (columnaDefault - (columnaAnchoFin * periodoId)), 2);
                        }
                        else
                        {
                            columnaAnchoFin = Math.Round(columnaAnchoFin - ((columnaAnchoFin * periodoId) - columnaDefault), 2);
                        }
                    }

                    for (int x = 10; x <= (10 + (periodoId - 1)); x++)
                    {
                        if (x == (10 + (periodoId - 1)))
                        {
                            excelWorksheet.Column(x).Width = columnaAnchoFin;
                        }
                        else
                        {
                            excelWorksheet.Column(x).Width = columnaAncho;
                        }

                    }

                    ExcelHelper excelHelper = new ExcelHelper();

                    // Grupo 1 //
                    Entidad entidad = new ReportHelper().GetDatosEntidad();
                    // Nombre del Ente Público
                    ExcelRange excelRange = excelWorksheet.Cells["A1:" + columnaFin + "1"];
                    excelRange.Value = entidad.Nombre;
                    excelHelper.ExcelTitulo(ref excelRange);
                    // Estado
                    excelRange = excelWorksheet.Cells["A2:" + columnaFin + "2"];
                    excelRange.Value = entidad.Estado;
                    excelHelper.ExcelTitulo(ref excelRange);
                    // Seguimiento a Indicadores de Desempeño
                    excelRange = excelWorksheet.Cells["A3:" + columnaFin + "3"];
                    excelRange.Value = "SEGUIMIENTO A INDICADORES DE DESEMPEÑO";
                    excelHelper.ExcelTitulo(ref excelRange);
                    // 
                    excelRange = excelWorksheet.Cells["A4:" + columnaFin + "4"];
                    excelRange.Value = "(" + new MesMapeo().BuscaNombrePeriodoTitulo(periodoId) + ")";
                    excelHelper.ExcelTitulo(ref excelRange);
                    // 1 Espacio
                    excelRange = excelWorksheet.Cells["A5:" + columnaFin + "5"];
                    excelHelper.ExcelTitulo(ref excelRange);
                    // Logotipo
                    if (logotipo != null)
                    {
                        ExcelPicture excelLogotipo = excelWorksheet.Drawings.AddPicture("SAACG.NET", logotipo);
                        excelLogotipo.SetSize(90, 60);
                        excelLogotipo.SetPosition(0, 30, 0, 20);
                    }
                    // Border
                    excelRange = excelWorksheet.Cells["A1:" + columnaFin + "5"];
                    excelRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    /////////////
                    // Grupo 2 //
                    // Identificacion
                    excelRange = excelWorksheet.Cells["A6:" + columnaFin + "6"];
                    excelRange.Value = "IDENTIFICACIÓN DEL PROGRAMA";
                    excelHelper.ExcelTitulo(ref excelRange);
                    // Programa Presupuestario
                    excelRange = excelWorksheet.Cells["A7:" + columnaFin + "7"];
                    excelRange.Value = "Programa: " + rptLibroConsultaMatrizIndicadorResultado.ProgramaPresupuestario;
                    excelHelper.ExcelParrafo(ref excelRange);
                    // Unidad Responsable del Gasto
                    excelRange = excelWorksheet.Cells["A8:" + columnaFin + "8"];
                    excelRange.Value = "Unidad Responsable del Gasto: " + entidad.Nombre;
                    excelHelper.ExcelParrafo(ref excelRange);
                    // Población Objetivo
                    excelRange = excelWorksheet.Cells["A9:" + columnaFin + "9"];
                    excelRange.Value = "Población Objetivo: " + rptLibroConsultaMatrizIndicadorResultado.PoblacionObjetivo;
                    excelHelper.ExcelParrafo(ref excelRange);
                    // Border
                    excelRange = excelWorksheet.Cells["A5:" + columnaFin + "9"];
                    excelRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    /////////////
                    // Creamos el variable de filaSiguiente para empezar la fila en Alineación
                    int filaSiguiente = 11;
                    // Grupo 3 //
                    // Alineación
                    excelRange = excelWorksheet.Cells["A10:" + columnaFin + "10"];
                    excelRange.Value = "ALINEACIÓN GENERAL PLAN DE DESARROLLO";
                    excelHelper.ExcelTitulo(ref excelRange);
                    // Ciclo la fila contador por alineacion
                    foreach (MIspRptLibroConsultaPlanDesarrolloEstructura_Result rptLibroConsultaPlanDesarrolloEstructura in listRptLibroConsultaPlanDesarrolloEstructura)
                    {
                        // Nivel
                        excelRange = excelWorksheet.Cells["A" + filaSiguiente + ":" + columnaFin + "" + filaSiguiente + ""];
                        excelRange.Value = rptLibroConsultaPlanDesarrolloEstructura.NombreNivel;
                        excelHelper.ExcelParrafo(ref excelRange);
                        filaSiguiente++;
                    }
                    // Border
                    excelRange = excelWorksheet.Cells["A10:" + columnaFin + "" + (filaSiguiente - 1) + ""];
                    excelRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    /////////////

                    int filaIndicadorDefault = filaSiguiente;
                    filaSiguiente = filaSiguiente + 2;
                    // Grupo 4 //
                    excelWorksheet.Row(filaIndicadorDefault + 1).Height = Convert.ToDouble(22.5);
                    // Resumen Narrativo
                    excelRange = excelWorksheet.Cells["A" + filaIndicadorDefault + ":B" + (filaIndicadorDefault + 1) + ""];
                    excelRange.Value = "RESUMEN NARRATIVO";
                    excelHelper.ExcelTablaTitulo(ref excelRange, true, 11, false, false);
                    // Indicadores
                    excelRange = excelWorksheet.Cells["C" + filaIndicadorDefault + ":G" + filaIndicadorDefault + ""];
                    excelRange.Value = "INDICADORES";
                    excelHelper.ExcelTablaTitulo(ref excelRange, true, 11, true, true);
                    // Nombre del Indicador
                    excelRange = excelWorksheet.Cells["C" + (filaIndicadorDefault + 1) + ""];
                    excelRange.Value = "NOMBRE DEL INDICADOR";
                    excelHelper.ExcelTablaTitulo(ref excelRange, false, 8, false, false);
                    // Fórmula
                    excelRange = excelWorksheet.Cells["D" + (filaIndicadorDefault + 1) + ""];
                    excelRange.Value = "FÓRMULA";
                    excelHelper.ExcelTablaTitulo(ref excelRange, false, 8, false, false);
                    // Linea Base
                    excelRange = excelWorksheet.Cells["E" + (filaIndicadorDefault + 1) + ""];
                    excelRange.Value = "LINEA BASE";
                    excelHelper.ExcelTablaTitulo(ref excelRange, false, 8, false, false);
                    // Frecuencia
                    excelRange = excelWorksheet.Cells["F" + (filaIndicadorDefault + 1) + ""];
                    excelRange.Value = "FRECUENCIA";
                    excelHelper.ExcelTablaTitulo(ref excelRange, false, 8, false, false);
                    // Unidad de Medida
                    excelRange = excelWorksheet.Cells["G" + (filaIndicadorDefault + 1) + ""];
                    excelRange.Value = "UNIDAD DE MEDIDA";
                    excelHelper.ExcelTablaTitulo(ref excelRange, false, 8, false, false);
                    // Medios de Verificación
                    excelRange = excelWorksheet.Cells["H" + filaIndicadorDefault + ":H" + (filaIndicadorDefault + 1) + ""];
                    excelRange.Value = "MEDIOS DE VERIFICACIÓN";
                    excelHelper.ExcelTablaTitulo(ref excelRange, true, 8, false, false);
                    // Meta
                    excelRange = excelWorksheet.Cells["I" + filaIndicadorDefault + ":I" + (filaIndicadorDefault + 1) + ""];
                    excelRange.Value = "META";
                    excelHelper.ExcelTablaTitulo(ref excelRange, true, 8, false, false);
                    // Variables
                    excelRange = excelWorksheet.Cells["J" + filaIndicadorDefault + ":" + columnaFin + "" + filaIndicadorDefault + ""];
                    excelRange.Value = "AVANCE";
                    excelHelper.ExcelTablaTitulo(ref excelRange, true, 11, true, true);
                    // Ciclo para saber cual es el periodo de fin (TRIM SEM ANUAL)
                    for (int x = 1; x <= periodoId; x++)
                    {
                        // TRIM SEM ANUAL Depende con el ID de Periodo es fin
                        excelRange = excelWorksheet.Cells[obtenerColumnaJaP(x) + (filaIndicadorDefault + 1) + ""];
                        excelRange.Value = new MesMapeo().BuscaNombrePeriodo3Letras(x);
                        excelHelper.ExcelTablaTitulo(ref excelRange, false, 11, false, false);
                    }
                    /////////////
                    // Grupo 5 //
                    foreach (MIspRptLibroConsultaListaMatrizIndicadorResultado_Result rptLibroConsultaListaMatrizIndicadorResultado in listRptLibroConsultaListaMatrizIndicadorResultado)
                    {
                        List<int> listaUnidadMedidaFormulaVariableId = new List<int>();
                        // Matriz Indicador Resultado Indicador
                        MItblMatrizIndicadorResultadoIndicador matrizIndicadorResultadoIndicador = new MatrizIndicadorResultadoIndicadorService().BuscaPorId(rptLibroConsultaListaMatrizIndicadorResultado.MIRIndicadorId.Value);
                        // Lista Seguimiento Indicador Desempeño
                        IEnumerable<MIfnObtenerSeguimientoIndicadorDesempenio_Result> listaSeguimientoIndicadorDesempenio = new MatrizConfiguracionPresupuestalSeguimientoVariableService().BuscaReporteSIDPorMIRIndicadorId(matrizIndicadorResultadoIndicador.MIRIndicadorId);
                        // Control Maestro Unidad Medida 
                        MItblControlMaestroUnidadMedida controlMaestroUnidadMedida = new ControlMaestroUnidadMedidaService().BuscaPorId(matrizIndicadorResultadoIndicador.UnidadMedidaId);
                        // Creamos variables
                        int mes = 0;
                        double iTrim = 0, iiTrim = 0, iSem = 0, iiiTrim = 0, ivTrim = 0, iiSem = 0, anual = 0;
                        // I, II, III y IV Trimestre -> Mensual y Trimestral
                        if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.MENSUAL || matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.TRIMESTRAL)
                        {
                            mes = 1;
                            // I Trimestre
                            do
                            {
                                Expression ncalc = new Expression(controlMaestroUnidadMedida.Formula);
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.MENSUAL)
                                {
                                    listaSeguimientoIndicadorDesempenio.ToList().ForEach(sid =>
                                    {
                                        // Enero
                                        if (mes == 1)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Enero != null ? sid.Enero : 0;
                                        }
                                        // Febrero
                                        if (mes == 2)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Febrero != null ? sid.Febrero : 0;
                                        }
                                        // Marzo
                                        if (mes == 3)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Marzo != null ? sid.Marzo : 0;
                                        }
                                    });
                                    try
                                    {
                                        iTrim += Convert.ToDouble(ncalc.Evaluate());
                                    }
                                    catch
                                    {
                                        iTrim += 0;
                                    }
                                    
                                    // Contador
                                    mes++;
                                    // Divide 3
                                    if (mes == 4)
                                    {
                                        iTrim = iTrim / 3;
                                    }
                                }
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.TRIMESTRAL)
                                {
                                    listaSeguimientoIndicadorDesempenio.ToList().ForEach(sid =>
                                    {
                                        ncalc.Parameters[sid.Variable] = (sid.Enero != null ? sid.Enero : 0) + (sid.Febrero != null ? sid.Febrero : 0) + (sid.Marzo != null ? sid.Marzo : 0);
                                    });
                                    try
                                    {
                                        iTrim += Convert.ToDouble(ncalc.Evaluate());
                                    }
                                    catch
                                    {
                                        iTrim += 0;
                                    }
                                    // Cambiamos el valor
                                    mes = 4;
                                }
                            } while (!(mes == 4));
                            // II Trimestre
                            do
                            {
                                Expression ncalc = new Expression(controlMaestroUnidadMedida.Formula);
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.MENSUAL)
                                {
                                    listaSeguimientoIndicadorDesempenio.ToList().ForEach(sid =>
                                    {
                                        // Abril
                                        if (mes == 4)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Abril != null ? sid.Abril : 0;
                                        }
                                        // Mayo
                                        if (mes == 5)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Mayo != null ? sid.Mayo : 0;
                                        }
                                        // Junio
                                        if (mes == 6)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Junio != null ? sid.Junio : 0;
                                        }
                                    });
                                    try
                                    {
                                        iiTrim += Convert.ToDouble(ncalc.Evaluate());
                                    }
                                    catch
                                    {
                                        iiTrim += 0;
                                    }
                                    // Contador
                                    mes++;
                                    // Divide 3
                                    if (mes == 7)
                                    {
                                        iiTrim = iiTrim / 3;
                                    }
                                }
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.TRIMESTRAL)
                                {
                                    listaSeguimientoIndicadorDesempenio.ToList().ForEach(sid =>
                                    {
                                        ncalc.Parameters[sid.Variable] = (sid.Abril != null ? sid.Abril : 0) + (sid.Mayo != null ? sid.Mayo : 0) + (sid.Junio != null ? sid.Junio : 0);
                                    });
                                    try
                                    {
                                        iiTrim += Convert.ToDouble(ncalc.Evaluate());
                                    }
                                    catch
                                    {
                                        iiTrim += 0;
                                    }
                                    // Cambiamos el valor
                                    mes = 7;
                                }
                            } while (!(mes == 7));
                            // III Trimestre
                            do
                            {
                                Expression ncalc = new Expression(controlMaestroUnidadMedida.Formula);
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.MENSUAL)
                                {
                                    listaSeguimientoIndicadorDesempenio.ToList().ForEach(sid =>
                                    {
                                        // Julio
                                        if (mes == 7)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Julio != null ? sid.Julio : 0;
                                        }
                                        // Agosto
                                        if (mes == 8)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Agosto != null ? sid.Agosto : 0;
                                        }
                                        // Septiembre
                                        if (mes == 9)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Septiembre != null ? sid.Septiembre : 0;
                                        }
                                    });
                                    try
                                    {
                                        iiiTrim += Convert.ToDouble(ncalc.Evaluate());
                                    }
                                    catch
                                    {
                                        iiiTrim += 0;
                                    }
                                    // Contador
                                    mes++;
                                    // Divide 3
                                    if (mes == 10)
                                    {
                                        iiiTrim = iiiTrim / 3;
                                    }
                                }
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.TRIMESTRAL)
                                {
                                    listaSeguimientoIndicadorDesempenio.ToList().ForEach(sid =>
                                    {
                                        ncalc.Parameters[sid.Variable] = (sid.Julio != null ? sid.Julio : 0) + (sid.Agosto != null ? sid.Agosto : 0) + (sid.Septiembre != null ? sid.Septiembre : 0);
                                    });
                                    try
                                    {
                                        iiiTrim += Convert.ToDouble(ncalc.Evaluate());
                                    }
                                    catch
                                    {
                                        iiiTrim += 0;
                                    }
                                    // Cambiamos el valor
                                    mes = 10;
                                }
                            } while (!(mes == 10));
                            // IV Trimestre
                            do
                            {
                                Expression ncalc = new Expression(controlMaestroUnidadMedida.Formula);
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.MENSUAL)
                                {
                                    listaSeguimientoIndicadorDesempenio.ToList().ForEach(sid =>
                                    {
                                        // Octubre
                                        if (mes == 10)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Octubre != null ? sid.Octubre : 0;
                                        }
                                        // Noviembre
                                        if (mes == 11)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Noviembre != null ? sid.Noviembre : 0;
                                        }
                                        // Diciembre
                                        if (mes == 12)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Diciembre != null ? sid.Diciembre : 0;
                                        }
                                    });
                                    try
                                    {
                                        ivTrim += Convert.ToDouble(ncalc.Evaluate());
                                    }
                                    catch
                                    {
                                        ivTrim += 0;
                                    }
                                    // Contador
                                    mes++;
                                    // Divide 3
                                    if (mes == 13)
                                    {
                                        ivTrim = ivTrim / 3;
                                    }
                                }
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.TRIMESTRAL)
                                {
                                    listaSeguimientoIndicadorDesempenio.ToList().ForEach(sid =>
                                    {
                                        ncalc.Parameters[sid.Variable] = (sid.Octubre != null ? sid.Octubre : 0) + (sid.Noviembre != null ? sid.Noviembre : 0) + (sid.Diciembre != null ? sid.Diciembre : 0);
                                    });
                                    try
                                    {
                                        ivTrim += Convert.ToDouble(ncalc.Evaluate());
                                    }
                                    catch
                                    {
                                        ivTrim += 0;
                                    }
                                    // Cambiamos el valor
                                    mes = 13;
                                }
                            } while (!(mes == 13));
                        }
                        // I y II Semestre -> Mensual, Trimestral y Semestral
                        if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.MENSUAL || matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.TRIMESTRAL || matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.SEMESTRAL)
                        {
                            mes = 1;
                            // I Semestre
                            do
                            {
                                Expression ncalc = new Expression(controlMaestroUnidadMedida.Formula);
                                // Mensual
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.MENSUAL)
                                {
                                    listaSeguimientoIndicadorDesempenio.ToList().ForEach(sid =>
                                    {
                                        // Enero
                                        if (mes == 1)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Enero != null ? sid.Enero : 0;
                                        }
                                        // Febrero
                                        if (mes == 2)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Febrero != null ? sid.Febrero : 0;
                                        }
                                        // Marzo
                                        if (mes == 3)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Marzo != null ? sid.Marzo : 0;
                                        }
                                        // Abril
                                        if (mes == 4)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Abril != null ? sid.Abril : 0;
                                        }
                                        // Mayo
                                        if (mes == 5)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Mayo != null ? sid.Mayo : 0;
                                        }
                                        // Junio
                                        if (mes == 6)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Junio != null ? sid.Junio : 0;
                                        }
                                    });
                                    try
                                    {
                                        iSem += Convert.ToDouble(ncalc.Evaluate());
                                    }
                                    catch
                                    {
                                        iSem += 0;
                                    }
                                    // Contador
                                    mes++;
                                    // Divide 6
                                    if (mes == 7)
                                    {
                                        iSem = iSem / 6;
                                    }
                                }
                                // Trimestral
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.TRIMESTRAL)
                                {
                                    iSem = ((iTrim + iiTrim) / 2);
                                    // Cambiamos el valor
                                    mes = 7;
                                }
                                // Semestral
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.SEMESTRAL)
                                {
                                    listaSeguimientoIndicadorDesempenio.ToList().ForEach(sid =>
                                    {
                                        ncalc.Parameters[sid.Variable] = (sid.Enero != null ? sid.Enero : 0) + (sid.Febrero != null ? sid.Febrero : 0) + (sid.Marzo != null ? sid.Marzo : 0) + (sid.Abril != null ? sid.Abril : 0) + (sid.Mayo != null ? sid.Mayo : 0) + (sid.Junio != null ? sid.Junio : 0);
                                    });
                                    try
                                    {
                                        iSem += Convert.ToDouble(ncalc.Evaluate());
                                    }
                                    catch
                                    {
                                        iSem += 0;
                                    }
                                    // Cambiamos el valor
                                    mes = 7;
                                }
                            } while (!(mes == 7));
                            // II Semestre
                            do
                            {
                                Expression ncalc = new Expression(controlMaestroUnidadMedida.Formula);
                                // Mensual
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.MENSUAL)
                                {
                                    listaSeguimientoIndicadorDesempenio.ToList().ForEach(sid =>
                                    {
                                        // Julio
                                        if (mes == 7)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Julio != null ? sid.Julio : 0;
                                        }
                                        // Agosto
                                        if (mes == 8)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Agosto != null ? sid.Agosto : 0;
                                        }
                                        // Septiembre
                                        if (mes == 9)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Septiembre != null ? sid.Septiembre : 0;
                                        }
                                        // Octubre
                                        if (mes == 10)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Octubre != null ? sid.Octubre : 0;
                                        }
                                        // Noviembre
                                        if (mes == 11)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Noviembre != null ? sid.Noviembre : 0;
                                        }
                                        // Diciembre
                                        if (mes == 12)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Diciembre != null ? sid.Diciembre : 0;
                                        }
                                    });
                                    try
                                    {
                                        iiSem += Convert.ToDouble(ncalc.Evaluate());
                                    }
                                    catch
                                    {
                                        iiSem += 0;
                                    }
                                    // Contador
                                    mes++;
                                    // Divide 6
                                    if (mes == 13)
                                    {
                                        iiSem = iiSem / 6;
                                    }
                                }
                                // Trimestral
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.TRIMESTRAL)
                                {
                                    iiSem = ((iiiTrim + ivTrim) / 2);
                                    // Cambiamos el valor
                                    mes = 13;
                                }
                                // Semestral
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.SEMESTRAL)
                                {
                                    listaSeguimientoIndicadorDesempenio.ToList().ForEach(sid =>
                                    {
                                        ncalc.Parameters[sid.Variable] = (sid.Julio != null ? sid.Julio : 0) + (sid.Agosto != null ? sid.Agosto : 0) + (sid.Septiembre != null ? sid.Septiembre : 0) + (sid.Octubre != null ? sid.Octubre : 0) + (sid.Noviembre != null ? sid.Noviembre : 0) + (sid.Diciembre != null ? sid.Diciembre : 0);
                                    });
                                    try
                                    {
                                        iiSem += Convert.ToDouble(ncalc.Evaluate());
                                    }
                                    catch
                                    {
                                        iiSem += 0;
                                    }
                                    // Cambiamos el valor
                                    mes = 13;
                                }
                            } while (!(mes == 13));
                        }
                        // Anual -> Mensual, Trimestral, Semestral y Anual
                        if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.MENSUAL || matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.TRIMESTRAL || matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.SEMESTRAL || matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.ANUAL)
                        {
                            mes = 1;
                            // Anual
                            do
                            {
                                Expression ncalc = new Expression(controlMaestroUnidadMedida.Formula);
                                // Mensual
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.MENSUAL)
                                {
                                    listaSeguimientoIndicadorDesempenio.ToList().ForEach(sid =>
                                    {
                                        // Enero
                                        if (mes == 1)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Enero != null ? sid.Enero : 0;
                                        }
                                        // Febrero
                                        if (mes == 2)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Febrero != null ? sid.Febrero : 0;
                                        }
                                        // Marzo
                                        if (mes == 3)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Marzo != null ? sid.Marzo : 0;
                                        }
                                        // Abril
                                        if (mes == 4)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Abril != null ? sid.Abril : 0;
                                        }
                                        // Mayo
                                        if (mes == 5)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Mayo != null ? sid.Mayo : 0;
                                        }
                                        // Junio
                                        if (mes == 6)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Junio != null ? sid.Junio : 0;
                                        }
                                        // Julio
                                        if (mes == 7)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Julio != null ? sid.Julio : 0;
                                        }
                                        // Agosto
                                        if (mes == 8)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Agosto != null ? sid.Agosto : 0;
                                        }
                                        // Septiembre
                                        if (mes == 9)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Septiembre != null ? sid.Septiembre : 0;
                                        }
                                        // Octubre
                                        if (mes == 10)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Octubre != null ? sid.Octubre : 0;
                                        }
                                        // Noviembre
                                        if (mes == 11)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Noviembre != null ? sid.Noviembre : 0;
                                        }
                                        // Diciembre
                                        if (mes == 12)
                                        {
                                            ncalc.Parameters[sid.Variable] = sid.Diciembre != null ? sid.Diciembre : 0;
                                        }
                                    });
                                    try
                                    {
                                        anual += Convert.ToDouble(ncalc.Evaluate());
                                    }
                                    catch
                                    {
                                        anual += 0;
                                    }
                                    // Contador
                                    mes++;
                                }
                                // Trimestral
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.TRIMESTRAL)
                                {
                                    anual = iTrim + iiTrim + iiiTrim + ivTrim;
                                    // Cambiamos el valor
                                    mes = 13;
                                }
                                // Semestral
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.SEMESTRAL)
                                {
                                    anual = iSem + iiSem;
                                    // Cambiamos el valor
                                    mes = 13;
                                }
                                // Anual
                                if (matrizIndicadorResultadoIndicador.FrecuenciaMedicionId == FrecuenciaMedicionMapeo.ID.ANUAL)
                                {
                                    listaSeguimientoIndicadorDesempenio.ToList().ForEach(sid =>
                                    {
                                        ncalc.Parameters[sid.Variable] = (sid.Enero != null ? sid.Enero : 0) + (sid.Febrero != null ? sid.Febrero : 0) + (sid.Marzo != null ? sid.Marzo : 0) + (sid.Abril != null ? sid.Abril : 0) + (sid.Mayo != null ? sid.Mayo : 0) + (sid.Junio != null ? sid.Junio : 0) + (sid.Julio != null ? sid.Julio : 0) + (sid.Agosto != null ? sid.Agosto : 0) + (sid.Septiembre != null ? sid.Septiembre : 0) + (sid.Octubre != null ? sid.Octubre : 0) + (sid.Noviembre != null ? sid.Noviembre : 0) + (sid.Diciembre != null ? sid.Diciembre : 0);
                                    });
                                    try
                                    {
                                        anual += Convert.ToDouble(ncalc.Evaluate());
                                    }
                                    catch
                                    {
                                        anual += 0;
                                    }
                                    // Cambiamos el valor
                                    mes = 13;
                                }
                            } while (!(mes == 13));
                        }
                        // Nombre
                        excelRange = excelWorksheet.Cells["A" + (filaSiguiente) + ""];
                        excelRange.Value = rptLibroConsultaListaMatrizIndicadorResultado.Nombre;
                        excelHelper.ExcelTablaParrafo(ref excelRange, false, 8);
                        // Resumen
                        excelRange = excelWorksheet.Cells["B" + (filaSiguiente) + ""];
                        excelRange.Value = rptLibroConsultaListaMatrizIndicadorResultado.ResumenNarrativo;
                        excelHelper.ExcelTablaParrafo(ref excelRange, false, 8);
                        // Nombre del Indicador
                        excelRange = excelWorksheet.Cells["C" + (filaSiguiente) + ""];
                        excelRange.Value = rptLibroConsultaListaMatrizIndicadorResultado.NombreIndicador;
                        excelHelper.ExcelTablaParrafo(ref excelRange, false, 8);
                        // Formula
                        excelRange = excelWorksheet.Cells["D" + (filaSiguiente) + ""];
                        excelRange.Value = rptLibroConsultaListaMatrizIndicadorResultado.Formula;
                        excelHelper.ExcelTablaParrafo(ref excelRange, false, 8);
                        // Linea Base
                        excelRange = excelWorksheet.Cells["E" + (filaSiguiente) + ""];
                        excelRange.Value = rptLibroConsultaListaMatrizIndicadorResultado.LineaBase;
                        excelHelper.ExcelTablaParrafo(ref excelRange, false, 8);
                        // Frecuencia
                        excelRange = excelWorksheet.Cells["F" + (filaSiguiente) + ""];
                        excelRange.Value = rptLibroConsultaListaMatrizIndicadorResultado.FrecuenciaMedicion;
                        excelHelper.ExcelTablaParrafo(ref excelRange, false, 8);
                        // Unidad de Medida
                        excelRange = excelWorksheet.Cells["G" + (filaSiguiente) + ""];
                        excelRange.Value = rptLibroConsultaListaMatrizIndicadorResultado.UnidadMedida;
                        excelHelper.ExcelTablaParrafo(ref excelRange, false, 8);
                        // Medio Verificacion
                        excelRange = excelWorksheet.Cells["H" + (filaSiguiente) + ""];
                        excelRange.Value = rptLibroConsultaListaMatrizIndicadorResultado.MedioVerificacion;
                        excelHelper.ExcelTablaParrafo(ref excelRange, false, 8);
                        // Meta
                        excelRange = excelWorksheet.Cells["I" + (filaSiguiente) + ""];
                        excelRange.Value = rptLibroConsultaListaMatrizIndicadorResultado.Meta;
                        excelHelper.ExcelTablaParrafo(ref excelRange, false, 8);
                        // Enero a Diciembre Depende con el ID de MES
                        for (int x = 1; x <= periodoId; x++)
                        {
                            excelRange = excelWorksheet.Cells[obtenerColumnaJaP(x) + (filaSiguiente) + ""];
                            // I TRIM
                            if (x == 1)
                            {
                                excelRange.Value = string.Format("{0:n2}", iTrim);
                            }
                            // II TRIM
                            if (x == 2)
                            {
                                excelRange.Value = string.Format("{0:n2}", iiTrim);
                            }
                            // I SEM
                            if (x == 3)
                            {
                                excelRange.Value = string.Format("{0:n2}", iSem);
                            }
                            // III TRIM
                            if (x == 4)
                            {
                                excelRange.Value = string.Format("{0:n2}", iiiTrim);
                            }
                            // IV TRIM
                            if (x == 5)
                            {
                                excelRange.Value = string.Format("{0:n2}", ivTrim);
                            }
                            // II SEM
                            if (x == 6)
                            {
                                excelRange.Value = string.Format("{0:n2}", iiSem);
                            }
                            // ANUAL
                            if (x == 7)
                            {
                                excelRange.Value = string.Format("{0:n2}", anual);
                            }

                            excelHelper.ExcelTablaParrafo(ref excelRange, false, 8);
                        }
                        // Contador
                        filaSiguiente++;
                    }
                    
                    Session["DescargarExcel_ReporteSID"] = excelPackage.GetAsByteArray();
                }

                return Json("Ha encontrado el reporte", JsonRequestBehavior.AllowGet);
            }
            else
            {
                throw new Exception("No hay los reportes.");
            }
        }

        public ActionResult DescargarExcel()
        {

            if (Session["DescargarExcel_ReporteSID"] != null)
            {
                byte[] data = Session["DescargarExcel_ReporteSID"] as byte[];
                return File(data, "application/octet-stream", "ReporteSID.xlsx");
            }
            else
            {
                return new EmptyResult();
            }
        }

        private string obtenerColumnaJaP(int periodoId)
        {
            switch (periodoId)
            {
                case 1:
                    return "J";
                case 2:
                    return "K";
                case 3:
                    return "L";
                case 4:
                    return "M";
                case 5:
                    return "N";
                case 6:
                    return "O";
                case 7:
                    return "P";
                default:
                    return "J";
            }
        }
    }
}