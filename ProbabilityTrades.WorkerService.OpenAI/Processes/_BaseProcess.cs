namespace ProbabilityTrades.WorkerService.OpenAI.Processes;

public abstract class BaseProcess<T>
{
    readonly internal IConfiguration _configuration;
    readonly internal ILogger<T> _logger;

    private readonly string _workerServiceName = "Open AI Worker Service";
    private readonly IMailService _mailService;

    public BaseProcess(IConfiguration configuration, ILogger<T> logger, IMailService mailService)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
    }
}