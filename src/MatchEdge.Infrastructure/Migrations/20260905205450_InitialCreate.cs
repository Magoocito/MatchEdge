using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchEdge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoricalOdds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SourceMatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    MatchDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TournamentId = table.Column<int>(type: "INTEGER", nullable: false),
                    Round = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    HomeTeamName = table.Column<string>(type: "TEXT", nullable: false),
                    AwayTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    AwayTeamName = table.Column<string>(type: "TEXT", nullable: false),
                    HomeWinOdds = table.Column<double>(type: "REAL", precision: 10, scale: 4, nullable: false),
                    DrawOdds = table.Column<double>(type: "REAL", precision: 10, scale: 4, nullable: false),
                    AwayWinOdds = table.Column<double>(type: "REAL", precision: 10, scale: 4, nullable: false),
                    SofaScoreEventId = table.Column<int>(type: "INTEGER", nullable: true),
                    SofaScoreHomeTeamId = table.Column<int>(type: "INTEGER", nullable: true),
                    SofaScoreAwayTeamId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MappedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalOdds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MatchMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    SourceMatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    SofaScoreEventId = table.Column<int>(type: "INTEGER", nullable: false),
                    MatchDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MatchConfidence = table.Column<double>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    SourceTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceTeamName = table.Column<string>(type: "TEXT", nullable: false),
                    SofaScoreTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    SofaScoreTeamName = table.Column<string>(type: "TEXT", nullable: false),
                    Confidence = table.Column<double>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMappings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalOdds_MatchDate_HomeTeamId_AwayTeamId",
                table: "HistoricalOdds",
                columns: new[] { "MatchDate", "HomeTeamId", "AwayTeamId" });

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalOdds_SofaScoreEventId",
                table: "HistoricalOdds",
                column: "SofaScoreEventId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalOdds_Source_SourceMatchId",
                table: "HistoricalOdds",
                columns: new[] { "Source", "SourceMatchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchMappings_SofaScoreEventId",
                table: "MatchMappings",
                column: "SofaScoreEventId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchMappings_Source_SourceMatchId",
                table: "MatchMappings",
                columns: new[] { "Source", "SourceMatchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMappings_SofaScoreTeamId",
                table: "TeamMappings",
                column: "SofaScoreTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMappings_Source_SourceTeamId",
                table: "TeamMappings",
                columns: new[] { "Source", "SourceTeamId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricalOdds");

            migrationBuilder.DropTable(
                name: "MatchMappings");

            migrationBuilder.DropTable(
                name: "TeamMappings");
        }
    }
}
