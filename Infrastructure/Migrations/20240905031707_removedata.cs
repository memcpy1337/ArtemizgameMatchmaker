using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removedata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropIndex(
                name: "IX_UserToMatches_MatchId",
                table: "UserToMatches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_Id",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserToMatches");

            migrationBuilder.CreateIndex(
                name: "IX_UserToMatches_MatchId",
                table: "UserToMatches",
                column: "MatchId")
                .Annotation("Npgsql:IndexInclude", new[] { "IsConnected" });

            migrationBuilder.CreateIndex(
                name: "IX_UserToMatches_Ticket",
                table: "UserToMatches",
                column: "Ticket");

            migrationBuilder.CreateIndex(
                name: "IX_UserToMatches_UserId",
                table: "UserToMatches",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Id",
                table: "Matches",
                column: "Id")
                .Annotation("Npgsql:IndexInclude", new[] { "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserToMatches_MatchId",
                table: "UserToMatches");

            migrationBuilder.DropIndex(
                name: "IX_UserToMatches_Ticket",
                table: "UserToMatches");

            migrationBuilder.DropIndex(
                name: "IX_UserToMatches_UserId",
                table: "UserToMatches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_Id",
                table: "Matches");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserToMatches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    MatchId = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsReady = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servers_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserToMatches_MatchId",
                table: "UserToMatches",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Id",
                table: "Matches",
                column: "Id")
                .Annotation("Npgsql:IndexInclude", new[] { "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Servers_MatchId",
                table: "Servers",
                column: "MatchId");
        }
    }
}
