using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class onlineOrderModelEdited_DeliverySlots_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "OnlineOrderId",
                table: "UserDeliverySlot",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserDeliverySlot_OnlineOrderId",
                table: "UserDeliverySlot",
                column: "OnlineOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDeliverySlot_OnlineOrder_OnlineOrderId",
                table: "UserDeliverySlot",
                column: "OnlineOrderId",
                principalTable: "OnlineOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDeliverySlot_OnlineOrder_OnlineOrderId",
                table: "UserDeliverySlot");

            migrationBuilder.DropIndex(
                name: "IX_UserDeliverySlot_OnlineOrderId",
                table: "UserDeliverySlot");

            migrationBuilder.DropColumn(
                name: "OnlineOrderId",
                table: "UserDeliverySlot");

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
    }
}
