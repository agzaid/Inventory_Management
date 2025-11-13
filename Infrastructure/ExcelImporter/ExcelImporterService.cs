using Domain.Entities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Infrastructure.ExcelImporter
{
    public class ExcelImporterService
    {
        public List<Product> ReadProducts(Stream excelStream)
        {
            var products = new List<Product>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(excelStream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return products;

                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    var idText = worksheet.Cells[row, 1].Text?.Trim();
                    if (!int.TryParse(idText, out int id))
                        continue; // Skip row if ID is not a number

                    var productName = worksheet.Cells[row, 2].Text?.Trim();
                    if (string.IsNullOrWhiteSpace(productName))
                        continue; // Required field
                    var productNameAr = worksheet.Cells[row, 3].Text?.Trim();
                    if (string.IsNullOrWhiteSpace(productName))
                        continue; // Required field

                    decimal.TryParse(worksheet.Cells[row, 6].Text, out decimal price);
                    decimal.TryParse(worksheet.Cells[row, 7].Text, out decimal othrprice);
                    decimal.TryParse(worksheet.Cells[row, 8].Text, out decimal qty);
                    DateTime.TryParse(worksheet.Cells[row, 9].Text, out DateTime expDate);
                    DateTime.TryParse(worksheet.Cells[row, 10].Text, out DateTime createdDate);

                    var product = new Product
                    {
                        Id = id,
                        ProductName = productName,
                        ProductNameAr = productNameAr,
                        Description = worksheet.Cells[row, 4].Text?.Trim(),
                        SellingPrice = price,
                        OtherShopsPrice = othrprice,
                        StockQuantity = qty,
                        ProductExpiryDate = expDate != DateTime.MinValue
                                            ? DateOnly.FromDateTime(expDate)
                                            : DateOnly.MinValue,
                        Create_Date = createdDate != DateTime.MinValue
                                      ? createdDate
                                      : DateTime.Now
                    };

                    products.Add(product);
                }
            }

            return products;
        }

    }
}
