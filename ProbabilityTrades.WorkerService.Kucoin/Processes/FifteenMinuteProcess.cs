namespace ProbabilityTrades.WorkerService.Kucoin.Processes;

public class FifteenMinuteProcess : BaseProcess<FifteenMinuteProcess>, IInvocable
{
    public FifteenMinuteProcess(IConfiguration config, ILogger<FifteenMinuteProcess> logger, ICalculatePumpService calculatePumpService,
                                ICurrencyHistoryService currencyHistoryService, ICurrencyHistoryProcessService currencyHistoryProcessService,
                                IDiscordNotificationService discordNotificationService, IExchangeApiService exchangeApiService,
                                IIndicatorAnalysisService indicatorAnalysisService, IMarketService marketService, IMailService mailService,
                                IMovingAverageService movingAverageService)
        : base(DataSource.Kucoin, config, logger, CandlestickPattern.FifteenMinute, calculatePumpService, currencyHistoryService, currencyHistoryProcessService,
               discordNotificationService, exchangeApiService, indicatorAnalysisService, mailService, marketService, movingAverageService)
    { }

    public async Task Invoke()
    {
        _logger.LogInformation($"{_candlestickPattern}: Started");

        await UpdateCurrencyHistoryAsync();

        _logger.LogInformation($"{_candlestickPattern}: Completed");
    }
}