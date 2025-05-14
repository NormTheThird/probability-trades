namespace ProbabilityTrades.Console.Services;

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
        return hostBuilder.UseSerilog((context, LoggerConfiguration) => LoggerConfiguration
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            .Enrich.FromLogContext()
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
            //.Destructure.ByTransforming<GetOHLCVResponse>(response =>
            //    new { response.Success, response.ErrorMessage, response.CandleCycles.Count })
            //.WriteTo.MSSqlServer(
            //    restrictedToMinimumLevel: LogEventLevel.Information,
            //    connectionString: context.Configuration.GetConnectionString("ApplicationDatabaseSqlServer"),
            //    sinkOptions: new MSSqlServerSinkOptions
            //    {
            //        TableName = "CurrencyHistoryWorkerLog",
            //        AutoCreateSqlTable = true
            //    })
            );
    }

    public static IHostBuilder ConfigureServices(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureServices((hostContext, services) =>
        {
            services.AddDatabaseContext<ApplicationDbContext>(hostContext.Configuration.GetConnectionString("ApplicationDatabaseSqlServer"), ServiceLifetime.Scoped);
            services.AddDatabaseContext<CurrencyHistoryDbContext>(hostContext.Configuration.GetConnectionString("CurrencyHistoryDatabaseSqlServer"), ServiceLifetime.Scoped);

            services.AddTransient<ICalculatePumpService, CalculatePumpService>();
            services.AddTransient<ICurrencyHistoryService, KucoinService>();
            services.AddTransient<IExchangeApiService, KrakenApiService>();
            services.AddTransient<IIndicatorAnalysisService, IndicatorAnalysisService>();
            services.AddTransient<IMarketService, MarketService>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IMovingAverageService, MovingAverageService>();
        });
    }
}