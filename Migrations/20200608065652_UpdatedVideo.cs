using Microsoft.EntityFrameworkCore.Migrations;

namespace StudyMATEUpload.Migrations
{
    public partial class UpdatedVideo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Videos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Videos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Videos");
        }
    }
}
