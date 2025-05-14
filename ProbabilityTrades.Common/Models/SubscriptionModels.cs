namespace ProbabilityTrades.Common.Models;

public class UserSubscriptionModel
{
    public Guid UserId { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public long DiscordUserId { get; set; } = 0;
    public string DiscordAccessToken { get; set; } = string.Empty;
    public string DiscordRefreshToken { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public bool HasSubscription { get; set; } = false;
    public DateTimeOffset? LastSubscriptionDateChange { get; set; } = null;
}