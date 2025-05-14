namespace ProbabilityTrades.WorkerService.CalculatePump.Processes;

public class NightlyProcess : BaseProcess<NightlyProcess>, IInvocable
{
    public NightlyProcess(ILogger<NightlyProcess> logger, ICalculatePumpService calculatePumpService, IExchangeApiService exchangeApiService, 
                          IIndicatorAnalysisService indicatorAnalysisService, IMailService mailService)     
        : base(logger, calculatePumpService, exchangeApiService, indicatorAnalysisService, mailService)
    { }

    public async Task Invoke()
    {
        _logger.LogInformation($"Nightly Process Started");

        await CloseAllOpenPumpOrdersAsync();

        _logger.LogInformation($"Nightly Process Completed");
    }
}