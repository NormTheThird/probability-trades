namespace ProbabilityTrades.WorkerService.OpenAI.Processes;

public class NightlyProcess : BaseProcess<NightlyProcess>, IInvocable
{
    private readonly IOpenAIApiInterface _openAIApiInterface;

    public NightlyProcess(IConfiguration configuration, ILogger<NightlyProcess> logger, IMailService mailService,
                          IOpenAIApiInterface openAIApiInterface)
        : base(configuration, logger, mailService)
    {
        _openAIApiInterface = openAIApiInterface ?? throw new ArgumentNullException(nameof(openAIApiInterface));
    }

    public async Task Invoke()
    {
        try
        {
            _logger.LogInformation($"Nightly Process Started");

            await _openAIApiInterface.Test();

            _logger.LogInformation($"Nightly Process Completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Nightly Process Failed");
        }

    }
}