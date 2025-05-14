namespace ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

public interface IDiscordNotificationService
{
    Task<List<DiscordNotificationModel>> GetDiscordNotificationsNotNotifiedAsync();
    Task<Guid> CreateDiscordNotificationAsync(DiscordNotificationModel discordNotificationModel);
    Task UpdateDiscordNotificationAsNotifiedAsync(Guid discordNotificationId);
}