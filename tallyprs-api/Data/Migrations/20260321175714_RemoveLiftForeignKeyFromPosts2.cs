using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TallahasseePRs.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLiftForeignKeyFromPosts2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Lifts_LiftId",
                table: "Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Lifts_LiftId",
                table: "Posts");
        }
    }
}
