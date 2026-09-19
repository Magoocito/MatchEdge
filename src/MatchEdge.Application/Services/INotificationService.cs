namespace MatchEdge.Application.Services;

public interface INotificationService
{
    Task<bool> SendPickNotificationAsync(PickNotification notification);
    Task<bool> SendPipelineResultAsync(PipelineNotification notification);
    Task<bool> SendDailySummaryAsync(DailySummaryNotification notification);
    Task<NotificationStatus> GetStatusAsync();
}

public record PickNotification(
    string Team,
    string Market,
    string Description,
    decimal Odds,
    decimal RecommendedStake,
    string Classification,
    double Score,
    double Edge,
    DateTime MatchTime,
    string League,
    string HomeTeam,
    string AwayTeam
);

public record PipelineNotification(
    int FixturesScraped,
    int TotalTrends,
    int PicksScored,
    int PicksWithStakes,
    decimal TotalKellyStakes,
    List<PickNotification> TopPicks,
    TimeSpan Duration
);

public record DailySummaryNotification(
    DateTime Date,
    int TotalPicks,
    int PicksWithStakes,
    decimal TotalKellyStakes,
    List<PickNotification> TopPicks,
    List<CompletedPick> CompletedPicks
);

public record CompletedPick(
    string Team,
    string Market,
    string Result,
    decimal Profit
);

public record NotificationStatus(
    bool TelegramEnabled,
    bool EmailEnabled,
    string TelegramChatId,
    string EmailAddress,
    int NotificationsSentToday,
    DateTime? LastNotificationTime
);
