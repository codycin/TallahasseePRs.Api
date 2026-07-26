using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TallahasseePRs.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileToConversationParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_Users_UserId",
                table: "ConversationParticipants");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_Profiles_UserId",
                table: "ConversationParticipants",
                column: "UserId",
                principalTable: "Profiles",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_Profiles_UserId",
                table: "ConversationParticipants");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_Users_UserId",
                table: "ConversationParticipants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
