using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StudyMATEUpload.Migrations
{
    public partial class Scared : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateWatched",
                table: "UserFeedbacks");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "UserFeedbacks");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "UserFeedbacks",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "UserFeedbacks");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateWatched",
                table: "UserFeedbacks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<float>(
                name: "Duration",
                table: "UserFeedbacks",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
