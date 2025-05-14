namespace ProbabilityTrades.Bot.Discord;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly DiscordClient _discordClient;

    public Worker(ILogger<Worker> logger, DiscordClient discordClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _discordClient = discordClient ?? throw new ArgumentNullException(nameof(discordClient));
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Started Worker");
        _discordClient.MessageCreated += OnMessageCreated;
        var registeredCommands = _discordClient.GetSlashCommands().RegisteredCommands;
        await _discordClient.ConnectAsync();
    }

    private async Task OnMessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e)
    {
        if (e.Message.Content == "Ping")
        {
            await e.Message.RespondAsync("Pong");
        }
        else if (e.Message.Content == "Jason Is")
        {
            await e.Message.RespondAsync("LAME!!!!!!");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stopped Worker");
        _discordClient.MessageCreated -= OnMessageCreated;
        await _discordClient.DisconnectAsync();
        _discordClient.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) => await Task.CompletedTask;
}