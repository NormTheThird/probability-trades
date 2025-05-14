namespace ProbabilityTrades.WorkerService.Kraken.Processes;

public class DailyProcess : BaseProcess<DailyProcess>, IInvocable
{
    public DailyProcess(IConfiguration config, ILogger<DailyProcess> logger, ICalculatePumpService calculatePumpService,
                        ICurrencyHistoryService currencyHistoryService, ICurrencyHistoryProcessService currencyHistoryProcessService,
                        IDiscordNotificationService discordNotificationService, IExchangeApiService exchangeApiService,
                        IIndicatorAnalysisService indicatorAnalysisService, IMarketService marketService, IMailService mailService,
                        IMovingAverageService movingAverageService)
        : base(DataSource.Kraken, config, logger, CandlestickPattern.OneDay, calculatePumpService, currencyHistoryService, currencyHistoryProcessService,
               discordNotificationService, exchangeApiService, indicatorAnalysisService, mailService, marketService, movingAverageService)
    { }

    public async Task Invoke()
    {
        _logger.LogInformation($"{_candlestickPattern}: Started");

        await UpdateMarketValuesAsync();

        await UpdateCurrencyHistoryAsync();

        await MoveKucoinRecordsToKucoinArchiveAsync();

        await CalculateMovingAverageAsync();

        _logger.LogInformation($"{_candlestickPattern}: Completed");
    }
}