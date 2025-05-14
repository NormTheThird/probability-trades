namespace ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

public interface IMarketService
{
    Task<List<MarketModel>> GetMarketsAsync();
    Task<List<MarketBaseModel>> GetActiveMarketListAsync();
    Task<Guid?> CreateMarketAsync(DataSource dataSource, string marketName, string createdBy);
    Task UpdateMarketAsync(MarketModel marketModel);
}