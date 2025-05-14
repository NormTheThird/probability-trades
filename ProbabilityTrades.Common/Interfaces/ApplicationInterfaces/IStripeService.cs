namespace ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

public interface IStripeService
{
    Task<(string CustomerId, bool HasCustomerId)> GetStripeCustomerIdAsync(Guid userId);
    Task<Guid> CreateStripeCustomerAsync(Guid userId, string CustomerId);
    Task UpdateStripeCustomerSubscriptionAsync(string customerId, bool hasSubscription);
}
