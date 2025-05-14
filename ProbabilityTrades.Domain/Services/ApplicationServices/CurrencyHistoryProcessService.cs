namespace ProbabilityTrades.Domain.Services.ApplicationServices;

public class CurrencyHistoryProcessService : BaseApplicationService, ICurrencyHistoryProcessService
{
    public CurrencyHistoryProcessService(IConfiguration configuration, ApplicationDbContext db) : base(configuration, db) { }

    public async Task<List<CurrencyHistoryProcessModel>> GetCurrencyHistoryProcesses(DataSource dataSource)
    {
        return await _db.CurrencyHistoryProcesses.AsNoTracking()
                                                 .Where(_ => _.DataSource.Equals(dataSource.ToString()))
                                                 .OrderBy(_ => _.BaseCurrency)
                                                 .Select(_ => new CurrencyHistoryProcessModel
                                                 {
                                                     Id = _.Id,
                                                     DataSource = _.DataSource,
                                                     BaseCurrency = _.BaseCurrency,
                                                     QuoteCurrency = _.QuoteCurrency,
                                                     CandlestickPattern = Enum.Parse<CandlestickPattern>(_.CandlePattern),
                                                     IntervalsBack = _.IntervalsBack,
                                                     IsActive = _.IsActive,
                                                     LastChangedBy = _.LastChangedBy,
                                                     LastChangedAt = _.DateLastChanged,
                                                     DateCreated = _.DateCreated,
                                                 }).ToListAsync();
    }

    public async Task UpdateIntervalsBack(Guid currencyHistoryProcessId, int intervalsBack, string lastChangedBy)
    {
        var currencyHistoryProcess = await _db.CurrencyHistoryProcesses.FirstOrDefaultAsync(_ => _.Id.Equals(currencyHistoryProcessId))
            ?? throw new KeyNotFoundException($"Unable to get currency history process for id {currencyHistoryProcessId}");

        currencyHistoryProcess.IntervalsBack = intervalsBack;
        currencyHistoryProcess.LastChangedBy = lastChangedBy;
        currencyHistoryProcess.DateLastChanged = DateTime.Now.InCst();

        await _db.SaveChangesAsync();
    }
}