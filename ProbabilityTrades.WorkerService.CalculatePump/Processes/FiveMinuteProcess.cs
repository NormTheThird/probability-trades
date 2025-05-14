namespace ProbabilityTrades.WorkerService.CalculatePump.Processes;

public class FiveMinuteProcess : BaseProcess<FiveMinuteProcess>, IInvocable
{
    public FiveMinuteProcess(ILogger<FiveMinuteProcess> logger, ICalculatePumpService calculatePumpService, IExchangeApiService exchangeApiService,
                             IIndicatorAnalysisService indicatorAnalysisService, IMailService mailService)
        : base(logger, calculatePumpService, exchangeApiService, indicatorAnalysisService, mailService)
    { }

    public async Task Invoke()
    {
        _logger.LogInformation($"Calculate Pump Started");

        var tasks = new List<Task>
        {
            //Task.Run(async () => await CheckStopLossStatusAsync()),
            Task.Run(async () => await CalculatePumpStatusesAsync())
        };

        await Task.WhenAll(tasks);

        _logger.LogInformation($"Calculate Pump  Completed");
    }
}