using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class DiscordNotification
{
    public Guid Id { get; set; }

    public long ChannelId { get; set; }

    public string Channel { get; set; } = null!;

    public string NotificationType { get; set; } = null!;

    public string NotificationColor { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string Footer { get; set; } = null!;

    public bool IsNotified { get; set; }

    public DateTime? NotificationSentAt { get; set; }

    public DateTime DateCreated { get; set; }
}
