using Microsoft.EntityFrameworkCore.Migrations;

namespace StudyMATEUpload.Migrations
{
    public partial class UpdateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Level",
                table: "Users",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "QuizId",
                table: "Feedbacks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VideoId",
                table: "Feedbacks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Awards",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_QuizId",
                table: "Feedbacks",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_VideoId",
                table: "Feedbacks",
                column: "VideoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Quizes_QuizId",
                table: "Feedbacks",
                column: "QuizId",
                principalTable: "Quizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Videos_VideoId",
                table: "Feedbacks",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Quizes_QuizId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Videos_VideoId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_QuizId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_VideoId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "QUizId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "QuizId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Awards");
        }
    }
}
