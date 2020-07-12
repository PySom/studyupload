using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StudyMATEUpload.Migrations
{
    public partial class InitMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level = table.Column<byte>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Duration = table.Column<float>(nullable: false),
                    LearnCourseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Videos_LearnCourses_LearnCourseId",
                        column: x => x.LearnCourseId,
                        principalTable: "LearnCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            

            migrationBuilder.CreateTable(
                name: "UserLearnCourses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    LearnCourseId = table.Column<int>(nullable: false)
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

            

            migrationBuilder.CreateTable(
                name: "UserVideos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateWatched = table.Column<DateTime>(nullable: false),
                    Duration = table.Column<float>(nullable: false),
                    UserCourseId = table.Column<int>(nullable: false),
                    VideoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVideos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserVideos_UserLearnCourses_UserCourseId",
                        column: x => x.UserCourseId,
                        principalTable: "UserLearnCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserVideos_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            

            migrationBuilder.CreateTable(
                name: "UserQuizzes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTaken = table.Column<DateTime>(nullable: false),
                    CorrectOption = table.Column<int>(nullable: false),
                    UserOption = table.Column<int>(nullable: false),
                    UserCourseId = table.Column<int>(nullable: false),
                    QuizId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserQuizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserQuizzes_Quizes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserQuizzes_UserLearnCourses_UserCourseId",
                        column: x => x.UserCourseId,
                        principalTable: "UserLearnCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLearnCourses_LearnCourseId",
                table: "UserLearnCourses",
                column: "LearnCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLearnCourses_UserId",
                table: "UserLearnCourses",
                column: "UserId");

            
            migrationBuilder.CreateIndex(
                name: "IX_UserQuizzes_QuizId",
                table: "UserQuizzes",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizzes_UserCourseId",
                table: "UserQuizzes",
                column: "UserCourseId");

            

            migrationBuilder.CreateIndex(
                name: "IX_UserVideos_UserCourseId",
                table: "UserVideos",
                column: "UserCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVideos_VideoId",
                table: "UserVideos",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_LearnCourseId",
                table: "Videos",
                column: "LearnCourseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Options");

            migrationBuilder.DropTable(
                name: "Referrals");

            migrationBuilder.DropTable(
                name: "UserAwards");

            migrationBuilder.DropTable(
                name: "UserCourses");

            migrationBuilder.DropTable(
                name: "UserFeedbacks");

            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.DropTable(
                name: "UserQuizzes");

            migrationBuilder.DropTable(
                name: "UserSubscriptions");

            migrationBuilder.DropTable(
                name: "UserVideos");

            migrationBuilder.DropTable(
                name: "Awards");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "Preferences");

            migrationBuilder.DropTable(
                name: "Quizes");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "UserLearnCourses");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "LearnCourses");

            migrationBuilder.DropTable(
                name: "Courses");
        }
    }
}
