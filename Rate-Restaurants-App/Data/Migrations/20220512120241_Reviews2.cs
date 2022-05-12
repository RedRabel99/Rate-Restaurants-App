using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rate_Restaurants_App.Data.Migrations
{
    public partial class Reviews2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Review",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Review_AuthorId",
                table: "Review",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_AspNetUsers_AuthorId",
                table: "Review",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_AspNetUsers_AuthorId",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_AuthorId",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Review");
        }
    }
}
