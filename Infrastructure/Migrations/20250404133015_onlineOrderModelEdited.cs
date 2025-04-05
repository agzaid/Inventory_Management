using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class onlineOrderModelEdited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductsOnlyAmount",
                table: "OnlineOrder");

            migrationBuilder.AddColumn<string>(
                name: "IndividualProductsPrice",
                table: "OnlineOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IndividualProductsQuatity",
                table: "OnlineOrder",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndividualProductsPrice",
                table: "OnlineOrder");

            migrationBuilder.DropColumn(
                name: "IndividualProductsQuatity",
                table: "OnlineOrder");

            migrationBuilder.AddColumn<decimal>(
                name: "ProductsOnlyAmount",
                table: "OnlineOrder",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
