using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SellerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Seller_SupplierId",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "SupplierName",
                table: "Seller",
                newName: "SellerName");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "Product",
                newName: "SellerId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_SupplierId",
                table: "Product",
                newName: "IX_Product_SellerId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Seller",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SellerNameAr",
                table: "Seller",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SellerId",
                table: "Image",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Image_SellerId",
                table: "Image",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Seller_SellerId",
                table: "Image",
                column: "SellerId",
                principalTable: "Seller",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Seller_SellerId",
                table: "Product",
                column: "SellerId",
                principalTable: "Seller",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_Seller_SellerId",
                table: "Image");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Seller_SellerId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Image_SellerId",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Seller");

            migrationBuilder.DropColumn(
                name: "SellerNameAr",
                table: "Seller");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Image");

            migrationBuilder.RenameColumn(
                name: "SellerName",
                table: "Seller",
                newName: "SupplierName");

            migrationBuilder.RenameColumn(
                name: "SellerId",
                table: "Product",
                newName: "SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_SellerId",
                table: "Product",
                newName: "IX_Product_SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Seller_SupplierId",
                table: "Product",
                column: "SupplierId",
                principalTable: "Seller",
                principalColumn: "Id");
        }
    }
}
