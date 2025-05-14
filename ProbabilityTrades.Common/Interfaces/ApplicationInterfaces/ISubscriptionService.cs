namespace ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

public interface ISubscriptionService
{
    Task<IEnumerable<UserSubscriptionModel>> GetUsersAndSubscriptionsAsync();
}