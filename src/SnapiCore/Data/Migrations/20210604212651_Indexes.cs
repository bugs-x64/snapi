using Microsoft.EntityFrameworkCore.Migrations;

namespace SnapiCore.Migrations
{
    public partial class Indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscribers_FromId",
                table: "Subscribers");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IndexName",
                table: "Users",
                column: "IndexName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_FromId_ToId",
                table: "Subscribers",
                columns: new[] { "FromId", "ToId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_IndexName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Subscribers_FromId_ToId",
                table: "Subscribers");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_FromId",
                table: "Subscribers",
                column: "FromId");
        }
    }
}
