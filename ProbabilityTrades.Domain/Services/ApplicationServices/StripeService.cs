namespace ProbabilityTrades.Domain.Services.ApplicationServices;

public class StripeService : BaseApplicationService, IStripeService
{
    public StripeService(IConfiguration configuration, ApplicationDbContext db) : base(configuration, db) { }

    public async Task<(string CustomerId, bool HasCustomerId)> GetStripeCustomerIdAsync(Guid userId)
    {
        var stripeCustomer = await _db.StripeCustomers.AsNoTracking().FirstOrDefaultAsync(_ => _.UserId.Equals(userId));
        if (stripeCustomer is null)
            return (null, false);

        return (stripeCustomer.CustomerId, true);
    }

    public async Task<Guid> CreateStripeCustomerAsync(Guid userId, string CustomerId)
    {
        var customer = new StripeCustomer
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CustomerId = CustomerId,
            DateCreated = DateTime.Now.InCst()
        };

        _db.StripeCustomers.Add(customer);
        await _db.SaveChangesAsync();

        return customer.Id;
    }

    public async Task UpdateStripeCustomerSubscriptionAsync(string customerId, bool hasSubscription)
    {
        var stripeCustomer = await _db.StripeCustomers.FirstOrDefaultAsync(_ => _.CustomerId.Equals(customerId));
        if (stripeCustomer is null)
            throw new KeyNotFoundException($"Stripe customer {customerId} does not exist.");

        stripeCustomer.HasSubscription = hasSubscription;
        stripeCustomer.LastSubscriptionDateChange = DateTime.Now.InCst();

        await _db.SaveChangesAsync();
    }
}