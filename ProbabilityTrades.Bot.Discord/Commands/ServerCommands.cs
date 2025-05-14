namespace ProbabilityTrades.Bot.Discord.Commands;

public class ServerCommands : ApplicationCommandModule
{
    //public IMovingAverageService _movingAverageService { private get; set; }
    private readonly ILogger<ServerCommands> _logger;
    private readonly DiscordClient _discordClient;
    private readonly IMovingAverageService _movingAverageService;

    //public ServerCommands(ILogger<ServerCommands> logger, DiscordClient discordClient)
    //{
    //    _logger = logger;
    //    _discordClient = discordClient;
    //    _movingAverageService = null;
    //}

    public ServerCommands(ILogger<ServerCommands> logger, DiscordClient discordClient, IMovingAverageService movingAverageService)
    {
        _logger = logger;
        _discordClient = discordClient;
        _movingAverageService = movingAverageService;
    }


    [SlashCommand("test", "Sends a test message")]
    public async Task Test(InteractionContext context) =>
        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new()  {   Content = "Jason is super lame" });


    [SlashCommand("status", "Gets the current status of any asset we are tracking")]
    public async Task Status(InteractionContext context) =>
        await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new() { Content = await GetStatusContent() });

    private async Task<string> GetStatusContent()
    {
        try
        {
            var content = "Test";
            //var currentMovingAveragePositions = await _movingAverageService.GetCurrentMovingAveragePositionsAsync(DataSource.Kucoin);
            //foreach (var movingAveragePosition in currentMovingAveragePositions)
            //    content += $"{movingAveragePosition.BaseCurrency}-{movingAveragePosition.QuoteCurrency} is in {movingAveragePosition.MarketPosition} as of {movingAveragePosition.CloseDate}\n";

            return content;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}