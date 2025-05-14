using OfficeOpenXml;

namespace ProbabilityTrades.Common.Extensions;

public static class ListExtensions
{
    public static string ToExcelBase64String<T>(this IEnumerable<T> list, string title)
    {
        using var excelPackage = new ExcelPackage();
        excelPackage.Workbook.Properties.Author = "Uniek Software";
        excelPackage.Workbook.Properties.Title = title;
        excelPackage.Workbook.Properties.Subject = title;
        excelPackage.Workbook.Properties.Created = DateTime.Now;
        var worksheet = excelPackage.Workbook.Worksheets.Add(title);

        var columnHeaders = typeof(T).GetProperties();
        var column = 1;
        foreach (var columnHeader in columnHeaders)
        {
            worksheet.Cells[$"{column.GetColumnName()}1"].Value = columnHeader.Name;
            column++;
        }

        var currentRow = 2;
        foreach (var row in list)
        {
            column = 1;
            foreach (var propertyInfo in columnHeaders)
            {
                var cell = $"{column.GetColumnName()}{currentRow}";
                var value = propertyInfo.GetValue(row, null);
                worksheet.Cells[cell].Value = value;
                worksheet.Cells[cell].Style.Numberformat.Format = GetFormatTypeFromPropertyType(propertyInfo.PropertyType);
                column++;
            }
            currentRow++;
        }

        return Convert.ToBase64String(excelPackage.GetAsByteArray());
    }

    private static string GetFormatTypeFromPropertyType(Type propertyType)
    {
        if(propertyType == typeof(decimal) || propertyType == typeof(decimal?))
            return "0.000000000000";
        else
            return "General";
    }
}