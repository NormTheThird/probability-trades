namespace ProbabilityTrades.Domain.Services.ApplicationServices;

public class MarketService : BaseApplicationService, IMarketService
{
    public MarketService(IConfiguration configuration, ApplicationDbContext db) : base(configuration, db) { }

    public async Task<List<MarketModel>> GetMarketsAsync()
    {
        var markets = await _db.Markets.AsNoTracking()
                                       .Select(_ => new MarketModel
                                       {
                                           Id = _.Id,
                                           DataSource = _.DataSource,
                                           Name = _.Name,
                                           DisplayName = _.DisplayName,
                                           Description = _.Description,
                                           IsActive = _.IsActive,
                                           IsDeleted = _.IsDeleted,
                                           LastChangedBy = _.LastChangedBy,
                                           DateLastChanged = _.DateLastChanged,
                                           DateCreated = _.DateCreated
                                       }).ToListAsync();
        return markets;
    }

    public async Task<List<MarketBaseModel>> GetActiveMarketListAsync()
    {
        var activeMarkets = await _db.Markets.AsNoTracking()
                                             .Where(_ => _.IsActive && !_.IsDeleted)
                                             .Select(_ => new MarketBaseModel
                                             {
                                                 Id = _.Id,
                                                 DataSource = _.DataSource,
                                                 Name = _.Name,
                                                 DisplayName = _.DisplayName,
                                             }).ToListAsync();
        return activeMarkets;
    }

    public async Task<Guid?> CreateMarketAsync(DataSource dataSource, string marketName, string createdBy)
    {
        var utcNow = DateTime.UtcNow;
        var market = await _db.Markets.FirstOrDefaultAsync(_ => _.DataSource.Equals(dataSource.ToString()) && _.Name.Equals(marketName));
        if (market != null)
            return market.Id;

        market = new Market
        {
            Id = Guid.NewGuid(),
            DataSource = dataSource.ToString(),
            Name = marketName,
            DisplayName = marketName,
            Description = marketName,
            IsActive = false,
            IsDeleted = false,
            LastChangedBy = createdBy,
            DateLastChanged = utcNow,
            DateCreated = DateTimeOffset.UtcNow
        };
        _db.Markets.Add(market);
        await _db.SaveChangesAsync();

        return market.Id;
    }

    public async Task UpdateMarketAsync(MarketModel marketModel)
    {
        var market = await _db.Markets.FirstOrDefaultAsync(_ => _.Id.Equals(marketModel.Id));
        if (market == null)
            throw new ApplicationException($"Market cannot be found for id {marketModel.Id}");

        market.DisplayName = marketModel.DisplayName;
        market.Description = marketModel.Description;
        market.IsActive = marketModel.IsActive;
        market.IsDeleted = marketModel.IsDeleted;
        market.LastChangedBy = marketModel.LastChangedBy;
        market.DateLastChanged = DateTime.Now.InCst();

        await _db.SaveChangesAsync();
    }
}