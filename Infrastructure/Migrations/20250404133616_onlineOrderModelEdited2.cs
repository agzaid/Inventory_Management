using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class onlineOrderModelEdited2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IndividualProductsQuatity",
                table: "OnlineOrder",
                newName: "IndividualProductsQuatities");

            migrationBuilder.RenameColumn(
                name: "IndividualProductsPrice",
                table: "OnlineOrder",
                newName: "IndividualProductsPrices");

            migrationBuilder.RenameColumn(
                name: "AllProductItems",
                table: "OnlineOrder",
                newName: "IndividualProductsNames");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IndividualProductsQuatities",
                table: "OnlineOrder",
                newName: "IndividualProductsQuatity");

            migrationBuilder.RenameColumn(
                name: "IndividualProductsPrices",
                table: "OnlineOrder",
                newName: "IndividualProductsPrice");

            migrationBuilder.RenameColumn(
                name: "IndividualProductsNames",
                table: "OnlineOrder",
                newName: "AllProductItems");
        }
    }
}
