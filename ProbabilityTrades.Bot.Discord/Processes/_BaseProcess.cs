namespace ProbabilityTrades.Bot.Discord.Processes;

public abstract class BaseProcess<T>
{
    readonly internal IConfiguration _configuration;
    readonly internal ILogger<T> _logger;
    readonly internal DiscordClient _discordClient;

    public BaseProcess(IConfiguration configuration, ILogger<T> logger, DiscordClient discordClient)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _discordClient = discordClient ?? throw new ArgumentNullException(nameof(discordClient));
    }
}