using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class onlineOrderModelEdited3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AmountBeforeShipping",
                table: "OnlineOrder",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliverySlots",
                table: "OnlineOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "OnlineOrder",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderStatus",
                table: "OnlineOrder",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountBeforeShipping",
                table: "OnlineOrder");

            migrationBuilder.DropColumn(
                name: "DeliverySlots",
                table: "OnlineOrder");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "OnlineOrder");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "OnlineOrder");
        }
    }
}
