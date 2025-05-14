namespace ProbabilityTrades.Common.Interfaces.CurrencyHistoryInterfaces;

public interface ICurrencyHistoryService
{
    Task<CurrencyHistoryModel> GetCurrentHistoryRecordAsync(string baseCurrency, string quoteCurrency, CandlestickPattern candlestickPattern);
    Task<List<CurrencyHistoryModel>> GetHistoryRecordsByIntervalsBackAsync(string baseCurrency, string quoteCurrency, CandlestickPattern candlestickPattern, int intervalsBack);
    Task<decimal> GetDailyVolumeAverageForDaysBack(string baseCurrency, string quoteCurrency, int daysBack);
    Task CreateFromListAsync(string baseCurrency, string quoteCurrency, CandlestickPattern candlestickPattern, List<CreateCurrencyHistoryModel> createCurrencyHistoryModels, string lastChangedBy);
    Task UpdateMovingAveragesAsync();
    Task MoveRecordsToArchiveAsync();
    Task<IEnumerable<BaseCurrencyHistoryModel>> GetCurrencyHistorySummary();
    Task<IEnumerable<CurrencyHistorySummaryDetailModel>> GetCurrencyHistorySummaryDetail(string dataSource, string baseCurrency, string quoteCurrency);
    Task<IEnumerable<CurrencyHistoryModel>> GetCurrencyHistoryDetails(string dataSource, string baseCurrency, string quoteCurrency, string candlePattern, int offset, int fetch);
    Task<IEnumerable<CurrencyHistoryModel>> GetCurrencyHistoryDetailsByDate(BaseDataModel baseDataModel, DateTime startDate, DateTime endDate, int shortMovingAvg, int longMovingAvg);
}