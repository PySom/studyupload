using Microsoft.EntityFrameworkCore.Migrations;

namespace StudyMATEUpload.Migrations
{
    public partial class ModTestStart1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TestEnded",
                table: "UserTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TestStarted",
                table: "UserTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VideoEnded",
                table: "UserTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VideoStarted",
                table: "UserTests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestEnded",
                table: "UserTests");

            migrationBuilder.DropColumn(
                name: "TestStarted",
                table: "UserTests");

            migrationBuilder.DropColumn(
                name: "VideoEnded",
                table: "UserTests");

            migrationBuilder.DropColumn(
                name: "VideoStarted",
                table: "UserTests");
        }
    }
}
