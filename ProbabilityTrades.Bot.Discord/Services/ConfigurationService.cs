namespace ProbabilityTrades.Bot.Discord.Services;

public static class ConfigurationService
{
    /// <summary>
    ///     Documentation: https://github.com/serilog/serilog/wiki/Configuration-Basics
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, LoggerConfiguration) =>
        {
            LoggerConfiguration
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.MSSqlServer(
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    connectionString: context.Configuration.GetConnectionString("ApplicationDatabaseSqlServer"),
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "Log_DiscordBot",
                        AutoCreateSqlTable = true
                    });
        });
    }

    public static IHostBuilder ConfigureServices(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureServices((hostContext, services) =>
        {
            var applicationConnectionString = hostContext.Configuration.GetConnectionString("ApplicationDatabaseSqlServer");
            if (!string.IsNullOrEmpty(applicationConnectionString))
                services.AddDatabaseContext<ApplicationDbContext>(applicationConnectionString, ServiceLifetime.Scoped);

            services.AddScheduler();

            services.AddTransient<NotificationProcess>();
            services.AddTransient<SubscriptionProcess>();

            services.AddTransient<IDiscordNotificationService, DiscordNotificationService>();
            services.AddTransient<IMovingAverageService, MovingAverageService>();
            services.AddTransient<ISubscriptionService, SubscriptionService>();

            services.AddTransient<DiscordClient>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var discordClient = new DiscordClient(new DiscordConfiguration
                {
                    Token = configuration["Discord:Token"],
                    TokenType = TokenType.Bot,
                    Intents = DiscordIntents.All       ,
                    AutoReconnect = true,
                });

                discordClient.UseSlashCommands(new SlashCommandsConfiguration { Services = serviceProvider })
                             .RegisterCommands<ServerCommands>();

                return discordClient;
            });
            //services.ConfigureDiscord();

            services.AddHostedService<Worker>();
        });
    }
}