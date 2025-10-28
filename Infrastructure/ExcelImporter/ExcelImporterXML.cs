using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExcelImporter
{
    public class ExcelImporterXML
    {
        public List<Product> ReadProducts(Stream excelStream)
        {
            var products = new List<Product>();

            using (SpreadsheetDocument document = SpreadsheetDocument.Open(excelStream, false))
            {
                var workbookPart = document.WorkbookPart;
                var sheet = workbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                var rows = sheetData.Elements<Row>().ToList();
                if (rows.Count <= 1)
                    return products;

                for (int i = 1; i < rows.Count; i++) // Skip header row
                {
                    var row = rows[i];
                    var cells = row.Elements<Cell>().ToList();

                    var product = new Product
                    {
                        ProductName = GetCellText(cells.ElementAtOrDefault(0), workbookPart),
                        Description = GetCellText(cells.ElementAtOrDefault(1), workbookPart),
                        SellingPrice = decimal.TryParse(GetCellText(cells.ElementAtOrDefault(3), workbookPart), out var price) ? price : 0,
                        StockQuantity = decimal.TryParse(GetCellText(cells.ElementAtOrDefault(4), workbookPart), out var qty) ? qty : 0,
                        ProductExpiryDate = DateTime.TryParse(GetCellText(cells.ElementAtOrDefault(5), workbookPart), out var exp)
                            ? DateOnly.FromDateTime(exp)
                            : DateOnly.MinValue,
                        Create_Date = DateTime.TryParse(GetCellText(cells.ElementAtOrDefault(6), workbookPart), out var created)
                            ? created
                            : DateTime.UtcNow
                    };

                    products.Add(product);
                }
            }

            return products;
        }

        private string GetCellText(Cell cell, WorkbookPart workbookPart)
        {
            if (cell == null)
                return string.Empty;

            string value = cell.InnerText;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                var sharedStringTable =
                    workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>();

                return sharedStringTable.ElementAt(int.Parse(value)).InnerText;
            }

            return value;
        }
    }
}
