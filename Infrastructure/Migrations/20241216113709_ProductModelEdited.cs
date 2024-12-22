using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductModelEdited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MyPrice",
                table: "Product",
                newName: "SellingPrice");

            migrationBuilder.RenameColumn(
                name: "FinalPrice",
                table: "Product",
                newName: "BuyingPrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SellingPrice",
                table: "Product",
                newName: "MyPrice");

            migrationBuilder.RenameColumn(
                name: "BuyingPrice",
                table: "Product",
                newName: "FinalPrice");
        }
    }
}
