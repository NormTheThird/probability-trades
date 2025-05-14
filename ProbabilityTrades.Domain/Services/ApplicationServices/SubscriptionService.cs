 namespace ProbabilityTrades.Domain.Services.ApplicationServices;

public class SubscriptionService : BaseApplicationService, ISubscriptionService
{
    public SubscriptionService(IConfiguration configuration, ApplicationDbContext db) : base(configuration, db) { }

    public async Task<IEnumerable<UserSubscriptionModel>> GetUsersAndSubscriptionsAsync()
    {
        return await _db.Database.SqlQuery<UserSubscriptionModel>($"EXEC GetUsersAndSubscriptions").ToListAsync();
    }
}