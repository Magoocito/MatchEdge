using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchEdge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTrendPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyPicks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TrendResultId = table.Column<int>(type: "INTEGER", nullable: false),
                    Team = table.Column<string>(type: "TEXT", nullable: false),
                    Market = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Score = table.Column<double>(type: "REAL", precision: 10, scale: 4, nullable: false),
                    Classification = table.Column<string>(type: "TEXT", nullable: false),
                    Edge = table.Column<double>(type: "REAL", precision: 10, scale: 4, nullable: false),
                    Odds = table.Column<double>(type: "REAL", precision: 10, scale: 4, nullable: false),
                    HitRate = table.Column<double>(type: "REAL", nullable: false),
                    SampleSize = table.Column<int>(type: "INTEGER", nullable: false),
                    StakeUnits = table.Column<double>(type: "REAL", precision: 10, scale: 4, nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    PickOutcomeId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyPicks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PickOutcomes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DailyPickId = table.Column<int>(type: "INTEGER", nullable: false),
                    MatchDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Team = table.Column<string>(type: "TEXT", nullable: false),
                    Market = table.Column<string>(type: "TEXT", nullable: false),
                    Odds = table.Column<double>(type: "REAL", precision: 10, scale: 4, nullable: false),
                    Won = table.Column<bool>(type: "INTEGER", nullable: false),
                    Profit = table.Column<double>(type: "REAL", precision: 10, scale: 4, nullable: false),
                    ProfitUnits = table.Column<double>(type: "REAL", precision: 10, scale: 4, nullable: false),
                    BalanceAfter = table.Column<double>(type: "REAL", precision: 10, scale: 4, nullable: false),
                    ResultDetail = table.Column<string>(type: "TEXT", nullable: false),
                    EvaluatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickOutcomes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrendResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScrapedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FixtureSlug = table.Column<string>(type: "TEXT", nullable: false),
                    FixtureApid = table.Column<long>(type: "INTEGER", nullable: false),
                    HomeTeam = table.Column<string>(type: "TEXT", nullable: false),
                    AwayTeam = table.Column<string>(type: "TEXT", nullable: false),
                    League = table.Column<string>(type: "TEXT", nullable: false),
                    Team = table.Column<string>(type: "TEXT", nullable: false),
                    TeamSlug = table.Column<string>(type: "TEXT", nullable: false),
                    Market = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Venue = table.Column<string>(type: "TEXT", nullable: false),
                    HitCount = table.Column<string>(type: "TEXT", nullable: false),
                    HitNumerator = table.Column<int>(type: "INTEGER", nullable: false),
                    HitDenominator = table.Column<int>(type: "INTEGER", nullable: false),
                    HitRate = table.Column<double>(type: "REAL", nullable: false),
                    OppHitRate = table.Column<int>(type: "INTEGER", nullable: false),
                    AvgValue = table.Column<double>(type: "REAL", nullable: false),
                    RatePercentage = table.Column<double>(type: "REAL", nullable: false),
                    Odds = table.Column<double>(type: "REAL", precision: 10, scale: 4, nullable: false),
                    Fixture = table.Column<string>(type: "TEXT", nullable: false),
                    Score = table.Column<double>(type: "REAL", precision: 10, scale: 4, nullable: false),
                    Classification = table.Column<string>(type: "TEXT", nullable: false),
                    Edge = table.Column<double>(type: "REAL", precision: 10, scale: 4, nullable: false),
                    IsSelected = table.Column<bool>(type: "INTEGER", nullable: false),
                    DailyPickId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrendResults", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyPicks_Date",
                table: "DailyPicks",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPicks_Date_Team_Market",
                table: "DailyPicks",
                columns: new[] { "Date", "Team", "Market" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyPicks_Status",
                table: "DailyPicks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPicks_TrendResultId",
                table: "DailyPicks",
                column: "TrendResultId");

            migrationBuilder.CreateIndex(
                name: "IX_PickOutcomes_DailyPickId",
                table: "PickOutcomes",
                column: "DailyPickId");

            migrationBuilder.CreateIndex(
                name: "IX_PickOutcomes_MatchDate",
                table: "PickOutcomes",
                column: "MatchDate");

            migrationBuilder.CreateIndex(
                name: "IX_PickOutcomes_Won",
                table: "PickOutcomes",
                column: "Won");

            migrationBuilder.CreateIndex(
                name: "IX_TrendResults_Classification",
                table: "TrendResults",
                column: "Classification");

            migrationBuilder.CreateIndex(
                name: "IX_TrendResults_FixtureApid",
                table: "TrendResults",
                column: "FixtureApid");

            migrationBuilder.CreateIndex(
                name: "IX_TrendResults_FixtureApid_Team_Market",
                table: "TrendResults",
                columns: new[] { "FixtureApid", "Team", "Market" });

            migrationBuilder.CreateIndex(
                name: "IX_TrendResults_IsSelected",
                table: "TrendResults",
                column: "IsSelected");

            migrationBuilder.CreateIndex(
                name: "IX_TrendResults_ScrapedAt",
                table: "TrendResults",
                column: "ScrapedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyPicks");

            migrationBuilder.DropTable(
                name: "PickOutcomes");

            migrationBuilder.DropTable(
                name: "TrendResults");
        }
    }
}
