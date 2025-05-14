namespace ProbabilityTrades.Domain.Services.CurrencyHistoryServices;

public class KucoinService : BaseCurrencyHistoryService, ICurrencyHistoryService
{
    public KucoinService(CurrencyHistoryDbContext db) : base(db) { }

    public async Task<CurrencyHistoryModel> GetCurrentHistoryRecordAsync(string baseCurrency, string quoteCurrency, CandlestickPattern candlestickPattern)
    {
        var currentHistoryRecord = await _db.Kucoins.AsNoTracking()
                                                   .Where(_ => _.BaseCurrency.Equals(baseCurrency)
                                                            && _.QuoteCurrency.Equals(quoteCurrency)
                                                            && _.CandlestickPattern.Equals(candlestickPattern.ToString()))
                                                   .OrderByDescending(_ => _.ChartTimeEpoch)
                                                   .FirstOrDefaultAsync();

        if (currentHistoryRecord is null)
            throw new KeyNotFoundException($"Unable to get current history record for {baseCurrency}-{quoteCurrency} {candlestickPattern}");

        return new CurrencyHistoryModel
        {
            Id = currentHistoryRecord.Id,
            BaseCurrency = currentHistoryRecord.BaseCurrency,
            QuoteCurrency = currentHistoryRecord.QuoteCurrency,
            CandlestickPattern = Enum.Parse<CandlestickPattern>(currentHistoryRecord.CandlestickPattern),
            ChartTimeEpoch = currentHistoryRecord.ChartTimeEpoch,
            ChartTimeUTC = currentHistoryRecord.ChartTimeUTC,
            ChartTimeCST = currentHistoryRecord.ChartTimeCST ?? new(),
            OpeningPrice = currentHistoryRecord.OpeningPrice,
            ClosingPrice = currentHistoryRecord.ClosingPrice,
            HighestPrice = currentHistoryRecord.HighestPrice,
            LowestPrice = currentHistoryRecord.LowestPrice,
            Volume = currentHistoryRecord.Volume,
            Turnover = currentHistoryRecord.Turnover,
            LastChangedBy = currentHistoryRecord.LastChangedBy,
            DateLastChanged = currentHistoryRecord.DateLastChanged,
            DateCreated = currentHistoryRecord.DateCreated
        };
    }

    public async Task<List<CurrencyHistoryModel>> GetHistoryRecordsByIntervalsBackAsync(string baseCurrency, string quoteCurrency, CandlestickPattern candlestickPattern, int intervalsBack)
    {
        return await _db.Kucoins.AsNoTracking()
                               .Where(_ => _.BaseCurrency.Equals(baseCurrency)
                                        && _.QuoteCurrency.Equals(quoteCurrency)
                                        && _.CandlestickPattern.Equals(candlestickPattern.ToString()))
                               .OrderByDescending(_ => _.ChartTimeEpoch)
                               .Take(intervalsBack)
                               .Select(_ => new CurrencyHistoryModel
                               {
                                   Id = _.Id,
                                   BaseCurrency = _.BaseCurrency,
                                   QuoteCurrency = _.QuoteCurrency,
                                   CandlestickPattern = Enum.Parse<CandlestickPattern>(_.CandlestickPattern),
                                   ChartTimeEpoch = _.ChartTimeEpoch,
                                   ChartTimeUTC = _.ChartTimeUTC,
                                   ChartTimeCST = _.ChartTimeCST ?? new(),
                                   OpeningPrice = _.OpeningPrice,
                                   ClosingPrice = _.ClosingPrice,
                                   HighestPrice = _.HighestPrice,
                                   LowestPrice = _.LowestPrice,
                                   Volume = _.Volume,
                                   Turnover = _.Turnover,
                                   LastChangedBy = _.LastChangedBy,
                                   DateLastChanged = _.DateLastChanged,
                                   DateCreated = _.DateCreated

                               })
                               .ToListAsync();
    }

    public async Task<decimal> GetDailyVolumeAverageForDaysBack(string baseCurrency, string quoteCurrency, int daysBack)
    {
        // Just searching the days back gives you n-1 as a list of records. I needed to add
        // a +1 to daysback to have the list return n records for the average.
        var daysBackEpoch = DateTime.UtcNow.ToEpoch() - (86400 * (daysBack + 1));
        return await _db.Kucoins.AsNoTracking()
                               .Where(_ => _.BaseCurrency.Equals(baseCurrency)
                                        && _.QuoteCurrency.Equals(quoteCurrency)
                                        && _.CandlestickPattern.Equals(CandlestickPattern.OneDay.ToString())
                                        && _.ChartTimeEpoch > daysBackEpoch)
                               .AverageAsync(_ => _.Volume);
    }

    public async Task CreateFromListAsync(string baseCurrency, string quoteCurrency, CandlestickPattern candlestickPattern, List<CreateCurrencyHistoryModel> createCurrencyHistoryModels, string lastChangedBy)
    {
        var kucoinList = new List<Kucoin>();
        var existingChartTimes = await _db.Kucoins.AsNoTracking()
                                                  .Where(_ => _.BaseCurrency.Equals(baseCurrency) && _.QuoteCurrency.Equals(quoteCurrency)
                                                           && _.CandlestickPattern.Equals(candlestickPattern.ToString()))
                                                  .Select(_ => _.ChartTimeEpoch)
                                                  .ToListAsync();

        foreach (var createCurrencyHistoryModel in createCurrencyHistoryModels)
        {
            if (existingChartTimes.Contains(createCurrencyHistoryModel.ChartTimeEpoch))
                continue;

            var nowInCst = DateTime.Now.InCst();
            kucoinList.Add(new Kucoin
            {
                Id = Guid.NewGuid(),
                BaseCurrency = createCurrencyHistoryModel.BaseCurrency,
                QuoteCurrency = createCurrencyHistoryModel.QuoteCurrency,
                CandlestickPattern = createCurrencyHistoryModel.CandlestickPattern.ToString(),
                ChartTimeEpoch = createCurrencyHistoryModel.ChartTimeEpoch,
                ChartTimeUTC = createCurrencyHistoryModel.ChartTimeUTC,
                OpeningPrice = createCurrencyHistoryModel.OpeningPrice,
                ClosingPrice = createCurrencyHistoryModel.ClosingPrice,
                HighestPrice = createCurrencyHistoryModel.HighestPrice,
                LowestPrice = createCurrencyHistoryModel.LowestPrice,
                Volume = createCurrencyHistoryModel.Volume,
                Turnover = createCurrencyHistoryModel.Turnover,
                LastChangedBy = lastChangedBy,
                DateLastChanged = nowInCst,
                DateCreated = nowInCst
            });
        }

        _db.Kucoins.AddRange(kucoinList);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateMovingAveragesAsync()
    {
        _db.Database.SetCommandTimeout(TimeSpan.FromMinutes(60));
        await _db.Database.ExecuteSqlRawAsync($"EXEC Kucoin_UpdateMovingAverages");
    }

    public async Task MoveRecordsToArchiveAsync()
    {
        _db.Database.SetCommandTimeout(TimeSpan.FromMinutes(30));
        await _db.Database.ExecuteSqlRawAsync($"EXEC Kucoin_MoveKucoinRecordsToKucoinArchive");
    }



    public async Task<IEnumerable<BaseCurrencyHistoryModel>> GetCurrencyHistorySummary()
    {
        return await _db.CallStoredProcedureAsync<BaseCurrencyHistoryModel>("GetCurrencyHistorySummary");
    }

    public async Task<IEnumerable<CurrencyHistorySummaryDetailModel>> GetCurrencyHistorySummaryDetail(string dataSource, string baseCurrency, string quoteCurrency)
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@DataSource", dataSource),
            new SqlParameter("@BaseCurrency", baseCurrency),
            new SqlParameter("@QuoteCurrency", quoteCurrency),
        };
        return await _db.CallStoredProcedureAsync<CurrencyHistorySummaryDetailModel>("GetCurrencyHistorySummaryDetail", parameters);
    }

    public async Task<IEnumerable<CurrencyHistoryModel>> GetCurrencyHistoryDetails(string dataSource, string baseCurrency, string quoteCurrency, string candlePattern, int offset, int fetch)
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@TableName", GetTableName(baseCurrency)),
            new SqlParameter("@DataSource", dataSource),
            new SqlParameter("@QuoteCurrency", quoteCurrency),
            new SqlParameter("@CandlePattern", candlePattern),
            new SqlParameter("@Offset", offset),
            new SqlParameter("@Fetch", fetch)
        };
        return await _db.CallStoredProcedureAsync<CurrencyHistoryModel>("GetCurrencyHistoryDetails", parameters);
    }

    public async Task<IEnumerable<CurrencyHistoryModel>> GetCurrencyHistoryDetailsByDate(BaseDataModel baseDataModel, DateTime startDate, DateTime endDate, int shortMovingAvg, int longMovingAvg)
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@TableName", GetTableName(baseDataModel.BaseCurrency)),
            new SqlParameter("@DataSource", baseDataModel.DataSource.ToString()),
            new SqlParameter("@QuoteCurrency", baseDataModel.QuoteCurrency),
            new SqlParameter("@CandlePattern", baseDataModel.CandlestickPattern.ToString()),
            new SqlParameter("@StartDate", startDate),
            new SqlParameter("@EndDate", endDate),
            new SqlParameter("@ShortMovingAverage", shortMovingAvg),
            new SqlParameter("@LongMovingAverage", longMovingAvg)
        };
        return await _db.CallStoredProcedureAsync<CurrencyHistoryModel>("GetCurrencyHistoryDetailsByDate", parameters);
    }
}