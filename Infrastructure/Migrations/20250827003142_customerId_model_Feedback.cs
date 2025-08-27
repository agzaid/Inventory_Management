using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class customerId_model_Feedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FeedbackId",
                table: "Image",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Feedback",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Image_FeedbackId",
                table: "Image",
                column: "FeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Feedback_FeedbackId",
                table: "Image",
                column: "FeedbackId",
                principalTable: "Feedback",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_Feedback_FeedbackId",
                table: "Image");

            migrationBuilder.DropIndex(
                name: "IX_Image_FeedbackId",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "FeedbackId",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Feedback");
        }
    }
}
