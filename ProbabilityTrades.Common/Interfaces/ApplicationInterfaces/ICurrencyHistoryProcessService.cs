namespace ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

public interface ICurrencyHistoryProcessService
{
    Task<List<CurrencyHistoryProcessModel>> GetCurrencyHistoryProcesses(DataSource dataSource);
    Task UpdateIntervalsBack(Guid currencyHistoryProcessId, int intervalsBack, string lastChangedBy);
}