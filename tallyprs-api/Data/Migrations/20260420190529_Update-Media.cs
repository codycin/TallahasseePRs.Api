using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TallahasseePRs.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlaybackContentType",
                table: "Media",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaybackObjectKey",
                table: "Media",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedAt",
                table: "Media",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessingError",
                table: "Media",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessingStartedAt",
                table: "Media",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlaybackContentType",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "PlaybackObjectKey",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ProcessingError",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ProcessingStartedAt",
                table: "Media");
        }
    }
}
