namespace ProbabilityTrades.WorkerService.Kraken.Services;

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
                        TableName = "Log_WorkerService_Kucoin",
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

            var currencyHistoryConnectionString = hostContext.Configuration.GetConnectionString("CurrencyHistoryDatabaseSqlServer");
            if (!string.IsNullOrEmpty(currencyHistoryConnectionString))
                services.AddDatabaseContext<CurrencyHistoryDbContext>(currencyHistoryConnectionString, ServiceLifetime.Scoped);

            services.AddScheduler();
            services.AddTransient<FiveMinuteProcess>();
            services.AddTransient<FifteenMinuteProcess>();
            services.AddTransient<HourlyProcess>();
            services.AddTransient<DailyProcess>();

            services.AddTransient<ICalculatePumpService, CalculatePumpService>();
            services.AddTransient<ICurrencyHistoryService, KucoinService>();
            services.AddTransient<ICurrencyHistoryProcessService, CurrencyHistoryProcessService>();
            services.AddTransient<IDiscordNotificationService, DiscordNotificationService>();
            services.AddTransient<IExchangeApiService, KucoinApiService>();
            services.AddTransient<IIndicatorAnalysisService, IndicatorAnalysisService>();
            services.AddTransient<IMarketService, MarketService>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IMovingAverageService, MovingAverageService>();
        });
    }
}