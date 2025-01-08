using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class invontoryUpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Customer_CustomerId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Order_InvoiceId",
                table: "OrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Order",
                table: "Order");

            migrationBuilder.RenameTable(
                name: "Order",
                newName: "Invoice");

            migrationBuilder.RenameIndex(
                name: "IX_Order_CustomerId",
                table: "Invoice",
                newName: "IX_Invoice_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuantityInStock = table.Column<int>(type: "int", nullable: true),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OtherShopsPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DifferencePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaximumDiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BuyingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Create_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Modified_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
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
                column: "ProductId",
                unique: true,
                filter: "[ProductId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Customer_CustomerId",
                table: "Invoice",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Invoice_InvoiceId",
                table: "OrderItem",
                column: "InvoiceId",
                principalTable: "Invoice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Customer_CustomerId",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Invoice_InvoiceId",
                table: "OrderItem");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice");

            migrationBuilder.RenameTable(
                name: "Invoice",
                newName: "Order");

            migrationBuilder.RenameIndex(
                name: "IX_Invoice_CustomerId",
                table: "Order",
                newName: "IX_Order_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Order",
                table: "Order",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Customer_CustomerId",
                table: "Order",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Order_InvoiceId",
                table: "OrderItem",
                column: "InvoiceId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
