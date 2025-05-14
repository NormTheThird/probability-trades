namespace ProbabilityTrades.Bot.Discord.Processes;

public class NotificationProcess : BaseProcess<NotificationProcess>, IInvocable
{
    private readonly IDiscordNotificationService _discordNotificationService;

    public NotificationProcess(IConfiguration configuration, ILogger<NotificationProcess> logger, DiscordClient discordClient, 
                               IDiscordNotificationService discordNotificationService)
        : base(configuration, logger, discordClient)
    {
        _discordNotificationService = discordNotificationService ?? throw new ArgumentNullException(nameof(discordNotificationService));
    }

    public async Task Invoke()
    {
        await GetAndSendNotifications();
    }

    private async Task GetAndSendNotifications()
    {
        var discordNotifications = await _discordNotificationService.GetDiscordNotificationsNotNotifiedAsync();
        foreach (var discordNotification in discordNotifications)
        {
            await SendMessageAsync(discordNotification);
            await _discordNotificationService.UpdateDiscordNotificationAsNotifiedAsync(discordNotification.Id);
        }
    }

    private async Task SendMessageAsync(DiscordNotificationModel discordNotificationModel)
    {
        var messageChannel = await _discordClient.GetChannelAsync(discordNotificationModel.ChannelId);
        var discordEmbededBuilder = new DiscordEmbedBuilder
        {
            Color = new DiscordColor(discordNotificationModel.NotificationColor),
            Title = discordNotificationModel.Title,
            Description = discordNotificationModel.Message
        };

        if (!string.IsNullOrEmpty(discordNotificationModel.Author))
            discordEmbededBuilder.Author = new DiscordEmbedBuilder.EmbedAuthor { Name = discordNotificationModel.Author };

        if (!string.IsNullOrEmpty(discordNotificationModel.Footer))
            discordEmbededBuilder.Footer = new DiscordEmbedBuilder.EmbedFooter { Text = discordNotificationModel.Footer };

        await messageChannel.SendMessageAsync(embed: discordEmbededBuilder);
    }
}