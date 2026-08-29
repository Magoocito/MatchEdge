using System.Globalization;
using MatchEdge.Domain.Odds;

namespace MatchEdge.Application.UseCases.OddsImport;

public class CsvOddsParser : ICsvOddsParser
{
    public IReadOnlyList<HistoricalOdds> Parse(string csvContent)
    {
        var results = new List<HistoricalOdds>();
        var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length == 0)
            return results;

        var headerLine = lines[0].Trim().TrimEnd(',');
        var headers = ParseCsvLine(headerLine);

        var matchDateIdx = FindColumn(headers, "MatchDate", "Date", "match_date");
        var tournamentIdIdx = FindColumn(headers, "TournamentId", "Tournament", "tournament_id");
        var roundIdx = FindColumn(headers, "Round", "round");
        var matchIdIdx = FindColumn(headers, "MatchId", "match_id");
        var homeTeamIdIdx = FindColumn(headers, "HomeTeamId", "home_team_id");
        var homeTeamNameIdx = FindColumn(headers, "HomeTeamName", "HomeTeam", "home_team_name", "Home");
        var awayTeamIdIdx = FindColumn(headers, "AwayTeamId", "away_team_id");
        var awayTeamNameIdx = FindColumn(headers, "AwayTeamName", "AwayTeam", "away_team_name", "Away");
        var homeWinIdx = FindColumn(headers, "HomeWinOdds", "HomeWin", "home_win", "H", "1");
        var drawIdx = FindColumn(headers, "DrawOdds", "Draw", "draw", "X");
        var awayWinIdx = FindColumn(headers, "AwayWinOdds", "AwayWin", "away_win", "A", "2");

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim().TrimEnd(',');
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var values = ParseCsvLine(line);
            if (values.Length < headers.Length)
                continue;

            if (!TryParseOdds(values, matchDateIdx, tournamentIdIdx, roundIdx, matchIdIdx,
                    homeTeamIdIdx, homeTeamNameIdx, awayTeamIdIdx, awayTeamNameIdx,
                    homeWinIdx, drawIdx, awayWinIdx, out var odds))
                continue;

            results.Add(odds);
        }

        return results;
    }

    private static int FindColumn(string[] headers, params string[] candidates)
    {
        foreach (var candidate in candidates)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                if (string.Equals(headers[i].Trim(), candidate, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
        }
        return -1;
    }

    private static bool TryParseOdds(
        string[] values, int matchDateIdx, int tournamentIdIdx, int roundIdx, int matchIdIdx,
        int homeTeamIdIdx, int homeTeamNameIdx, int awayTeamIdIdx, int awayTeamNameIdx,
        int homeWinIdx, int drawIdx, int awayWinIdx,
        out HistoricalOdds odds)
    {
        odds = new HistoricalOdds();

        if (matchDateIdx >= 0 && DateTime.TryParse(values[matchDateIdx].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            odds.MatchDate = date;

        if (tournamentIdIdx >= 0 && int.TryParse(values[tournamentIdIdx].Trim(), out var tid))
            odds.TournamentId = tid;

        if (roundIdx >= 0 && int.TryParse(values[roundIdx].Trim(), out var round))
            odds.Round = round;

        if (matchIdIdx >= 0 && int.TryParse(values[matchIdIdx].Trim(), out var mid))
            odds.MatchId = mid;

        if (homeTeamIdIdx >= 0 && int.TryParse(values[homeTeamIdIdx].Trim(), out var htid))
            odds.HomeTeamId = htid;

        if (homeTeamNameIdx >= 0)
            odds.HomeTeamName = values[homeTeamNameIdx].Trim();

        if (awayTeamIdIdx >= 0 && int.TryParse(values[awayTeamIdIdx].Trim(), out var atid))
            odds.AwayTeamId = atid;

        if (awayTeamNameIdx >= 0)
            odds.AwayTeamName = values[awayTeamNameIdx].Trim();

        if (homeWinIdx >= 0 && double.TryParse(values[homeWinIdx].Trim(), CultureInfo.InvariantCulture, out var hw))
            odds.HomeWinOdds = hw;

        if (drawIdx >= 0 && double.TryParse(values[drawIdx].Trim(), CultureInfo.InvariantCulture, out var d))
            odds.DrawOdds = d;

        if (awayWinIdx >= 0 && double.TryParse(values[awayWinIdx].Trim(), CultureInfo.InvariantCulture, out var aw))
            odds.AwayWinOdds = aw;

        return homeWinIdx >= 0 && drawIdx >= 0 && awayWinIdx >= 0
            && odds.HomeWinOdds > 0 && odds.DrawOdds > 0 && odds.AwayWinOdds > 0;
    }

    private static string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        bool inQuotes = false;
        var current = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        result.Add(current.ToString());
        return result.ToArray();
    }
}
