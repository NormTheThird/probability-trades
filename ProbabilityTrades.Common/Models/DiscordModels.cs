namespace ProbabilityTrades.Common.Models;

public class DiscordNotificationModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public ulong ChannelId { get; set; } = 0;
    public string Channel { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string NotificationColor { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Footer { get; set; } = string.Empty;
    public bool IsNotified { get; set; } = false;
    public DateTime? NotificationSentAt { get; set; } = null;
    public DateTime DateCreated { get; set; } = new();
}