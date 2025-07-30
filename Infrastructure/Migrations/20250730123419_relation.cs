using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OnlineOrder_InvoiceId",
                table: "OnlineOrder",
                column: "InvoiceId",
                unique: true,
                filter: "[InvoiceId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_OnlineOrder_Invoice_InvoiceId",
                table: "OnlineOrder",
                column: "InvoiceId",
                principalTable: "Invoice",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OnlineOrder_Invoice_InvoiceId",
                table: "OnlineOrder");

            migrationBuilder.DropIndex(
                name: "IX_OnlineOrder_InvoiceId",
                table: "OnlineOrder");
        }
    }
}
