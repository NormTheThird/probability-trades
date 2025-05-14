using Stripe;

namespace ProbabilityTrades.Common.Interfaces.ApiInterfaces;

public interface IStripeApiService
{
    Task<IEnumerable<StripeApiProductAndPriceModel>> GetProductsWithPrices();
    Task<Customer> CreateCustomerAsync(Guid userId, string username, string email);
    Task<string> CreateCheckoutSessionUrl(string priceId, string customerId);
    Task<string> CreateCustomerPortalSessionUrlAsync(string customerId);
}