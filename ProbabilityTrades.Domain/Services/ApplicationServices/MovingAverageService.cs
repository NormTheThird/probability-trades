namespace ProbabilityTrades.Domain.Services.ApplicationServices;

public class MovingAverageService : BaseApplicationService, IMovingAverageService
{
    public MovingAverageService(IConfiguration configuration, ApplicationDbContext db) : base(configuration, db) { }

    public async Task<List<MovingAverageConfigurationModel>> GetMovingAverageConfigurationsAsync(DataSource dataSource)
    {
        return await _db.MovingAverageConfigurations.AsNoTracking()
                                                    .Where(_ => _.DataSource.Equals(dataSource.ToString()))
                                                    .OrderBy(_ => _.BaseCurrency)
                                                    .ThenBy(_ => _.CandlestickPattern)
                                                    .Select(_ => new MovingAverageConfigurationModel
                                                    {
                                                        Id = _.Id,
                                                        DataSource = Enum.Parse<DataSource>(_.DataSource),
                                                        BaseCurrency = _.BaseCurrency,
                                                        QuoteCurrency = _.QuoteCurrency,
                                                        CandlestickPattern = Enum.Parse<CandlestickPattern>(_.CandlestickPattern),
                                                        ShortMovingAverageDays = _.ShortMovingAverageDays,
                                                        LongMovingAverageDays = _.LongMovingAverageDays,
                                                        StopLossPercentage = _.StopLossPercentage,
                                                        IsActive = _.IsActive,
                                                        IsSendSMSNotification = _.IsSendSMSNotification,
                                                        IsSendDiscordNotification = _.IsSendDiscordNotification,
                                                        LastChangedBy = _.LastChangedBy,
                                                        DateLastChanged = _.DateLastChanged,
                                                        DateCreated = _.DateCreated
                                                    })
                                                    .ToListAsync();
    }

    public async Task<MovingAverageConfigurationModel> GetMovingAverageConfigurationAsync(Guid maConfigurationId)
    {
        var maConfiguration = await _db.MovingAverageConfigurations.AsNoTracking().FirstOrDefaultAsync(_ => _.Id.Equals(maConfigurationId))
            ?? throw new KeyNotFoundException($"Unable to find MovingAverageConfiguration for id {maConfigurationId}");

        return new MovingAverageConfigurationModel
        {
            Id = maConfiguration.Id,
            DataSource = Enum.Parse<DataSource>(maConfiguration.DataSource),
            BaseCurrency = maConfiguration.BaseCurrency,
            QuoteCurrency = maConfiguration.QuoteCurrency,
            CandlestickPattern = Enum.Parse<CandlestickPattern>(maConfiguration.CandlestickPattern),
            ShortMovingAverageDays = maConfiguration.ShortMovingAverageDays,
            LongMovingAverageDays = maConfiguration.LongMovingAverageDays,
            StopLossPercentage = maConfiguration.StopLossPercentage,
            IsActive = maConfiguration.IsActive,
            IsSendSMSNotification = maConfiguration.IsSendSMSNotification,
            IsSendDiscordNotification = maConfiguration.IsSendDiscordNotification,
            LastChangedBy = maConfiguration.LastChangedBy,
            DateLastChanged = maConfiguration.DateLastChanged,
            DateCreated = maConfiguration.DateCreated
        };
    }

    public async Task<Guid> CreateMovingAverageConfigurationAsync(MovingAverageConfigurationModel maConfigurationModel, string lastChangedBy)
    {
        var maConfiguration = await _db.MovingAverageConfigurations.FirstOrDefaultAsync(_ => _.DataSource.Equals(maConfigurationModel.DataSource.ToString())
                                                                                           && _.BaseCurrency.Equals(maConfigurationModel.BaseCurrency)
                                                                                           && _.QuoteCurrency.Equals(maConfigurationModel.QuoteCurrency)
                                                                                           && _.CandlestickPattern.Equals(maConfigurationModel.CandlestickPattern.ToString()));
        if (maConfiguration is not null)
            throw new DuplicateNameException($"This configuration already exists.");

        var nowInCst = DateTime.Now.InCst();
        maConfiguration = new MovingAverageConfiguration
        {
            Id = Guid.NewGuid(),
            DataSource = maConfigurationModel.DataSource.ToString(),
            BaseCurrency = maConfigurationModel.BaseCurrency,
            QuoteCurrency = maConfigurationModel.QuoteCurrency,
            CandlestickPattern = maConfigurationModel.CandlestickPattern.ToString(),
            ShortMovingAverageDays = maConfigurationModel.ShortMovingAverageDays,
            LongMovingAverageDays = maConfigurationModel.LongMovingAverageDays,
            StopLossPercentage = maConfigurationModel.StopLossPercentage,
            IsActive = true,
            IsSendSMSNotification = maConfigurationModel.IsSendSMSNotification,
            IsSendDiscordNotification = maConfigurationModel.IsSendDiscordNotification,
            LastChangedBy = lastChangedBy,
            DateLastChanged = nowInCst,
            DateCreated = nowInCst
        };

        _db.MovingAverageConfigurations.Add(maConfiguration);
        await _db.SaveChangesAsync();
        return maConfiguration.Id;
    }

    public async Task UpdateMovingAverageConfigurationAsync(MovingAverageConfigurationModel maConfigurationModel, string lastChangedBy)
    {
        var maConfiguration = await _db.MovingAverageConfigurations.FirstOrDefaultAsync(_ => _.Id.Equals(maConfigurationModel.Id));
        if (maConfiguration is null)
            throw new KeyNotFoundException($"Unable to find moving average configuration for id {maConfiguration.Id}");

        maConfiguration.ShortMovingAverageDays = maConfigurationModel.ShortMovingAverageDays;
        maConfiguration.LongMovingAverageDays = maConfigurationModel.LongMovingAverageDays;
        maConfiguration.StopLossPercentage = maConfigurationModel.StopLossPercentage;
        maConfiguration.IsActive = maConfigurationModel.IsActive;
        maConfiguration.IsSendSMSNotification = maConfigurationModel.IsSendSMSNotification;
        maConfiguration.IsSendDiscordNotification = maConfigurationModel.IsSendDiscordNotification;
        maConfiguration.LastChangedBy = lastChangedBy;
        maConfiguration.DateLastChanged = DateTime.Now.InCst();

        await _db.SaveChangesAsync();
    }

    public async Task DeleteMovingAverageConfigurationAsync(Guid maConfigurationId)
    {
        var maConfiguration = await _db.MovingAverageConfigurations.FirstOrDefaultAsync(_ => _.Id.Equals(maConfigurationId));
        if (maConfiguration is null)
            throw new KeyNotFoundException($"Unable to find moving average configuration for id {maConfigurationId}");

        _db.MovingAverageConfigurations.Remove(maConfiguration);

        await _db.SaveChangesAsync();
    }

    public async Task<List<MovingAverageStatusModel>> GetMovingAverageStatusesAsync(BaseDataModel baseDataModel, int numberOfPositions = 0)
    {
        return await _db.MovingAverageStatuses.AsNoTracking()
                                              .Where(_ => _.DataSource.Equals(baseDataModel.DataSource.ToString()) &&
                                                          _.BaseCurrency.Equals(baseDataModel.BaseCurrency) &&
                                                          _.QuoteCurrency.Equals(baseDataModel.QuoteCurrency) &&
                                                          _.CandlestickPattern.Equals(baseDataModel.CandlestickPattern.ToString()))
                                              .OrderByDescending(_ => _.ChartTimeEpoch)
                                              .TakeIf(numberOfPositions)
                                              .Select(_ => new MovingAverageStatusModel
                                              {
                                                  Id = _.Id,
                                                  DataSource = Enum.Parse<DataSource>(_.DataSource),
                                                  BaseCurrency = _.BaseCurrency,
                                                  QuoteCurrency = _.QuoteCurrency,
                                                  ChartTimeEpoch = _.ChartTimeEpoch,
                                                  CandlestickPattern = baseDataModel.CandlestickPattern,
                                                  MarketPosition = Enum.Parse<MarketPosition>(_.MarketPosition),
                                                  IsActionChange = _.IsPositionChange,
                                                  CloseDate = _.CloseDate,
                                                  ClosePrice = _.ClosePrice,
                                                  ShortMovingAverageDays = _.ShortMovingAverageDays,
                                                  ShortMovingAverage = _.ShortMovingAverage,
                                                  LongMovingAverageDays = _.LongMovingAverageDays,
                                                  LongMovingAverage = _.LongMovingAverage
                                              })
                                              .ToListAsync();
    }

    public async Task<List<MovingAveragePositionStatusModel>> GetMovingAveragePositionStatusesAsync(BaseDataModel baseDataModel, int numberOfPositions = 0)
    {
        return await _db.MovingAverageStatuses.AsNoTracking()
                                              .Where(_ => _.DataSource.Equals(baseDataModel.DataSource.ToString()) &&
                                                           _.BaseCurrency.Equals(baseDataModel.BaseCurrency) &&
                                                           _.QuoteCurrency.Equals(baseDataModel.QuoteCurrency) &&
                                                           _.CandlestickPattern.Equals(baseDataModel.CandlestickPattern.ToString()) &&
                                                           _.IsPositionChange)
                                              .OrderByDescending(_ => _.ChartTimeEpoch)
                                              .TakeIf(numberOfPositions)
                                              .Select(_ => new MovingAveragePositionStatusModel
                                              {
                                                  DataSource = Enum.Parse<DataSource>(_.DataSource),
                                                  BaseCurrency = _.BaseCurrency,
                                                  QuoteCurrency = _.QuoteCurrency,
                                                  MarketPosition = Enum.Parse<MarketPosition>(_.MarketPosition),
                                                  CloseDate = _.CloseDate,
                                                  ClosePrice = _.ClosePrice,
                                              })
                                              .ToListAsync();
    }

    public async Task<List<MovingAverageStatusModel>> GetMovingAverageStatusPositionChangesByDateAsync(DataSource dataSource, DateTime dateTime)
    {
        return await _db.MovingAverageStatuses.AsNoTracking()
                                              .Where(_ => _.DataSource.Equals(dataSource.ToString())
                                                       && _.CloseDate.Equals(DateOnly.FromDateTime(dateTime))  
                                                       && _.IsPositionChange)
                                              .Select(_ => new MovingAverageStatusModel
                                              {
                                                  Id = _.Id,
                                                  DataSource = Enum.Parse<DataSource>(_.DataSource),
                                                  BaseCurrency = _.BaseCurrency,
                                                  QuoteCurrency = _.QuoteCurrency,
                                                  ChartTimeEpoch = _.ChartTimeEpoch,
                                                  CandlestickPattern = Enum.Parse<CandlestickPattern>(_.CandlestickPattern),
                                                  MarketPosition = Enum.Parse<MarketPosition>(_.MarketPosition),
                                                  IsActionChange = _.IsPositionChange,
                                                  CloseDate = _.CloseDate,
                                                  ClosePrice = _.ClosePrice,
                                                  ShortMovingAverageDays = _.ShortMovingAverageDays,
                                                  ShortMovingAverage = _.ShortMovingAverage,
                                                  LongMovingAverageDays = _.LongMovingAverageDays,
                                                  LongMovingAverage = _.LongMovingAverage
                                              })
                                              .ToListAsync();
    }

    public async Task<IEnumerable<MovingAverageCurrentPositionModel>> GetCurrentMovingAveragePositionsAsync(DataSource dataSource)
    {
        return await _db.Database.SqlQuery<MovingAverageCurrentPositionModel>($"EXEC MovingAverage_GetCurrentPositions {dataSource.ToString()}").ToListAsync();
    }

    public async Task<Guid> CreateMovingAverageStatusAsync(MovingAverageStatusModel movingAverageStatusModel)
    {
        // TODO: TREY: 2022.10.19: DISCUSS: Instead of just entering new record check if moving average position exsists
        //       for DataSource, BaseCurrency, QuoteCurrency, CandlePattern and ChartTimeEpoch
        // var movingAveragePosition = await _db.MovingAveragePositions.FirstOrDefaultAsync(_ => _.Id.Equals(movingAveragePositionModel.Id));
        var previousMovingAverageStatus = await _db.MovingAverageStatuses.Where(_ => _.DataSource.Equals(movingAverageStatusModel.DataSource.ToString()) &&
                                                                                     _.BaseCurrency.Equals(movingAverageStatusModel.BaseCurrency) &&
                                                                                     _.QuoteCurrency.Equals(movingAverageStatusModel.QuoteCurrency) &&
                                                                                     _.CandlestickPattern.Equals(movingAverageStatusModel.CandlestickPattern.ToString()))
                                                                         .OrderByDescending(_ => _.CloseDate)
                                                                         .FirstOrDefaultAsync();

        var isMarketPositionChange = false;
        if (previousMovingAverageStatus == null || previousMovingAverageStatus.MarketPosition != movingAverageStatusModel.MarketPosition.ToString())
            isMarketPositionChange = true;

        var movingAverageStatus = await _db.MovingAverageStatuses.FirstOrDefaultAsync(_ => _.DataSource.Equals(movingAverageStatusModel.DataSource.ToString()) &&
                                                                                           _.BaseCurrency.Equals(movingAverageStatusModel.BaseCurrency) &&
                                                                                           _.QuoteCurrency.Equals(movingAverageStatusModel.QuoteCurrency) &&
                                                                                           _.CandlestickPattern.Equals(movingAverageStatusModel.CandlestickPattern.ToString()) &&
                                                                                           _.ChartTimeEpoch.Equals(movingAverageStatusModel.ChartTimeEpoch));

        if (movingAverageStatus == null)
        {
            movingAverageStatus = new MovingAverageStatus
            {
                Id = Guid.NewGuid(),
                DataSource = movingAverageStatusModel.DataSource.ToString(),
                BaseCurrency = movingAverageStatusModel.BaseCurrency,
                QuoteCurrency = movingAverageStatusModel.QuoteCurrency,
                ChartTimeEpoch = movingAverageStatusModel.ChartTimeEpoch,
                CandlestickPattern = movingAverageStatusModel.CandlestickPattern.ToString(),
                MarketPosition = movingAverageStatusModel.MarketPosition.ToString(),
                IsPositionChange = isMarketPositionChange,
                CloseDate = movingAverageStatusModel.CloseDate,
                ClosePrice = movingAverageStatusModel.ClosePrice,
                ShortMovingAverageDays = movingAverageStatusModel.ShortMovingAverageDays,
                ShortMovingAverage = movingAverageStatusModel.ShortMovingAverage,
                LongMovingAverageDays = movingAverageStatusModel.LongMovingAverageDays,
                LongMovingAverage = movingAverageStatusModel.LongMovingAverage,
                DateCreated = DateTime.Now.InCst()
            };

            _db.MovingAverageStatuses.Add(movingAverageStatus);
            await _db.SaveChangesAsync();
        }

        return movingAverageStatus.Id;
    }
}