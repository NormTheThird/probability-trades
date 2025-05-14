namespace ProbabilityTrades.Data.SqlServer.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddDatabaseContext<T>(this IServiceCollection services, string connectionString, ServiceLifetime serviceLifetime) where T : DbContext
    {
        services.AddDbContext<T>(options => options.UseSqlServer(connectionString), serviceLifetime);
    }
}