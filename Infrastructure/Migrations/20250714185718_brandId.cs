using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class brandId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                table: "Image",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Image_BrandId",
                table: "Image",
                column: "BrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Brand_BrandId",
                table: "Image",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_Brand_BrandId",
                table: "Image");

            migrationBuilder.DropIndex(
                name: "IX_Image_BrandId",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Image");
        }
    }
}
