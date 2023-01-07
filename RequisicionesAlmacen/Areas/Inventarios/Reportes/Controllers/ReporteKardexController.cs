using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using RequisicionesAlmacen.Areas.Inventarios.Reportes.Models.ViewModel;
using RequisicionesAlmacen.Controllers;
using RequisicionesAlmacen.Helpers;
using RequisicionesAlmacenBL.Entities;
using RequisicionesAlmacenBL.Models.Mapeos;
using RequisicionesAlmacenBL.Services;
using RequisicionesAlmacenBL.Services.Almacen;
using RequisicionesAlmacenBL.Services.SAACG;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RequisicionesAlmacen.Areas.Inventarios.Reportes.Controllers
{
    [Authenticated(nodoMenuId = MenuPrincipalMapeo.ID.KARDEX)]
    public class ReporteKardexController : BaseController<ReporteKardexViewModel, ReporteKardexViewModel>
    {
        public ActionResult Index()
        {
            // Creamos el objecto nuevo
            ReporteKardexViewModel reporteViewModel = new ReporteKardexViewModel();

            //Agregamos todos los datos necesarios para el funcionamiento de la ficha
            //como son los Listados para combos, tablas, arboles.
            GetDatosFicha(ref reporteViewModel);

            ViewBag.WebReport = null;

            //Retornamos la vista junto con su Objeto Modelo
            return View("ReporteKardex", reporteViewModel);
        }
        
        public override ActionResult Editar(int id)
        {
            throw new NotImplementedException();
        }

        public override JsonResult Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        [JsonException]
        public override JsonResult Guardar(ReporteKardexViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public override ActionResult Listar()
        {
            throw new NotImplementedException();
        }

        public override ActionResult Nuevo()
        {
            throw new NotImplementedException();
        }

        protected override void GetDatosFicha(ref ReporteKardexViewModel viewModel)
        {
            //Datos de Almacén
            viewModel.ListAlmacenes = new AlmacenService().BuscaTodos();
            viewModel.ListProductos = new ProductoService().BuscaActivos();
            viewModel.ListTiposMovimiento = new ControlMaestroService().BuscaControl("TipoInventarioMovimiento");

            //Datos Financiamiento
            viewModel.ListUnidadesAdministrativas = new DependenciaService().BuscaTodos();
            viewModel.ListProyectos = new ProyectoService().BuscaTodos();
            viewModel.ListFuentesFinanciamiento = new RamoService().BuscaTodos();
            viewModel.ListPolizas = new PolizaService().BuscaTodos();
        }

        [JsonException]
        public ActionResult BuscarReporte(ARrptKardex reporte)
        {
            string nombreEnte = "El ente de aqui de Jalisco";
            string estado = "Jalisco";
            string tituloReporte = "Kardex";

            List<ARrptKardex> registrosReporte = new ReportesService().GetRptKardex(reporte.FechaInicio,
                                                                                    reporte.FechaFin,
                                                                                    reporte.TipoMvtoId,
                                                                                    reporte.MvtoId,
                                                                                    reporte.PolizaId,
                                                                                    reporte.AlmacenId,
                                                                                    reporte.ProductoId,
                                                                                    reporte.UnidadAdministrativaId,
                                                                                    reporte.ProyectoId,
                                                                                    reporte.FuenteFinanciamientoId,
                                                                                    SessionHelper.GetUsuario().UsuarioId);

            if (registrosReporte.Count > 0)
            {
                List<string> columnas = new List<string>
                {
                    "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U"
                };

                List<string> titulosColumnas = new List<string>
                {
                    "Almacén ID",
                    "Nombre Almacén",
                    "Fecha Mov.",
                    "Tipo Mov.",
                    "Mov.ID",
                    "Descripción Movimiento",
                    "Póliza",
                    "Producto ID",
                    "Descripción",
                    "UA",
                    "Proyecto",
                    "FF",
                    "Unidad Medida",
                    "Entrada",
                    "Salida",
                    "Existencia",
                    "Costo Unitario",
                    "Total",
                    "Costo Promedio"
                };

                List<string> propiedades = new List<string>
                {
                    "AlmacenId",
                    "Almacen",
                    "Fecha",
                    "TipoMovimiento",
                    "ReferenciaMovtoId",
                    "MotivoMovto",
                    "Poliza",
                    "ProductoId",
                    "Producto",
                    "UA",
                    "Proyecto",
                    "FF",
                    "UM",
                    "Entrada",
                    "Salida",
                    "Existencia",
                    "CostoUnitario",
                    "Total",
                    "CostoPromedio"
                };

                FileInfo logotipo = new ArchivoService().GetImagenLogotipo("saacg-net.png");

                using (var excelPackage = new ExcelPackage(new MemoryStream()))
                {
                    ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Kárdex");
                    ExcelHelper excelHelper = new ExcelHelper();
                    ExcelRange excelRange = null;

                    excelWorksheet.PrinterSettings.Orientation = eOrientation.Landscape;
                    excelWorksheet.PrinterSettings.PaperSize = ePaperSize.Legal;

                    //Nombre del Ente Público
                    excelRange = excelWorksheet.Cells["A1:U1"];
                    excelRange.Value = nombreEnte;
                    excelHelper.ExcelTitulo(ref excelRange);

                    //Estado
                    excelRange = excelWorksheet.Cells["A2:U2"];
                    excelRange.Value = estado;
                    excelHelper.ExcelTitulo(ref excelRange);

                    // Variables de los Indicadores
                    excelRange = excelWorksheet.Cells["A3:U3"];
                    excelRange.Value = tituloReporte;
                    excelHelper.ExcelTitulo(ref excelRange);

                    //Espacio
                    excelRange = excelWorksheet.Cells["A4:U4"];
                    excelHelper.ExcelTitulo(ref excelRange);

                    //Usuario
                    excelRange = excelWorksheet.Cells["B5"];
                    excelRange.Value = "Usr: ";
                    excelRange = excelWorksheet.Cells["C5"];
                    excelRange.Value = registrosReporte[0].Usuario;

                    //Nombre Reporte
                    excelRange = excelWorksheet.Cells["B6"];
                    excelRange.Value = "Rep: ";
                    excelRange = excelWorksheet.Cells["C6"];
                    excelRange.Value = registrosReporte[0].Reporte;

                    //Fecha Impresión
                    excelRange = excelWorksheet.Cells["S5"];
                    excelRange.Value = "Fecha y";
                    excelRange = excelWorksheet.Cells["T5"];
                    excelRange.Value = registrosReporte[0].FechaImpresion;

                    //Hora Impresión
                    excelRange = excelWorksheet.Cells["S6"];
                    excelRange.Value = "hora de Impresión";
                    excelRange = excelWorksheet.Cells["T6"];
                    excelRange.Value = registrosReporte[0].HoraImpresion;

                    //Logotipo
                    if (logotipo != null)
                    {
                        ExcelPicture excelLogotipo = excelWorksheet.Drawings.AddPicture("SAACG.NET", logotipo);

                        excelLogotipo.SetSize(90, 60);
                        excelLogotipo.SetPosition(0, 30, 0, 30);
                    }                    

                    //Agregar Titulos de Columnas
                    for (int i = 1; i < columnas.Count - 1; i++ )
                    {
                        excelRange = excelWorksheet.Cells[columnas[i] + 7];
                        excelRange.Value = titulosColumnas[i-1];
                    }

                    //Border
                    excelRange = excelWorksheet.Cells["A8:U8"];
                    excelRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    //Variable contador iniciar rows
                    int iniciarEn = 9;

                    //Agrgamos cada fila del reporte
                    foreach (ARrptKardex registro in registrosReporte)
                    {
                        for (int i = 0; i < propiedades.Count; i++)
                        {
                            excelRange = excelWorksheet.Cells[columnas[i+1] + iniciarEn];
                            excelRange.Value = registro.GetType().GetProperty(propiedades[i]).GetValue(registro, null);
                        }

                        iniciarEn++;
                    }

                    Session["DescargarExcel_ReporteKardex"] = excelPackage.GetAsByteArray();
                }

                return Json(registrosReporte.Count, JsonRequestBehavior.AllowGet);
            }
            else
            {
                throw new Exception("El reporte no contiene registros que mostrar.");
            }
        }

        [JsonException]
        public ActionResult DescargarExcel()
        {
            Object excel = Session["DescargarExcel_ReporteKardex"];

            if (excel != null)
            {
                return File(excel as byte[], "application/octet-stream", "ReporteKardex.xlsx");
            }
            else
            {
                throw new Exception("No se pudo generar el Reporte.");
            }
        }
    }
}