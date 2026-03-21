using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TallahasseePRs.Api.Migrations
{
    /// <inheritdoc />
    public partial class postupdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Lifts_LiftId",
                table: "Posts");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Lifts_LiftId",
                table: "Posts",
                column: "LiftId",
                principalTable: "Lifts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Lifts_LiftId",
                table: "Posts");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Lifts_LiftId",
                table: "Posts",
                column: "LiftId",
                principalTable: "Lifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
