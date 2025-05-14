namespace ProbabilityTrades.Domain.Services.ApplicationServices;

public class DiscordNotificationService : BaseApplicationService, IDiscordNotificationService
{
    public DiscordNotificationService(IConfiguration configuration, ApplicationDbContext db) : base(configuration, db) { }

    public async Task<List<DiscordNotificationModel>> GetDiscordNotificationsNotNotifiedAsync()
    {
        var discordNotifications = await _db.DiscordNotifications.AsNoTracking()
                                                                 .Where(_ => !_.IsNotified)      
                                                                 .OrderByDescending(_ => _.DateCreated)
                                                                 .Select(_ => new DiscordNotificationModel
                                                                 {
                                                                     Id = _.Id,
                                                                     ChannelId = (ulong)_.ChannelId,
                                                                     Channel = _.Channel,
                                                                     NotificationType = _.NotificationType,
                                                                     NotificationColor = _.NotificationColor,
                                                                     Author = _.Author,
                                                                     Title = _.Title,
                                                                     Message = _.Message,
                                                                     Footer = _.Footer,
                                                                     IsNotified = _.IsNotified,
                                                                     NotificationSentAt = _.NotificationSentAt,
                                                                     DateCreated = _.DateCreated
                                                                 })
                                                                 .ToListAsync();
        return discordNotifications;
    }

    public async Task<Guid> CreateDiscordNotificationAsync(DiscordNotificationModel discordNotificationModel)
    {
        var discordNotification = new DiscordNotification
        {
            Id = Guid.NewGuid(),
            ChannelId = (long)discordNotificationModel.ChannelId,
            Channel = discordNotificationModel.Channel,
            NotificationType = discordNotificationModel.NotificationType,
            NotificationColor = discordNotificationModel.NotificationColor,
            Author = discordNotificationModel.Author,
            Title = discordNotificationModel.Title,
            Message = discordNotificationModel.Message,
            Footer = discordNotificationModel.Footer,
            IsNotified = false,
            NotificationSentAt = null,
            DateCreated = DateTime.Now.InCst()
        };
        
        _db.DiscordNotifications.Add(discordNotification);

        await _db.SaveChangesAsync();

        return discordNotification.Id;
    }

    public async Task UpdateDiscordNotificationAsNotifiedAsync(Guid discordNotificationId)
    {
        var discordNotification = await _db.DiscordNotifications.FirstOrDefaultAsync(_ => _.Id.Equals(discordNotificationId))
            ?? throw new KeyNotFoundException($"Unable to find DiscordNotification with Id: {discordNotificationId}");

        discordNotification.IsNotified = true;
        discordNotification.NotificationSentAt = DateTime.Now.InCst();

        await _db.SaveChangesAsync();
    }
}