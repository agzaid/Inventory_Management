using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class shippingFrieght : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.CreateTable(
                name: "ShippingFrieght",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: true),
                    Create_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Modified_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingFrieght", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingFrieght");

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuyingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Create_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DifferencePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    MaximumDiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Modified_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OtherShopsPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    QuantityInStock = table.Column<int>(type: "int", nullable: true),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ProductId",
                table: "Inventory",
                column: "ProductId");
        }
    }
}
