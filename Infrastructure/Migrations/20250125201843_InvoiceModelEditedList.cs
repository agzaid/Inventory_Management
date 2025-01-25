using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InvoiceModelEditedList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Customer_CustomerId",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "OrderItem");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Invoice",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "Invoice",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Customer_CustomerId",
                table: "Invoice",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Customer_CustomerId",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "Invoice");

            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "OrderItem",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Invoice",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Customer_CustomerId",
                table: "Invoice",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
