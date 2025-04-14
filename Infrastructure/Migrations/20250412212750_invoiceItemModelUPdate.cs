using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class invoiceItemModelUPdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "InvoiceItem",
                newName: "SellingPriceFromProduct");

            migrationBuilder.AddColumn<string>(
                name: "BarcodeFromProduct",
                table: "InvoiceItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BuyingPriceFromProduct",
                table: "InvoiceItem",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DifferencePercentageFromProduct",
                table: "InvoiceItem",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumDiscountPercentageFromProduct",
                table: "InvoiceItem",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherShopsPriceFromProduct",
                table: "InvoiceItem",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceSoldToCustomer",
                table: "InvoiceItem",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ProductExpiryDateFromProduct",
                table: "InvoiceItem",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductTagsFromProduct",
                table: "InvoiceItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StockQuantityFromProduct",
                table: "InvoiceItem",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarcodeFromProduct",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "BuyingPriceFromProduct",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "DifferencePercentageFromProduct",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "MaximumDiscountPercentageFromProduct",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "OtherShopsPriceFromProduct",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "PriceSoldToCustomer",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "ProductExpiryDateFromProduct",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "ProductTagsFromProduct",
                table: "InvoiceItem");

            migrationBuilder.DropColumn(
                name: "StockQuantityFromProduct",
                table: "InvoiceItem");

            migrationBuilder.RenameColumn(
                name: "SellingPriceFromProduct",
                table: "InvoiceItem",
                newName: "Price");
        }
    }
}
