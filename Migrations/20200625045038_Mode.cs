using Microsoft.EntityFrameworkCore.Migrations;

namespace StudyMATEUpload.Migrations
{
    public partial class Mode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Mode",
                table: "UserQuizzes",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mode",
                table: "UserQuizzes");
        }
    }
}
