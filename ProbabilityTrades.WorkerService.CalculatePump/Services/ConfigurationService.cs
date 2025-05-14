namespace ProbabilityTrades.WorkerService.CalculatePump.Services;

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
                        TableName = "Log_WorkerService_CalculatePump",
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
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(applicationConnectionString), ServiceLifetime.Transient);
            }


            var currencyHistoryConnectionString = hostContext.Configuration.GetConnectionString("CurrencyHistoryDatabaseSqlServer");
            if (!string.IsNullOrEmpty(currencyHistoryConnectionString))
            {
                services.AddDbContext<CurrencyHistoryDbContext>(options => options.UseSqlServer(currencyHistoryConnectionString), ServiceLifetime.Transient);
            }

            services.AddScheduler();
            services.AddScoped<FiveMinuteProcess>();
            services.AddScoped<NightlyProcess>();

            services.AddTransient<ICalculatePumpService, CalculatePumpService>();
            services.AddTransient<ICurrencyHistoryService, KucoinService>();
            //services.AddTransient<IExchangeApiService, KucoinApiService>();
            services.AddTransient<IIndicatorAnalysisService, IndicatorAnalysisService>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IMovingAverageService, MovingAverageService>();
        });
    }
}