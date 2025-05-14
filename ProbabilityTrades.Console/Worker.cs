using QuantConnect.Algorithm;
using QuantConnect.Configuration;

namespace ProbabilityTrades.Console;

public class Worker
{
    private readonly ILogger<Worker> _logger;
    private readonly ICalculatePumpService _calculatePumpService;
    private readonly ICurrencyHistoryService _currencyHistoryService;
    private readonly IExchangeApiService _exchangeApiService;
    private readonly IIndicatorAnalysisService _indicatorAnalysisService;
    private readonly IMailService _mailService;


    public Worker(ILogger<Worker> logger, ICalculatePumpService calculatePumpService, ICurrencyHistoryService currencyHistoryService,
                  IExchangeApiService exchangeApiService, IIndicatorAnalysisService indicatorAnalysisService, IMailService mailService)
    {
        _logger = logger;
        _calculatePumpService = calculatePumpService ?? throw new ArgumentNullException(nameof(calculatePumpService));
        _currencyHistoryService = currencyHistoryService ?? throw new ArgumentNullException(nameof(currencyHistoryService));
        _exchangeApiService = exchangeApiService ?? throw new ArgumentNullException(nameof(exchangeApiService));
        _indicatorAnalysisService = indicatorAnalysisService ?? throw new ArgumentNullException(nameof(indicatorAnalysisService));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
    }

    public async Task RunAsync()
    {
        try
        {
            QCAlgorithm _algorithm;


            // Initialize the configuration
            Config.Set("data-folder", "../../../Data");
            Config.Set("cache-location", "cache");
            Config.Set("data-directory", "../../../Data");
            Config.Set("api-access-token", "YOUR_API_TOKEN");
            Config.Set("version-id", "1.0.0.0");
            Config.Set("job-user-id", "YOUR_USER_ID");
            Config.Set("project-id", "YOUR_PROJECT_ID");
            Config.Set("job-organization-id", "YOUR_ORG_ID");
            Config.Set("results-destination-folder", "Results");
            Config.Set("plugin-directory", "Plugins");
            Config.Set("composer-dll-directory", "C:\\path\\to\\dlls");

            _algorithm = new();
            _algorithm.Initialize();

            _logger.LogInformation("Hello, Quant!\n");

            var quantConnectTest = new QuantConnectTest();
        }
        catch (Exception ex)
        {

            throw ex;
        }


        //TestLogger();

        //await RunTest();

        // await KucoinCryptoPumpTrading();

        // await GetOHLCVAndUpdateKucoin("GMEE", "USDT", CandlestickPattern.OneMinute, 1400);

        //await RunScalpingStrategyAsync("BTC", "USDT", CandlestickPattern.OneMinute, 9, 50);

        //await TestScalpingStrategyPairAsync("BTC", "USDT", CandlestickPattern.OneMinute, 500, 3, 9);
        //await TestScalpingStrategyPairAsync("BTC", "USDT", CandlestickPattern.OneMinute, 500, 5, 9);
        //await TestScalpingStrategyPairAsync("BTC", "USDT", CandlestickPattern.OneMinute, 500, 5, 50);
        //await TestScalpingStrategyPairAsync("BTC", "USDT", CandlestickPattern.OneMinute, 1440, 9, 50);

        //await TestScalpingStrategyPairsAsync("BTC", "USDT", CandlestickPattern.OneMinute, 1440, GenerateMovingAveragePairs(GetCustomNumbers()));
        //await TestScalpingStrategyPairsAsync("ETH", "USDT", CandlestickPattern.OneMinute, 1440, GenerateMovingAveragePairs(GetCustomNumbers()));
        //await TestScalpingStrategyPairsAsync("ADA", "USDT", CandlestickPattern.OneMinute, 1440, GenerateMovingAveragePairs(GetCustomNumbers()));
        //await TestScalpingStrategyPairsAsync("SHIB", "USDT", CandlestickPattern.OneMinute, 1440, GenerateMovingAveragePairs(GetCustomNumbers()));

        _logger.LogInformation("Goodbye :(\n");

        System.Console.ReadKey();
    }

    private async Task GetOHLCVAndUpdateKucoin(string baseCurrency, string quoteCurrency, CandlestickPattern candlestickPattern, int intervalsBack)
    {
        try
        {
            var tokenModel = GetExchangeTokenModel();
            var symbol = $"{baseCurrency}-{quoteCurrency}";
            _logger.LogInformation($"{candlestickPattern}: UpdateKucoin [{symbol}, {intervalsBack}]");
            var candleCycleDetails = candlestickPattern switch
            {
                CandlestickPattern.OneMinute => await _exchangeApiService.Get1MinOHLCVAsync(tokenModel, symbol, intervalsBack),
                CandlestickPattern.FiveMinute => await _exchangeApiService.Get5MinOHLCVAsync(tokenModel, symbol, intervalsBack),
                CandlestickPattern.FifteenMinute => await _exchangeApiService.Get15MinOHLCVAsync(tokenModel, symbol, intervalsBack),
                CandlestickPattern.OneHour => await _exchangeApiService.GetHourlyOHLCVAsync(tokenModel, symbol, intervalsBack),
                CandlestickPattern.OneDay => await _exchangeApiService.GetDailyOHLCVAsync(tokenModel, symbol, intervalsBack),
                _ => throw new NotImplementedException()
            };

            await UpdateKucoinAsync(baseCurrency, quoteCurrency, candlestickPattern, candleCycleDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{baseCurrency}-{quoteCurrency}] " + ex.Message);
        }
    }

    private void TestLogger()
    {
        _logger.LogInformation("Information");
        _logger.LogWarning("Warning");
        _logger.LogCritical("Critical");
        _logger.LogDebug("Debug");
        _logger.LogError("Error");
        _logger.LogTrace("Trace");
    }

    private async Task RunTest()
    {
        var calculatePumpConfigurations = await _calculatePumpService.GetCalculatePumpConfigurationsAsync(DataSource.Kucoin, false);
        _logger.LogInformation("Running calculate pump test for {count} records\n", calculatePumpConfigurations.Count);
        foreach (var calculatePumpConfiguration in calculatePumpConfigurations)
        {
            await RunCalculatePumpTest(calculatePumpConfiguration.BaseCurrency, calculatePumpConfiguration.QuoteCurrency, CandlestickPattern.OneHour, false);// calculatePumpConfiguration.IsCurrentlyPumping);
        }
    }

    private async Task RunCalculatePumpTest(string baseCurrency, string quoteCurrency, CandlestickPattern candlestickPattern, bool isCurrentlyPumping)
    {
        try
        {
            _logger.LogInformation("Running calculate pump test for {base}-{quote}\n", baseCurrency, quoteCurrency);

            // Get 200 intervals back and loop through each one.
            var currencyHistories = await _currencyHistoryService.GetHistoryRecordsByIntervalsBackAsync(baseCurrency, quoteCurrency, candlestickPattern, 200);
            var calculatePumpData = currencyHistories.OrderBy(_ => _.ChartTimeEpoch)
                                                     .Select(_ => new CalculatePumpDataModel
                                                     {
                                                         ChartTimeEpoch = _.ChartTimeEpoch,
                                                         HighPrice = _.HighestPrice,
                                                         LowPrice = _.LowestPrice,
                                                         ClosePrice = _.ClosingPrice,
                                                         Volume = _.Volume
                                                     })
                                                     .ToList();

            foreach (var data in calculatePumpData)
            {
                var pumpingPercentage = await _calculatePumpService.GetCalculatePumpPercentageTest(data);

                var isPumping = pumpingPercentage.Equals(100.00m);

                if (isPumping)
                {
                    _logger.LogInformation("ChartTime [{chartTime}] | Percent [{percent} : pumping]", data.ChartTimeEpoch, pumpingPercentage);

                }
                else
                {
                    _logger.LogInformation("ChartTime [{chartTime}] | Percent [{percent} : not pumping]", data.ChartTimeEpoch, pumpingPercentage);
                }
            }

            ////var adx = _calculatePumpService.TestCalculateADX(high, low, close);
            ////_logger.LogInformation("CalculateADX(14, {count}) - ADX: {adx}", filteredHistories.Count(), adx);

            ////var adxAndDx = _calculatePumpService.TestCalculateADXandDX(high, low, close);
            ////_logger.LogInformation("CalculateADXandDX(14, {count}) - ADX: {adx}, DIPlus: {diPlus}, DIMinus: {diMinus}", filteredHistories.Count(), adxAndDx.ADX, adxAndDx.DIPlus, adxAndDx.DIMinus);

            ////var dmi = _calculatePumpService.CalculateADX(high, low, close);
            ////_logger.LogInformation("CalculateADX(14, {count}) - ADX: {dmi}", filteredHistories.Count(), dmi);


            //var dmi = _calculatePumpService.TestCalculateADX2(high, low, close);
            //_logger.LogInformation("CalculateADX2(14, {count}) - ADX: {dmi}", filteredHistories.Count(), dmi);

            //System.Console.WriteLine();
        }
        catch (Exception ex)
        {
            _logger.LogError("{ex}\n", ex.Message);
        }

    }

    /// <summary>
    ///     Documentation: https://justpaste.it/6q46b
    ///     
    ///     1. Create an account on https://www.kucoin.com/ or sign up through their app. Kucoin allows users from all 
    ///        around the world making it the best place to trade/pump altcoins to large audiences.
    ///     2. Have USDT available to trade with. You can either deposit USDT directly or deposit any crypto then trade 
    ///        it for USDT. In order to join the pump you must be trading in the USDT market.
    ///     3. Make sure your funds are in your "Trading Account" not the Main or Futures account. In order to be used 
    ///        for trading your USDT need to be in your trading wallet.
    ///     4. On the day of the pump have Kucoin open and discord/telegram so you are ready to buy as soon as the coin 
    ///        is posted. Here is a shortcut to the spot trading market https://trade.kucoin.com/
    ///     5. Now you are ready to pump! The coin, expected gain, and any coin news will be posted in the #pump-signals 
    ///        channel at the scheduled time. Buy the target coin as soon it is posted, all of us buying will begin to 
    ///        increase the price and start the pump. Using a 75% market buy is strongly encouraged for quickness and 
    ///        simplicity. This will use 75% of your balance to buy the lowest priced available coins. As the price is 
    ///        rising a limit order buy might not fill so avoid it.
    ///     6. Do not use a market sell to sell during a pump, this will sell dump your coins for whatever price at or 
    ///        below the current level and may cost you a portion of your profit. We do not want to sell the price lower 
    ///        we want to keep it rising. Sell your coins using a limit sell. Either calculate where you want to sell by 
    ///        using the "expected gain" or wait until we reach the target and click a red sell order to copy its price.
    /// </summary>
    /// <returns></returns>
    private async Task KucoinCryptoPumpTrading()
    {
        var token = "ETH";
        var funds = 5.00m;
        var percentUp = 300.0m;


        var symbol = $"{token}-USDT";
        var exchangeTokens = GetExchangeTokenModel();

        // Buy Order
        //var createdOrder = await _exchangeApiService.CreateMarginMarketOrderAsync(exchangeTokens, true, symbol, funds)
        //    ?? throw new Exception($"Unable to create margin market buy order for {symbol}");
        //_logger.LogInformation("CreateMarginMarketBuyOrder: Created margin market buy order {orderId} for {symbol}", createdOrder.OrderId, symbol);

        var accounts = await _exchangeApiService.GetAccountsAsync(exchangeTokens, AccountType.Margin);
        var tokenAccount = accounts.FirstOrDefault(_ => _.Currency.Equals(token))
            ?? throw new Exception("USDT account does not exist");

        // Validate Order
        var buyPrice = 30260.00m;
        var sellPrice = 30260.00m;

        // Watch Price and Get Out Of Order
        var openOrder = tokenAccount.Available > 0;
        while (openOrder)
        {
            var marketData = await _exchangeApiService.GetMarketDataAsync(exchangeTokens, symbol);
            _logger.LogInformation("Market Data: [Price: {price}] [Ask: {ask}] [Bid: {bid}]", marketData.Price, marketData.BestAsk, marketData.BestBid);

            if (marketData.Price >= sellPrice)
            {
                // Sell Order
                var createdLimitSellOrder = await _exchangeApiService.CreateMarginLimitSellOrderAsync(exchangeTokens, symbol, sellPrice, funds)
                    ?? throw new Exception($"Unable to create margin market sell order for {symbol}");
                _logger.LogInformation("CreateMarginLimitSellOrderAsync: Created margin market sell order {orderId} for {symbol}", createdLimitSellOrder.OrderId, symbol);
                openOrder = false;
            }
        }
    }


    private async Task UpdateKucoinAsync(string baseCurrency, string quoteCurrency, CandlestickPattern candlestickPattern, List<CandleCycleDetail> candleCycleDetails)
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
                CandlestickPattern = candlestickPattern,
                ChartTimeEpoch = candleCycleDetail.CycleTimeEpoch,
                ChartTimeUTC = DateTimeOffset.FromUnixTimeSeconds(candleCycleDetail.CycleTimeEpoch),
                OpeningPrice = candleCycleDetail.OpeningPrice,
                ClosingPrice = candleCycleDetail.ClosingPrice,
                HighestPrice = candleCycleDetail.HighestPrice,
                LowestPrice = candleCycleDetail.LowestPrice,
                Turnover = candleCycleDetail.Turnover,
                Volume = candleCycleDetail.Volume
            });

            if (createCurrencyHistoryModelList.Count != 0 && createCurrencyHistoryModelList.Count % 500 == 0)
            {
                totalCount += createCurrencyHistoryModelList.Count;
                await _currencyHistoryService.CreateFromListAsync(baseCurrency, quoteCurrency, candlestickPattern, createCurrencyHistoryModelList, "Console");
                _logger.LogInformation($"Completed: {totalCount} details");
                createCurrencyHistoryModelList.Clear();
            }
        }

        if (createCurrencyHistoryModelList.Count != 0)
        {
            totalCount += createCurrencyHistoryModelList.Count;
            await _currencyHistoryService.CreateFromListAsync(baseCurrency, quoteCurrency, candlestickPattern, createCurrencyHistoryModelList, "Console");
        }

        _logger.LogInformation($"Completed: {createCurrencyHistoryModelList.Count} details");
    }


    private async Task RunScalpingStrategyAsync(string baseCurrency, string quoteCurrency, CandlestickPattern candlestickPattern, int shortMovingAverageDays, int longMovingAverageDays)
    {
        _logger.LogInformation($"RunScalpingStrategyAsync: {baseCurrency}-{quoteCurrency} [{candlestickPattern}] [{shortMovingAverageDays}] [{longMovingAverageDays}]");

        var tokenModel = GetExchangeTokenModel();
        var symbol = $"{baseCurrency}-{quoteCurrency}";
        var cashAmount = 1000.00m;
        var tradeBTC = 0.00m;
        var boughtPrice = 0.00m;
        var currentPossition = "Unknown";

        while (true)
        {
            var cycleDetails = await _exchangeApiService.Get1MinOHLCVAsync(tokenModel, symbol, longMovingAverageDays);
            var currentCycleDetail = cycleDetails.FirstOrDefault();
            var shortMovingAverage = Math.Round(cycleDetails.Take(shortMovingAverageDays).Average(_ => _.ClosingPrice), 2);
            var longMovingAverage = Math.Round(cycleDetails.Take(longMovingAverageDays).Average(_ => _.ClosingPrice), 2);
            var newPosition = shortMovingAverage > longMovingAverage ? "Above" : "Below";

            var closingPrice = currentCycleDetail?.ClosingPrice ?? 0.0m;
            if (newPosition == "Above")
            {
                // buy
                if (currentPossition == "Below")
                {
                    boughtPrice = closingPrice;
                    tradeBTC = Math.Round(cashAmount / closingPrice, 8);
                    cashAmount = 0.0m;
                    currentPossition = newPosition;
                }
                else if (currentPossition == "Unknown")
                {

                }
                else
                {
                    tradeBTC = Math.Round(cashAmount / closingPrice, 8);
                    currentPossition = newPosition;
                }

            }
            else if (newPosition == "Below")
            {
                if (currentPossition == "Above")
                {
                    // Sell
                    cashAmount = tradeBTC * closingPrice;
                    tradeBTC = 0.0m;
                }
                currentPossition = newPosition;
            }

            _logger.LogInformation("Cycle Time: {cycleTimeEpoch}, Closing Price: {closingPrice}, SMA: {sma}, LMA: {lma}, Position: {position}, Cash: {cash}, Trade: {trade}",
                currentCycleDetail?.CycleTimeEpoch, currentCycleDetail?.ClosingPrice, shortMovingAverage, longMovingAverage, currentPossition, cashAmount, tradeBTC);

            Task.Delay(60000).Wait();
        }

    }



    private async Task TestScalpingStrategyPairAsync(string baseCurrency, string quoteCurrency, CandlestickPattern candlestickPattern, int intervalsBack, int shortMovingAverageDays, int longMovingAverageDays, bool isShowDetail = false)
    {
        try
        {
            var symbol = $"{baseCurrency}-{quoteCurrency}";
            _logger.LogInformation("TestScalpingStrategyAsync: {symbol} [{candle}] [{intervals}] intervals at [{smaDays}] : [{lmaDays}]\n",
                symbol, candlestickPattern, intervalsBack, shortMovingAverageDays, longMovingAverageDays);

            var tokenModel = GetExchangeTokenModel();
            var totalIntervals = intervalsBack + longMovingAverageDays;

            var cycleDetails = await _exchangeApiService.Get1MinOHLCVAsync(tokenModel, symbol, totalIntervals);
            cycleDetails = cycleDetails.OrderBy(_ => _.CycleTimeEpoch).ToList();
            //cycleDetails.ForEach(_ => _logger.LogInformation("Cycle Time: {cycleTimeEpoch}, Closing Price: {closingPrice}", _.CycleTimeEpoch, _.ClosingPrice));

            var summary = TestScalpingStrategy(cycleDetails, shortMovingAverageDays, longMovingAverageDays, isShowDetail);

            _logger.LogInformation("Summary: Starting Cash {startingCash}, Ending Cash {endingCash}, Percent Changed {percentChanged}\n",
                summary.StartingCashAmount.ToString("0.00"), summary.EndingCashAmount.ToString("0.00"), summary.PercentChanged);

        }
        catch (Exception ex)
        {
            _logger.LogError("{ex}", ex.Message);
        }
    }

    private async Task TestScalpingStrategyPairsAsync(string baseCurrency, string quoteCurrency, CandlestickPattern candlestickPattern, int intervalsBack, List<(int, int)> pairs, bool isShowDetail = false)
    {
        try
        {
            var symbol = $"{baseCurrency}-{quoteCurrency}";
            _logger.LogInformation("TestScalpingStrategyAsync: {symbol} [{candle}] with {intervals} intervals\n", symbol, candlestickPattern, intervalsBack);

            var tokenModel = GetExchangeTokenModel();
            var maxLongMovingAverageDays = pairs.Max(_ => _.Item2);
            var totalIntervals = intervalsBack + maxLongMovingAverageDays;

            var cycleDetails = await _exchangeApiService.Get1MinOHLCVAsync(tokenModel, symbol, totalIntervals);
            cycleDetails = cycleDetails.OrderBy(_ => _.CycleTimeEpoch).ToList();

            var summaries = new List<SummaryModel>();
            foreach (var mapair in pairs)
            {
                var intervalCycleDetails = cycleDetails.Skip(maxLongMovingAverageDays - mapair.Item2).ToList();
                var summary = TestScalpingStrategy(intervalCycleDetails, mapair.Item1, mapair.Item2, isShowDetail);

                if (summary is not null)
                    summaries.Add(summary);
            }

            foreach (var summary in summaries.OrderByDescending(_ => _.PercentChanged).Take(10))
            {
                _logger.LogInformation("Summary for {sma} and {lma}: Starting Cash {startingCash}, Ending Cash {endingCash}, Percent Changed {percentChanged}",
                    summary.ShortMovingAverageDays, summary.LongMovingAverageDays, summary.StartingCashAmount.ToString("0.00"),
                    summary.EndingCashAmount.ToString("0.00"), summary.PercentChanged);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("{ex}", ex.Message);
        }
    }

    private SummaryModel TestScalpingStrategy(List<CandleCycleDetail> cycleDetails, int shortMovingAverageDays, int longMovingAverageDays, bool isShowDetail = false)
    {
        try
        {
            var skipCounter = 1;
            var tradePosition = "Cash";
            var previousAveragePosition = "Unknown";
            var startingCashAmount = 1000.00m;
            var cashAmount = startingCashAmount;
            var tradeAmount = 0.00m;
            var assetAmount = 0.00m;
            var boughtPrice = 0.00m;
            var percentChanged = 0.00m;
            foreach (var cycleDetail in cycleDetails.Skip(longMovingAverageDays))
            {
                var index = cycleDetails.IndexOf(cycleDetail);
                var intervalCycleDetails = cycleDetails.Skip(skipCounter).Take(longMovingAverageDays);
                var shortMovingAverage = Math.Round(intervalCycleDetails.Skip(longMovingAverageDays - shortMovingAverageDays).Average(_ => _.ClosingPrice), 2);
                var longMovingAverage = Math.Round(intervalCycleDetails.Average(_ => _.ClosingPrice), 2);
                var averagePosition = shortMovingAverage > longMovingAverage ? "Above" : "Below";
                var closingPrice = cycleDetail?.ClosingPrice ?? 0.0m;

                if (previousAveragePosition == "Unknown")
                {
                    if (averagePosition == "Below")
                    {
                        previousAveragePosition = averagePosition;
                    }
                }
                else if (averagePosition == "Above")
                {
                    if (previousAveragePosition == "Below")
                    {
                        // Enter Trade
                        boughtPrice = closingPrice;
                        tradeAmount = cashAmount;
                        cashAmount = 0.0m;
                        assetAmount = Math.Round(tradeAmount / closingPrice, 8);
                        tradePosition = "Long";
                    }

                    percentChanged = Math.Round(((closingPrice - boughtPrice) / boughtPrice) * 100, 2);
                    tradeAmount = Math.Round(assetAmount * closingPrice, 2);
                    previousAveragePosition = averagePosition;
                }
                else if (averagePosition == "Below")
                {
                    if (previousAveragePosition == "Above")
                    {
                        // Exit Trade
                        cashAmount = Math.Round(assetAmount * closingPrice, 2);
                        tradeAmount = 0.0m;
                        assetAmount = 0.0m;
                        percentChanged = 0.0m;
                        tradePosition = "Cash";
                    }

                    previousAveragePosition = averagePosition;
                }
                else
                {
                    throw new ArgumentException("Invalid average position");
                }

                if (isShowDetail)
                {
                    _logger.LogInformation("Closed: {cycleTimeEpoch} at {closingPrice}, SMA: {sma}, LMA: {lma}, AVG: {avgPos}, Trade: {tradePos}, Cash: {cashAmt}, TradeAMT: {tradeAmt}, AssetAMT: {assetAmt}, Pecent: {percentChanged}",
                        cycleDetail?.CycleTimeEpoch ?? 0, closingPrice.ToString("0.00"), shortMovingAverage.ToString("0.00"), longMovingAverage.ToString("0.00"), averagePosition, tradePosition, cashAmount.ToString("0.00"),
                        tradeAmount.ToString("0.00"), assetAmount, percentChanged);
                }

                skipCounter++;
            }

            if (previousAveragePosition == "Above")
            {
                cashAmount = tradeAmount;
            }

            return new SummaryModel
            {
                ShortMovingAverageDays = shortMovingAverageDays,
                LongMovingAverageDays = longMovingAverageDays,
                StartingCashAmount = startingCashAmount,
                EndingCashAmount = cashAmount,
                PercentChanged = Math.Round(((cashAmount - startingCashAmount) / startingCashAmount) * 100, 2)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("{ex}", ex.Message);
            return null;
        }
    }

    private static List<int> GetCustomNumbers()
    {
        return new() { 3, 5, 8, 9, 13, 21, 34, 50, 55, 89, 144, 233 };
        //return new() {3, 5, 9, 50 };
    }

    private static IEnumerable<int> GetNumberRange(int startIndex, int endIndex)
    {
        return Enumerable.Range(startIndex, endIndex);
    }

    private List<(int, int)> GenerateMovingAveragePairs(IEnumerable<int> numbers)
    {
        return numbers.SelectMany((x, i) => numbers.Skip(i + 1).Where(y => y > x).Select(y => (x, y)))
                      .ToList();
    }

    private ExchangeTokenModel GetExchangeTokenModel()
    {
        return new ExchangeTokenModel
        {
            ApiKey = "629fc9bd5e351b0001f924e5",
            ApiSecret = "f2024e72-4ebc-4602-942d-55fc81da9a6e",
            ApiPassphrase = "7f&@O$35q!ZoJjyjbX*W8s3tI9tZ1n9s"
        };
    }
}


public class SummaryModel
{
    public int ShortMovingAverageDays { get; set; }
    public int LongMovingAverageDays { get; set; }
    public decimal StartingCashAmount { get; set; }
    public decimal EndingCashAmount { get; set; }
    public decimal PercentChanged { get; set; }
}