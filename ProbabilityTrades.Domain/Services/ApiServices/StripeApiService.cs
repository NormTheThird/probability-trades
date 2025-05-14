using Stripe;
using Stripe.Checkout;

namespace ProbabilityTrades.Domain.Services.ApiServices;

public class StripeApiService : IStripeApiService
{
    private readonly IConfiguration _config;
    private readonly IStripeClient _stripeClient;

    public StripeApiService(IConfiguration config, IStripeClient stripeClient)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _stripeClient = stripeClient ?? throw new ArgumentNullException(nameof(stripeClient));
    }

    public async Task<IEnumerable<StripeApiProductAndPriceModel>> GetProductsWithPrices()
    {
        var priceListOptions = new PriceListOptions
        {
            Limit = 5,
            Active = true,
            Expand = new List<string> { "data.product" },
            Type = "recurring"
        };
        var priceService = new PriceService();
        var productsAndPrices = await priceService.ListAsync(priceListOptions);

        return productsAndPrices.Select(_ => new StripeApiProductAndPriceModel
        {
            PriceId = _.Id,
            ProductId = _.Product.Id,
            Features = _.Product.MarketingFeatures.Select(_ => _.Name).ToList(),
            Name = _.Product.Name,
            Description = _.Product.Description,
            Price = Math.Round((_.UnitAmountDecimal / 100) ?? 0.0m, 2, MidpointRounding.AwayFromZero)
        }).ToList();
    }

    public async Task<Customer> CreateCustomerAsync(Guid userId, string username, string email)
    {
        var options = new CustomerCreateOptions
        {
            Description = $"{userId}",
            Name = username,
            Email = email
        };

        var customerService = new CustomerService();
        return await customerService.CreateAsync(options);
    }

    public async Task<string> CreateCheckoutSessionUrl(string priceId, string customerId)
    {
        var baseUrl = _config.GetValue<string>("BaseUrl");
        var sessionCreateOptions = new SessionCreateOptions
        {
            Customer = customerId,
            LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = priceId,
                        Quantity = 1,
                    },
                },
            Mode = "subscription",
            AllowPromotionCodes = true,
            //Discounts = new List<SessionDiscountOptions>
            //{
            //        new SessionDiscountOptions
            //        {
            //            Coupon = "WFcVY5pn",
            //        },
            //    },
            SuccessUrl = $"{baseUrl}subscribed?sessionId={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{baseUrl}",
        };

        var sessionService = new SessionService(_stripeClient);
        var session = await sessionService.CreateAsync(sessionCreateOptions);
        return session.Url;
    }

    public async Task<string> CreateCustomerPortalSessionUrlAsync(string customerId)
    {
        var baseUrl = _config.GetValue<string>("BaseUrl");
        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Configuration = await CreateCustomerPortalConfigurationIdAsync(),
            Customer = customerId,
            ReturnUrl = $"{baseUrl}/subscription",
        };
        var service = new Stripe.BillingPortal.SessionService();

        var session = await service.CreateAsync(options);
        
        return session.Url;
    }


    private async Task<string> CreateCustomerPortalConfigurationIdAsync()
    {
        var options = new Stripe.BillingPortal.ConfigurationCreateOptions
        {
            BusinessProfile = new Stripe.BillingPortal.ConfigurationBusinessProfileOptions
            {
                PrivacyPolicyUrl = "https://example.com/privacy",
                TermsOfServiceUrl = "https://example.com/terms",
            },
            Features = new Stripe.BillingPortal.ConfigurationFeaturesOptions
            {
                CustomerUpdate = new Stripe.BillingPortal.ConfigurationFeaturesCustomerUpdateOptions
                {
                    AllowedUpdates = new List<string> { "email", "tax_id" },
                    Enabled = true,
                },
                InvoiceHistory = new Stripe.BillingPortal.ConfigurationFeaturesInvoiceHistoryOptions
                {
                    Enabled = true,
                },
                PaymentMethodUpdate = new Stripe.BillingPortal.ConfigurationFeaturesPaymentMethodUpdateOptions
                {
                    Enabled = true,
                },
                SubscriptionCancel = new Stripe.BillingPortal.ConfigurationFeaturesSubscriptionCancelOptions
                {
                    Enabled = true,
                },
                SubscriptionUpdate = new Stripe.BillingPortal.ConfigurationFeaturesSubscriptionUpdateOptions
                {
                    Enabled = true,
                }
            },
        };
        var service = new Stripe.BillingPortal.ConfigurationService();

        var configuration = await service.CreateAsync(options);

        return configuration.Id;
    }
}