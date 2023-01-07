using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RequisicionesAlmacen.Helpers
{
    public class ExcelHelper
    {
        public void ExcelTitulo(ref ExcelRange excelRange)
        {
            excelRange.Merge = true;
            excelRange.Style.Font.Bold = true;
            excelRange.Style.Font.Size = 12;
            excelRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            excelRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        }

        public void ExcelParrafo(ref ExcelRange excelRange)
        {
            excelRange.Merge = true;
            excelRange.Style.WrapText = true;
            //excelRange.Style.Font.Bold = true;
            excelRange.Style.Font.Size = 8;
            excelRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            excelRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        }

        public void ExcelTablaTitulo(ref ExcelRange excelRange, bool esMerge, int size, bool esCentroHorizontal, bool esCentroVertical)
        {
            excelRange.Merge = esMerge;
            excelRange.Style.WrapText = true;
            excelRange.Style.Font.Bold = true;
            excelRange.Style.Font.Size = size;
            excelRange.Style.HorizontalAlignment = esCentroHorizontal ? OfficeOpenXml.Style.ExcelHorizontalAlignment.Center : OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            excelRange.Style.VerticalAlignment = esCentroVertical ? OfficeOpenXml.Style.ExcelVerticalAlignment.Center : OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
            excelRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        }

        public void ExcelTablaParrafo(ref ExcelRange excelRange, bool esMerge, int size)
        {
            excelRange.Merge = esMerge;
            excelRange.Style.WrapText = true;
            excelRange.Style.Font.Size = size;
            excelRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
            excelRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            excelRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        }
    }
}