namespace ProbabilityTrades.WorkerService.CalculatePump.Processes;

public abstract class BaseProcess<T>
{
    readonly internal ILogger<T> _logger;

    private readonly string _workerServiceName = "Calculated Pump Worker Service";
    private readonly DataSource _dataSource = DataSource.Kucoin;
    private readonly ICalculatePumpService _calculatePumpService;
    private readonly IExchangeApiService _exchangeApiService;
    private readonly IIndicatorAnalysisService _indicatorAnalysisService;
    private readonly IMailService _mailService;

    public BaseProcess(ILogger<T> logger, ICalculatePumpService calculatePumpService, IExchangeApiService exchangeApiService, IIndicatorAnalysisService indicatorAnalysisService, IMailService mailService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _calculatePumpService = calculatePumpService ?? throw new ArgumentNullException(nameof(calculatePumpService));
        _exchangeApiService = exchangeApiService ?? throw new ArgumentNullException(nameof(exchangeApiService));
        _indicatorAnalysisService = indicatorAnalysisService ?? throw new ArgumentNullException(nameof(indicatorAnalysisService));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
    }

    internal async Task CalculatePumpStatusesAsync()
    {
        try
        {
            var tasks = new List<Task>();
            var calculatePumpConfigurations = await _calculatePumpService.GetCalculatePumpConfigurationsAsync(_dataSource, true);
            foreach (var calculatePumpConfiguration in calculatePumpConfigurations) //.Where(_ => _.BaseCurrency.Equals("LINK")))
            {
                //tasks.Add(Task.Run(async () => await CalculatePumpStatusForAsset(calculatePumpConfiguration)));
                await CalculatePumpStatusForAsset(calculatePumpConfiguration);
            }

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError("CalculatePumpStatuses Error: {ex}", ex);
        }

    }

    internal async Task CheckStopLossStatusAsync()
    {
        var openPumpOrders = await _calculatePumpService.GetOpenPumpOrdersAsync(_dataSource, Guid.Parse("6F1F7D73-423A-438C-8BDF-1A1B352470D9"));
        _logger.LogInformation("CheckStopLossStatus: Checking stop loss statuses");
        foreach (var openPumpOrder in openPumpOrders)
        {
            try
            {
                var symbol = $"{openPumpOrder.BaseCurrency}-{openPumpOrder.QuoteCurrency}";
                var tokenModel = GetExchangeTokenModel();
                var stopOrder = await _exchangeApiService.GetStopOrderAsync(tokenModel, openPumpOrder.StopOrderId);
                if (stopOrder is null)
                {
                    // Stop loss was completed, record order
                    var stopLossOrder = await _exchangeApiService.GetOrderAsync(tokenModel, openPumpOrder.StopOrderId);

                    openPumpOrder.ClosedOrderId = stopLossOrder.Id;
                    openPumpOrder.ClosedTimeUTC = DateTimeOffset.FromUnixTimeMilliseconds(stopLossOrder.CreatedAt);
                    openPumpOrder.ClosedAmount = stopLossOrder.DealFunds;
                    openPumpOrder.ClosedMarketPrice = stopLossOrder.Price;
                    openPumpOrder.ExecutedStop = true;

                    await _calculatePumpService.UpdateCalculateOrderAsync(openPumpOrder, _workerServiceName);

                    _logger.LogInformation("CheckStopLossStatus: Stop loss was activated for {symbol}", symbol);
                    await SendSMSMessage($"CheckStopLossStatus: Stop loss was activated for {symbol}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("CheckStopLossStatus: {error}", ex.Message);
                continue;
            }
        }
    }

    internal async Task CloseAllOpenPumpOrdersAsync()
    {
        var openPumpOrders = await _calculatePumpService.GetOpenPumpOrdersAsync(_dataSource, Guid.Parse("6F1F7D73-423A-438C-8BDF-1A1B352470D9"));
        foreach (var openPumpOrder in openPumpOrders)
        {
            try
            {
                await CloseOpenPumpOrderAsync(openPumpOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError("CloseAllOpenPumpOrders: {error}", ex.Message);
                continue;
            }
        }
    }



    private async Task CalculatePumpStatusForAsset(CalculatePumpConfigurationModel calculatePumpConfiguration)
    {
        try
        {
            var tokenModel = GetExchangeTokenModel();
            var symbol = $"{calculatePumpConfiguration.BaseCurrency}-{calculatePumpConfiguration.QuoteCurrency}";
            //var candleCycleDetail = await _exchangeApiService.GetCurrentDailyOHLCVAsync(tokenModel, symbol);
            var candleCycleDetail = (await _exchangeApiService.GetDailyOHLCVAsync(tokenModel, symbol, 1)).FirstOrDefault();

            var calculatePumpModel = new CalculatePumpModel
            {
                BaseCurrency = calculatePumpConfiguration.BaseCurrency,
                QuoteCurrency = calculatePumpConfiguration.QuoteCurrency,
                CandlestickPattern = calculatePumpConfiguration.CandlestickPattern,
                Period = calculatePumpConfiguration.Period,
                ATRMultiplier = calculatePumpConfiguration.ATRMultiplier,
                VolumeMultiplier = calculatePumpConfiguration.VolumeMultiplier,
                CurrentCandlePrice = candleCycleDetail.ClosingPrice,
                CurrentCandleVolume = candleCycleDetail.Volume
            };

            var calculatePumpStatus = await _indicatorAnalysisService.CalculatePumpStatusAsync(calculatePumpModel);
            var isPumpingStatus = calculatePumpStatus.IsPumping ? "started pumping" : "is not pumping";
            _logger.LogInformation("CalculatedPump: {baseCurrency:l} {isPumpingStatus}", calculatePumpConfiguration.BaseCurrency, isPumpingStatus);

            if (calculatePumpStatus.IsPumping)
            {
                await SendPumpMessage(symbol, calculatePumpConfiguration.CandlestickPattern);
                await CreateCalculatePumpStatusAsync(calculatePumpConfiguration, calculatePumpStatus, candleCycleDetail);
                await OpenPump(tokenModel, calculatePumpConfiguration.BaseCurrency, calculatePumpConfiguration.QuoteCurrency);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("CheckIfAssetIsPumping Error: {baseCurrency} {ex}", calculatePumpConfiguration.BaseCurrency, ex);
        }
    }


    private async Task CreateCalculatePumpStatusAsync(CalculatePumpConfigurationModel calculatePumpConfiguration, CalculatePumpStatusBaseModel calculatePumpStatusBase, CandleCycleDetail candleCycleDetail)
    {
        var calculatePumpStatusModel = new CalculatePumpStatusModel
        {
            DataSource = _dataSource,
            BaseCurrency = calculatePumpConfiguration.BaseCurrency,
            QuoteCurrency = calculatePumpConfiguration.QuoteCurrency,
            CandlestickPattern = calculatePumpConfiguration.CandlestickPattern,
            IsPumping = calculatePumpStatusBase.IsPumping,
            Period = calculatePumpConfiguration.Period,
            ATR = calculatePumpStatusBase.ATR,
            AverageVolume = calculatePumpStatusBase.AverageVolume,
            VolumeTarget = calculatePumpStatusBase.VolumeTarget,
            CurrentCandleVolume = candleCycleDetail.Volume,
            PriceTarget = calculatePumpStatusBase.PriceTarget,
            CurrentCandlePrice = candleCycleDetail.ClosingPrice
        };
        await _calculatePumpService.CreateCalculatePumpStatusAsync(calculatePumpStatusModel);
        _logger.LogInformation("CalculatedPump: Added record to CalculatePumpStatus table");
    }

    private async Task OpenPump(ExchangeTokenModel tokenModel, string baseCurrency, string quoteCurrency)
    {
        try
        {
            // Get all current margin accounts.
            var symbol = $"{baseCurrency}-{quoteCurrency}";
            var accounts = await _exchangeApiService.GetAccountsAsync(tokenModel, AccountType.Margin);
            var usdtAccount = accounts.FirstOrDefault(_ => _.Currency.Equals("USDT"))
                ?? throw new Exception("USDT account does not exist");

            if (usdtAccount.Available < 10.00m)
            {
                _logger.LogInformation("OpenPump: Available balance is less than $10");
                return;
            }

            var usdtAvailable = usdtAccount.Available * 0.995m;
            var symbolDetail = await GetSymbolDetailAsync(tokenModel, symbol);

            var newOpenOrderId = await CreateMarginMarketBuyOrderAsync(tokenModel, symbolDetail, symbol, 10.0m);

            var newOrder = await _exchangeApiService.GetOrderAsync(tokenModel, newOpenOrderId)
                ?? throw new Exception("WHAT TO DO IFNEW ORDER INFO IS NULL");

            var marketData = await _exchangeApiService.GetMarketDataAsync(tokenModel, symbol);

            var newStopOrder = await CreateLimitStopOrderAsync(tokenModel, symbolDetail, symbol, marketData.Price, newOrder.DealSize);

            // Store info in calculate pump order table
            var calculatePumpOrder = new CalculatePumpOrderBaseModel
            {
                UserId = Guid.Parse("6F1F7D73-423A-438C-8BDF-1A1B352470D9"),
                DataSource = _dataSource,
                BaseCurrency = baseCurrency,
                QuoteCurrency = quoteCurrency,
                OpenedOrderId = newOpenOrderId,
                OpenedTimeUTC = DateTimeOffset.FromUnixTimeMilliseconds(newOrder.CreatedAt),
                OpenedAmount = newOrder.DealFunds,
                //OpenedMarketPrice = newOrder.Price,
                OpenedMarketPrice = marketData.Price,
                StopOrderId = newStopOrder.StopOrderId,
                StopPrice = newStopOrder.StopPrice,
                OrderQuantity = newOrder.DealSize,
            };
            var calculatePumpOrderId = await _calculatePumpService.CreateCalculateOrderAsync(calculatePumpOrder, _workerServiceName);

            _logger.LogInformation("CalculatedPump: {baseCurrency:l} Added reacord to CalculatePumpOrder table", baseCurrency);
        }
        catch (Exception ex)
        {
            _logger.LogError($"OpenPump Error: {baseCurrency}, " + ex.Message);
        }
    }

    private async Task<SymbolDetail> GetSymbolDetailAsync(ExchangeTokenModel tokenModel, string symbol)
    {
        try
        {
            // TODO: TREY: 2023.04.18 What to do if the symbol detail cannot be found?
            var symbolDetails = await _exchangeApiService.GetSymbolsAsync(tokenModel);
            var symbolDetail = symbolDetails.FirstOrDefault(_ => _.Symbol.Equals(symbol));

            return symbolDetail is null
                ? throw new KeyNotFoundException($"Unable to find symbol {symbol}")
                : symbolDetail;
        }
        catch (Exception ex)
        {
            _logger.LogError("GetSymbolDetail Error: {error}", ex.Message);
            return null;
        }
    }

    private async Task<string> CreateMarginMarketBuyOrderAsync(ExchangeTokenModel tokenModel, SymbolDetail symbolDetail, string symbol, decimal funds)
    {
        try
        {
            // TODO: TREY: 2023.04.18 What to do if market order is not created?
            var decimalPlaces = symbolDetail.PriceIncrement.GetDecimalPlaces();
            var roundedFunds = funds.RoundToPrecision(decimalPlaces);
            var createdOrder = await _exchangeApiService.CreateMarginMarketOrderAsync(tokenModel, true, symbol, roundedFunds)
                 ?? throw new Exception($"Unable to create margin market buy order for {symbol}");

            _logger.LogInformation("CreateMarginMarketBuyOrder: Created margin market buy order {orderId} for {symbol}", createdOrder.OrderId, symbol);
            await SendSMSMessage($"CreateMarginMarketBuyOrder: Created margin market buy order {createdOrder.OrderId} for {symbol}");

            return createdOrder.OrderId;
        }
        catch (Exception ex)
        {
            _logger.LogError("CreateMarginMarketBuyOrder Error: {error}", ex.Message);
            return null;
        }
    }

    private async Task<(string StopOrderId, decimal StopPrice)> CreateLimitStopOrderAsync(ExchangeTokenModel tokenModel, SymbolDetail symbolDetail,
        string symbol, decimal currentPrice, decimal orderQuantity)
    {
        try
        {
            // TODO: TREY: 2023.04.18 What to do if stop price cannot be calculated?
            var stopPrice = currentPrice * .98m; // (1 - .02m); 2% stop loss
            var decimalPlaces = symbolDetail.PriceIncrement.GetDecimalPlaces();
            var roundedStopPrice = stopPrice.RoundToPrecision(decimalPlaces);

            // TODO: TREY: 2023.04.18 What to do if stop loss order is not created?
            var stopLossOrderId = await _exchangeApiService.CreateLimitStopOrderAsync(tokenModel, false, symbol, roundedStopPrice, orderQuantity)
                ?? throw new Exception($"Unable to create limit stop order for {symbol}");

            _logger.LogInformation("CreateLimitStopOrder: Created limit stop order {orderId} for {symbol}", stopLossOrderId, symbol);
            await SendSMSMessage($"CreateLimitStopOrder: Created limit stop order {stopLossOrderId} for {symbol}");

            return (stopLossOrderId, roundedStopPrice);
        }
        catch (Exception ex)
        {
            _logger.LogError("CreateLimitStopOrder Error: {error}", ex.Message);
            return ("UNKNOWN", 0.0m);
        }
    }

    private async Task CloseOpenPumpOrderAsync(CalculatePumpOrderModel calculatePumpOrderModel)
    {
        var tokenModel = GetExchangeTokenModel();
        var symbol = $"{calculatePumpOrderModel.BaseCurrency}-{calculatePumpOrderModel.QuoteCurrency}";

        await CancelStopLimitOrderAsync(tokenModel, calculatePumpOrderModel.StopOrderId);

        // Create new margin market sell order
        var createdOrder = await _exchangeApiService.CreateMarginMarketOrderAsync(tokenModel, false, symbol, calculatePumpOrderModel.OrderQuantity)
             ?? throw new Exception($"Unable to create margin market sell order for {symbol}");

        _logger.LogInformation("CloseOpenPumpOrder: Created margin market sell order {orderId} for {symbol}", createdOrder.OrderId, symbol);
        await SendSMSMessage($"CloseOpenPumpOrder: Created margin market sell order {createdOrder.OrderId} for {symbol}");

        var newOrder = await _exchangeApiService.GetOrderAsync(tokenModel, createdOrder.OrderId)
            ?? throw new Exception("WHAT TO DO IFNEW ORDER INFO IS NULL");

        var marketData = await _exchangeApiService.GetMarketDataAsync(tokenModel, symbol);

        calculatePumpOrderModel.ClosedOrderId = createdOrder.OrderId;
        calculatePumpOrderModel.ClosedTimeUTC = DateTimeOffset.FromUnixTimeMilliseconds(newOrder.CreatedAt);
        calculatePumpOrderModel.ClosedAmount = newOrder.DealFunds;
        calculatePumpOrderModel.ClosedMarketPrice = marketData.Price;
        calculatePumpOrderModel.ExecutedStop = false;

        await _calculatePumpService.UpdateCalculateOrderAsync(calculatePumpOrderModel, _workerServiceName);

        _logger.LogInformation("CloseAllOpenPumpOrders: {baseCurrency:l} pump order has been closed", calculatePumpOrderModel.BaseCurrency);
    }

    private async Task CancelStopLimitOrderAsync(ExchangeTokenModel tokenModel, string stopOrderId)
    {
        try
        {
            var stopOrder = await _exchangeApiService.GetStopOrderAsync(tokenModel, stopOrderId);
            if (stopOrder is not null)
            {
                await _exchangeApiService.CancelStopOrderAsync(tokenModel, stopOrderId);
                _logger.LogInformation("CancelStopLimitOrder: Cancled stop limit order {orderId} for {symbol}", stopOrderId, stopOrder.Symbol);
                await SendSMSMessage($"CancelStopLimitOrder: Cancled stop limit order {stopOrderId} for {stopOrder.Symbol}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("CancelStopLimitOrder Error: {error}", ex.Message);
        }
    }

    private async Task SendPumpMessage(string symbol, CandlestickPattern candlestickPattern)
    {
        var message = $"Currently Pumping!!!\n\n"
                    + $"DataSource: {_dataSource}\n"
                    + $"Symbol: {symbol}\n"
                    + $"Interval: {candlestickPattern}\n";

        await SendSMSMessage(message);
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

    private async Task SendSMSMessage(string message)
    {
        await SendSMSMessage("3612445674", message);
        await SendSMSMessage("6053603198", message);
    }

    private async Task SendSMSMessage(string phoneNumber, string message)
    {
        try
        {
            // TODO: TREY: 2023.11.29 We are not sending SMS messages at this time.
            // await _mailService.SendSMSMessageAsync(phoneNumber, message);
        }
        catch (Exception ex)
        {
            _logger.LogError("SendSMSMessage: {@response}", ex.Message);
        }
    }
}