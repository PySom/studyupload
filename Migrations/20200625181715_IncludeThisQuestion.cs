using Microsoft.EntityFrameworkCore.Migrations;

namespace StudyMATEUpload.Migrations
{
    public partial class IncludeThisQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IncludeThis",
                table: "Quizes",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludeThis",
                table: "Quizes");
        }
    }
}
