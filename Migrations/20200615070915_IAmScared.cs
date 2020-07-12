using Microsoft.EntityFrameworkCore.Migrations;

namespace StudyMATEUpload.Migrations
{
    public partial class IAmScared : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizzes_UserLearnCourses_UserCourseId",
                table: "UserQuizzes");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "UserCourses");

            migrationBuilder.AlterColumn<int>(
                name: "UserCourseId",
                table: "UserQuizzes",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserLearnCourseId",
                table: "UserQuizzes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizzes_UserLearnCourseId",
                table: "UserQuizzes",
                column: "UserLearnCourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuizzes_UserCourses_UserCourseId",
                table: "UserQuizzes",
                column: "UserCourseId",
                principalTable: "UserCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuizzes_UserLearnCourses_UserLearnCourseId",
                table: "UserQuizzes",
                column: "UserLearnCourseId",
                principalTable: "UserLearnCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizzes_UserCourses_UserCourseId",
                table: "UserQuizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizzes_UserLearnCourses_UserLearnCourseId",
                table: "UserQuizzes");

            migrationBuilder.DropIndex(
                name: "IX_UserQuizzes_UserLearnCourseId",
                table: "UserQuizzes");

            migrationBuilder.DropColumn(
                name: "UserLearnCourseId",
                table: "UserQuizzes");

            migrationBuilder.AlterColumn<int>(
                name: "UserCourseId",
                table: "UserQuizzes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Score",
                table: "UserCourses",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuizzes_UserLearnCourses_UserCourseId",
                table: "UserQuizzes",
                column: "UserCourseId",
                principalTable: "UserLearnCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
