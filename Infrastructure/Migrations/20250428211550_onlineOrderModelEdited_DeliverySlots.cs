using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class onlineOrderModelEdited_DeliverySlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeliverySlots",
                table: "OnlineOrder",
                newName: "DeliverySlotsAsString");

            migrationBuilder.AddColumn<int>(
                name: "OnlineOrderId",
                table: "DeliverySlot",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliverySlot_OnlineOrderId",
                table: "DeliverySlot",
                column: "OnlineOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliverySlot_OnlineOrder_OnlineOrderId",
                table: "DeliverySlot",
                column: "OnlineOrderId",
                principalTable: "OnlineOrder",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliverySlot_OnlineOrder_OnlineOrderId",
                table: "DeliverySlot");

            migrationBuilder.DropIndex(
                name: "IX_DeliverySlot_OnlineOrderId",
                table: "DeliverySlot");

            migrationBuilder.DropColumn(
                name: "OnlineOrderId",
                table: "DeliverySlot");

            migrationBuilder.RenameColumn(
                name: "DeliverySlotsAsString",
                table: "OnlineOrder",
                newName: "DeliverySlots");
        }
    }
}
