namespace ProbabilityTrades.Domain.Processes;

public abstract class BaseProcess<T>
{
    private readonly DataSource _dataSource;
    private readonly IConfiguration _config;
    public readonly ILogger<T> _logger;
    public readonly CandlestickPattern _candlestickPattern = CandlestickPattern.Unknown;
    private readonly ICalculatePumpService _calculatePumpService;
    private readonly ICurrencyHistoryService _currencyHistoryService;
    private readonly ICurrencyHistoryProcessService _currencyHistoryProcessService;
    private readonly IDiscordNotificationService _discordNotificationService;
    private readonly IExchangeApiService _exchangeApiService;
    private readonly IIndicatorAnalysisService _indicatorAnalysisService;
    private readonly IMailService _mailService;
    private readonly IMarketService _marketService;
    private readonly IMovingAverageService _movingAverageService;
    private readonly string _workerServiceName;

    public BaseProcess(DataSource dataSource, IConfiguration config, ILogger<T> logger, CandlestickPattern candlestickPattern,
                       ICalculatePumpService calculatePumpService, ICurrencyHistoryService currencyHistoryService, ICurrencyHistoryProcessService currencyHistoryProcessService,
                       IDiscordNotificationService discordNotificationService, IExchangeApiService exchangeApiService, IIndicatorAnalysisService indicatorAnalysisService,
                       IMailService mailService, IMarketService marketService, IMovingAverageService movingAverageService)
    {
        _dataSource = dataSource;
        if (_dataSource.Equals(DataSource.Unknown))
            throw new ArgumentNullException(nameof(dataSource));

        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _candlestickPattern = candlestickPattern;
        _calculatePumpService = calculatePumpService ?? throw new ArgumentNullException(nameof(calculatePumpService));
        _currencyHistoryService = currencyHistoryService ?? throw new ArgumentNullException(nameof(currencyHistoryService));
        _currencyHistoryProcessService = currencyHistoryProcessService ?? throw new ArgumentNullException(nameof(currencyHistoryProcessService));
        _discordNotificationService = discordNotificationService ?? throw new ArgumentNullException(nameof(discordNotificationService));
        _exchangeApiService = exchangeApiService ?? throw new ArgumentNullException(nameof(exchangeApiService));
        _indicatorAnalysisService = indicatorAnalysisService ?? throw new ArgumentNullException(nameof(indicatorAnalysisService));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _marketService = marketService ?? throw new ArgumentNullException(nameof(marketService));
        _movingAverageService = movingAverageService ?? throw new ArgumentNullException(nameof(movingAverageService));
        _workerServiceName = $"Worker Service {candlestickPattern}";
    }

    /// <summary>
    ///     Gets all the active processes for the candlestick pattern and loops through them to update the currency history.
    /// </summary>
    /// <returns></returns>
    public async Task UpdateCurrencyHistoryAsync()
    {
        // Delay 5 seconds to allow kucoin record to be written to history before we get it.
        await Task.Delay(5000);
        var tokenModel = GetExchangeTokenModel();

        // Get all active currency history processes and loop through the ones that match the candlestick pattern.
        var processes = await _currencyHistoryProcessService.GetCurrencyHistoryProcesses(_dataSource);
        var activeCandleStickProcesses = processes.Where(_ => _.IsActive && _.CandlestickPattern.Equals(_candlestickPattern)).ToList();
        foreach (var process in activeCandleStickProcesses)
        {
            // Default to 5 intervals back unless specified in the currency history process table.
            var intervalsBack = process.IntervalsBack <= 0 ? 20 : process.IntervalsBack;

            try
            {
                _logger.LogInformation($"{_candlestickPattern}: UpdateKucoin [{process.BaseCurrency}-{process.QuoteCurrency}, {intervalsBack}]");
                var candleCycleDetails = _candlestickPattern switch
                {
                    CandlestickPattern.FiveMinute => await _exchangeApiService.Get5MinOHLCVAsync(tokenModel, $"{process.BaseCurrency}-{process.QuoteCurrency}", intervalsBack),
                    CandlestickPattern.FifteenMinute => await _exchangeApiService.Get15MinOHLCVAsync(tokenModel, $"{process.BaseCurrency}-{process.QuoteCurrency}", intervalsBack),
                    CandlestickPattern.OneHour => await _exchangeApiService.GetHourlyOHLCVAsync(tokenModel, $"{process.BaseCurrency}-{process.QuoteCurrency}", intervalsBack),
                    CandlestickPattern.OneDay => await _exchangeApiService.GetDailyOHLCVAsync(tokenModel, $"{process.BaseCurrency}-{process.QuoteCurrency}", intervalsBack),
                    _ => throw new NotImplementedException()
                };

                await UpdateKucoinAsync(process.BaseCurrency, process.QuoteCurrency, candleCycleDetails);

                if (process.IntervalsBack != 0)
                    await _currencyHistoryProcessService.UpdateIntervalsBack(process.Id, 0, _workerServiceName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{process.BaseCurrency}-{process.QuoteCurrency}] " + ex.Message);
                continue;
            }
        }
    }

    /// <summary>
    ///     Move Kucoin records to KucoinArchive table
    /// </summary>
    /// <returns>Task</returns>
    public async Task MoveKucoinRecordsToKucoinArchiveAsync()
    {
        _logger.LogInformation($"Move Kucoin Records To KucoinArchive");
        await _currencyHistoryService.MoveRecordsToArchiveAsync();
    }

    /// <summary>
    ///     Calculate the moving averages for the specified currency pair.
    /// </summary>
    /// <param name="baseCurrency"></param>
    /// <param name="quoteCurrency"></param>
    /// <param name="candleCycleDetails"></param>
    /// <returns></returns>
    public async Task CalculateMovingAverageAsync()
    {
        var movingAverageConfigurations = await _movingAverageService.GetMovingAverageConfigurationsAsync(_dataSource);
        var activeMovingAverageConfigurations = movingAverageConfigurations.Where(_ => _.IsActive).ToList();
        var message = string.Empty;
        foreach (var configuration in activeMovingAverageConfigurations)
        {
            try
            {
                var currentRecord = await _currencyHistoryService.GetCurrentHistoryRecordAsync(configuration.BaseCurrency, configuration.QuoteCurrency, configuration.CandlestickPattern)
                    ?? throw new KeyNotFoundException($"No current record found for {configuration.BaseCurrency}-{configuration.QuoteCurrency}.");

                var shortMovingAverageDays = configuration.ShortMovingAverageDays;
                var longMovingAverageDays = configuration.LongMovingAverageDays;

                var baseDataModel = new BaseDataModel
                {
                    DataSource = _dataSource,
                    BaseCurrency = configuration.BaseCurrency,
                    QuoteCurrency = configuration.QuoteCurrency,
                    CandlestickPattern = _candlestickPattern
                };

                var movingAverageStatuses = await _movingAverageService.GetMovingAverageStatusesAsync(baseDataModel, 1);
                var lastMarketPosition = movingAverageStatuses.FirstOrDefault()?.MarketPosition ?? MarketPosition.Unknown;

                var movingAveragePosition = await _indicatorAnalysisService.CalculateMovingAveragePositionAsync(baseDataModel, shortMovingAverageDays, longMovingAverageDays);
                var newMarketPosition = movingAveragePosition.MarketPosition;

                var movingAverageStatusModel = new MovingAverageStatusModel
                {
                    DataSource = _dataSource,
                    BaseCurrency = configuration.BaseCurrency,
                    QuoteCurrency = configuration.QuoteCurrency,
                    CloseDate = DateOnly.FromDateTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(currentRecord.ChartTimeEpoch)),
                    ChartTimeEpoch = currentRecord.ChartTimeEpoch,
                    CandlestickPattern = _candlestickPattern,
                    MarketPosition = movingAveragePosition.MarketPosition,
                    ClosePrice = currentRecord.ClosingPrice,
                    ShortMovingAverageDays = shortMovingAverageDays,
                    ShortMovingAverage = movingAveragePosition.ShortMovingAverage,
                    LongMovingAverageDays = longMovingAverageDays,
                    LongMovingAverage = movingAveragePosition.LongMovingAverage
                };
                await _movingAverageService.CreateMovingAverageStatusAsync(movingAverageStatusModel);

                // Check if the marker position has changed
                if (newMarketPosition != lastMarketPosition)
                {
                    // Check if we should send a Discord notification
                    if (configuration.IsSendDiscordNotification)
                    {
                        var discordNotification = new DiscordNotificationModel
                        {
                            ChannelId = (ulong)DiscordChannel.TradeSignals,
                            Channel = DiscordChannel.TradeSignals.GetDescription(),
                            NotificationType = "MovingAverages",
                            NotificationColor = GetMovingAverageDiscordNotificationColor(newMarketPosition, lastMarketPosition),
                            Author = "",
                            Title = $"{configuration.BaseCurrency}/{configuration.QuoteCurrency}",
                            Message = CreateMovingAveragePostionDiscordMessage(newMarketPosition, lastMarketPosition, currentRecord.ClosingPrice.GetCurrencySpecificRounding(baseDataModel.BaseCurrency)),
                            Footer = $"Moving Averages {shortMovingAverageDays} {longMovingAverageDays}"
                        };

                        await _discordNotificationService.CreateDiscordNotificationAsync(discordNotification);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"CalculateMovingAverageAsync: [{configuration.BaseCurrency}-{configuration.QuoteCurrency}] {ex.Message}");
                throw;
            }
        }
    }

    /// <summary>
    ///     Get the exchange tokens for the Kucoin exchange
    ///     
    ///     TODO: TREY: 2023.07.24 Figure out how to apply tokens in ConfigureServices.
    /// </summary>
    /// <returns></returns>
    public ExchangeTokenModel GetExchangeTokenModel()
    {
        return new ExchangeTokenModel
        {
            ApiKey = _config["Kucoin:ApiKey"],
            ApiSecret = _config["Kucoin:ApiSecret"],
            ApiPassphrase = _config["Kucoin:ApiPassphrase"]
        };
    }

    public async Task UpdateMarketValuesAsync()
    {
        var tokenModel = GetExchangeTokenModel();
        var markets = await _exchangeApiService.GetMarketsAsync(tokenModel);
        foreach (var market in markets)
        {
            var newId = await _marketService.CreateMarketAsync(_dataSource, market, "CurrencyHistoryProcess.DailyProcess");
            if (newId == null)
                _logger.LogError("SaveMarketAsync: {@response}", "");
        }

        _logger.LogInformation("Updated {@marketCount} markets from {@dataSource}", markets.Count, _dataSource);
    }


    /// <summary>
    ///     Update Kucoin records up to 1000 at at time.
    /// </summary>
    /// <param name="baseCurrency">The base currency of the records</param>
    /// <param name="quoteCurrency"></param>
    /// <param name="candleCycleDetails"></param>
    /// <returns></returns>
    private async Task UpdateKucoinAsync(string baseCurrency, string quoteCurrency, List<CandleCycleDetail> candleCycleDetails)
    {
        _logger.LogInformation($"Records Found: {candleCycleDetails.Count}");
        var createCurrencyHistoryModelList = new List<CreateCurrencyHistoryModel>();
        var totalCount = 0;
        foreach (var candleCycleDetail in candleCycleDetails)
        {
            createCurrencyHistoryModelList.Add(new CreateCurrencyHistoryModel
            {
                BaseCurrency = baseCurrency,
                QuoteCurrency = quoteCurrency,
                CandlestickPattern = _candlestickPattern,
                ChartTimeEpoch = candleCycleDetail.CycleTimeEpoch,
                ChartTimeUTC = DateTimeOffset.FromUnixTimeSeconds(candleCycleDetail.CycleTimeEpoch),
                OpeningPrice = candleCycleDetail.OpeningPrice,
                ClosingPrice = candleCycleDetail.ClosingPrice,
                HighestPrice = candleCycleDetail.HighestPrice,
                LowestPrice = candleCycleDetail.LowestPrice,
                Turnover = candleCycleDetail.Turnover,
                Volume = candleCycleDetail.Volume
            });

            if (createCurrencyHistoryModelList.Count != 0 && createCurrencyHistoryModelList.Count % 1000 == 0)
            {
                totalCount += createCurrencyHistoryModelList.Count;
                await _currencyHistoryService.CreateFromListAsync(baseCurrency, quoteCurrency, _candlestickPattern, createCurrencyHistoryModelList, _workerServiceName);
                createCurrencyHistoryModelList.Clear();
                _logger.LogInformation($"Completed: {totalCount} details");
            }
        }

        if (createCurrencyHistoryModelList.Count != 0)
        {
            totalCount += createCurrencyHistoryModelList.Count;
            await _currencyHistoryService.CreateFromListAsync(baseCurrency, quoteCurrency, _candlestickPattern, createCurrencyHistoryModelList, _workerServiceName);
            createCurrencyHistoryModelList.Clear();
            _logger.LogInformation($"Completed: {totalCount} details");
        }
    }

    /// <summary>
    ///     Gets the color for the moving average discord notification
    /// </summary>
    /// <param name="newMarketPosition">The new market position</param>
    /// <param name="lastMarketPosition">The old market position</param>
    /// <returns></returns>
    private static string GetMovingAverageDiscordNotificationColor(MarketPosition newMarketPosition, MarketPosition lastMarketPosition)
    {
        switch (newMarketPosition)
        {
            case MarketPosition.Long:
                // Green(Enter Long Trade)
                return "#00FF00"; 
            case MarketPosition.Cash when lastMarketPosition == MarketPosition.Long:
                // Blue(Exit Long Trade)
                return "#0000FF"; 
            case MarketPosition.Short:
                // Red(Enter Short Trade)
                return "#FF0000"; 
            case MarketPosition.Cash when lastMarketPosition == MarketPosition.Short:
                // Yellow(Cover Short Trade) 
                return "#FFFF00"; 
            default:
                // Default color (Gray) for unhandled cases
                return "#808080"; 
        }
    }

    /// <summary>
    ///     Create the message to send to Discord.
    /// </summary>
    /// <param name="newMarketPosition">The new market position</param>
    /// <param name="lastMarketPosition">The old market position</param>
    /// <param name="closePrice">The close price</param>
    /// <returns>string</returns>
    /// <exception cref="NotImplementedException"></exception>
    private static string CreateMovingAveragePostionDiscordMessage(MarketPosition newMarketPosition, MarketPosition lastMarketPosition, decimal closePrice)
    {
        // In BTC(LONG) and I need to get out. Sell Long
        if (lastMarketPosition.Equals(MarketPosition.Long) && newMarketPosition.Equals(MarketPosition.Cash))
            return $"Sell LONG @ ${closePrice}";

        // In BTC(SHORT) and I need to get out. Cover Short
        else if (lastMarketPosition.Equals(MarketPosition.Short) && newMarketPosition.Equals(MarketPosition.Cash))
            return $"COVER SHORT @ ${closePrice}";

        // Not In BTC and I want to buy. Buy Long
        else if (lastMarketPosition.Equals(MarketPosition.Cash) && newMarketPosition.Equals(MarketPosition.Long))
            return $"BUY LONG @ ${closePrice}";

        // Not in BTC and I want to short. Short
        else if (lastMarketPosition.Equals(MarketPosition.Cash) && newMarketPosition.Equals(MarketPosition.Short))
            return $"SHORT @ ${closePrice}";

        else
            return $"Unkown message for last status of {lastMarketPosition} and new status of {newMarketPosition}";
    }
}