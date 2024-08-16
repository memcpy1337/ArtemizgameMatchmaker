using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class @ref : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Regime = table.Column<int>(type: "integer", nullable: false),
                    OwnerUserId = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateFinish = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Elo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IsReady = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MatchId = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "UserToMatches",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    MatchId = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Ticket = table.Column<string>(type: "text", nullable: false),
                    UserType = table.Column<int>(type: "integer", nullable: false),
                    UserIp = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsConnected = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToMatches", x => new { x.UserId, x.MatchId });
                    table.ForeignKey(
                        name: "FK_UserToMatches_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserToMatches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Id",
                table: "Matches",
                column: "Id")
                .Annotation("Npgsql:IndexInclude", new[] { "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Servers_MatchId",
                table: "Servers",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id",
                table: "Users",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserToMatches_MatchId",
                table: "UserToMatches",
                column: "MatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropTable(
                name: "UserToMatches");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
