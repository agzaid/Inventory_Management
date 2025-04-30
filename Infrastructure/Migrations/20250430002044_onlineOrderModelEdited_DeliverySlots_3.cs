using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class onlineOrderModelEdited_DeliverySlots_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserDeliverySlot",
                newName: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDeliverySlot_Customer_CustomerId",
                table: "UserDeliverySlot",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDeliverySlot_Customer_CustomerId",
                table: "UserDeliverySlot");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "UserDeliverySlot",
                newName: "UserId");
        }
    }
}
