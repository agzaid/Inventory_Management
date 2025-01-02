using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class invoiceModelAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Order_OrderId",
                table: "OrderItem");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "OrderItem",
                newName: "InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItem",
                newName: "IX_OrderItem_InvoiceId");

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Order_InvoiceId",
                table: "OrderItem",
                column: "InvoiceId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Order_InvoiceId",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "InvoiceId",
                table: "OrderItem",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_InvoiceId",
                table: "OrderItem",
                newName: "IX_OrderItem_OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Order_OrderId",
                table: "OrderItem",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
