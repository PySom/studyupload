using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StudyMATEUpload.Migrations
{
    public partial class RethoughtArch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizes_LearnCourses_LearnCourseId",
                table: "Quizes");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizes_Tests_TestId",
                table: "Quizes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourses_Tests_TestId",
                table: "UserCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizzes_UserCourses_UserCourseId",
                table: "UserQuizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizzes_UserLearnCourses_UserLearnCourseId",
                table: "UserQuizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserVideos_UserLearnCourses_UserCourseId",
                table: "UserVideos");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_LearnCourses_LearnCourseId",
                table: "Videos");

            migrationBuilder.DropTable(
                name: "UserLearnCourses");

            migrationBuilder.DropTable(
                name: "LearnCourses");

            migrationBuilder.DropIndex(
                name: "IX_Videos_LearnCourseId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_UserVideos_UserCourseId",
                table: "UserVideos");

            migrationBuilder.DropIndex(
                name: "IX_UserQuizzes_UserCourseId",
                table: "UserQuizzes");

            migrationBuilder.DropIndex(
                name: "IX_UserQuizzes_UserLearnCourseId",
                table: "UserQuizzes");

            migrationBuilder.DropIndex(
                name: "IX_UserCourses_TestId",
                table: "UserCourses");

            migrationBuilder.DropIndex(
                name: "IX_Quizes_LearnCourseId",
                table: "Quizes");

            migrationBuilder.DropColumn(
                name: "LearnCourseId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "UserCourseId",
                table: "UserVideos");

            migrationBuilder.DropColumn(
                name: "UserCourseId",
                table: "UserQuizzes");

            migrationBuilder.DropColumn(
                name: "UserLearnCourseId",
                table: "UserQuizzes");

            migrationBuilder.DropColumn(
                name: "TestId",
                table: "UserCourses");

            migrationBuilder.DropColumn(
                name: "LearnCourseId",
                table: "Quizes");

            migrationBuilder.AddColumn<int>(
                name: "TestId",
                table: "Videos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserTestId",
                table: "UserVideos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserTestId",
                table: "UserQuizzes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "UserCourses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "StudyType",
                table: "Tests",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AlterColumn<int>(
                name: "TestId",
                table: "Quizes",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "StudyLevel",
                table: "Courses",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "UserTests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsComplete = table.Column<bool>(nullable: false),
                    Score = table.Column<int>(nullable: false),
                    CurrentLevel = table.Column<byte>(nullable: false),
                    FinalLevel = table.Column<byte>(nullable: false),
                    DateTaken = table.Column<DateTime>(nullable: false),
                    UserCourseId = table.Column<int>(nullable: false),
                    TestId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTests_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTests_UserCourses_UserCourseId",
                        column: x => x.UserCourseId,
                        principalTable: "UserCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Videos_TestId",
                table: "Videos",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVideos_UserTestId",
                table: "UserVideos",
                column: "UserTestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizzes_UserTestId",
                table: "UserQuizzes",
                column: "UserTestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCourses_CourseId",
                table: "UserCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTests_TestId",
                table: "UserTests",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTests_UserCourseId",
                table: "UserTests",
                column: "UserCourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizes_Tests_TestId",
                table: "Quizes",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourses_Courses_CourseId",
                table: "UserCourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuizzes_UserTests_UserTestId",
                table: "UserQuizzes",
                column: "UserTestId",
                principalTable: "UserTests",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserVideos_UserTests_UserTestId",
                table: "UserVideos",
                column: "UserTestId",
                principalTable: "UserTests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Tests_TestId",
                table: "Videos",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizes_Tests_TestId",
                table: "Quizes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourses_Courses_CourseId",
                table: "UserCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizzes_UserTests_UserTestId",
                table: "UserQuizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserVideos_UserTests_UserTestId",
                table: "UserVideos");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Tests_TestId",
                table: "Videos");

            migrationBuilder.DropTable(
                name: "UserTests");

            migrationBuilder.DropIndex(
                name: "IX_Videos_TestId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_UserVideos_UserTestId",
                table: "UserVideos");

            migrationBuilder.DropIndex(
                name: "IX_UserQuizzes_UserTestId",
                table: "UserQuizzes");

            migrationBuilder.DropIndex(
                name: "IX_UserCourses_CourseId",
                table: "UserCourses");

            migrationBuilder.DropColumn(
                name: "TestId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "UserTestId",
                table: "UserVideos");

            migrationBuilder.DropColumn(
                name: "UserTestId",
                table: "UserQuizzes");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "UserCourses");

            migrationBuilder.DropColumn(
                name: "StudyType",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "StudyLevel",
                table: "Courses");

            migrationBuilder.AddColumn<int>(
                name: "LearnCourseId",
                table: "Videos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserCourseId",
                table: "UserVideos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserCourseId",
                table: "UserQuizzes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserLearnCourseId",
                table: "UserQuizzes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TestId",
                table: "UserCourses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TestId",
                table: "Quizes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "LearnCourseId",
                table: "Quizes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LearnCourses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudyLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    SubText = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearnCourses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLearnCourses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LearnCourseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLearnCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLearnCourses_LearnCourses_LearnCourseId",
                        column: x => x.LearnCourseId,
                        principalTable: "LearnCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLearnCourses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Videos_LearnCourseId",
                table: "Videos",
                column: "LearnCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVideos_UserCourseId",
                table: "UserVideos",
                column: "UserCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizzes_UserCourseId",
                table: "UserQuizzes",
                column: "UserCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizzes_UserLearnCourseId",
                table: "UserQuizzes",
                column: "UserLearnCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCourses_TestId",
                table: "UserCourses",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizes_LearnCourseId",
                table: "Quizes",
                column: "LearnCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLearnCourses_LearnCourseId",
                table: "UserLearnCourses",
                column: "LearnCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLearnCourses_UserId",
                table: "UserLearnCourses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizes_LearnCourses_LearnCourseId",
                table: "Quizes",
                column: "LearnCourseId",
                principalTable: "LearnCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizes_Tests_TestId",
                table: "Quizes",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourses_Tests_TestId",
                table: "UserCourses",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_UserVideos_UserLearnCourses_UserCourseId",
                table: "UserVideos",
                column: "UserCourseId",
                principalTable: "UserLearnCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_LearnCourses_LearnCourseId",
                table: "Videos",
                column: "LearnCourseId",
                principalTable: "LearnCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
