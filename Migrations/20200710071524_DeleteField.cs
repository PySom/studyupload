using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StudyMATEUpload.Migrations
{
    public partial class DeleteField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFeedbacks");

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Videos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "UserVideos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "UserTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "UserSubscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "UserQuizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "UserPreferences",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "UserCourses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "UserAwards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Tests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Subscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Referrals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Quizes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Preferences",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Options",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Feedbacks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Feedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Courses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Awards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Users_UserId",
                table: "Feedbacks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Users_UserId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "UserVideos");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "UserTests");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "UserQuizzes");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "UserPreferences");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "UserCourses");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "UserAwards");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Quizes");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Preferences");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Awards");

            migrationBuilder.CreateTable(
                name: "UserFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FeedbackId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFeedbacks_Feedbacks_FeedbackId",
                        column: x => x.FeedbackId,
                        principalTable: "Feedbacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFeedbacks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFeedbacks_FeedbackId",
                table: "UserFeedbacks",
                column: "FeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFeedbacks_UserId",
                table: "UserFeedbacks",
                column: "UserId");
        }
    }
}
