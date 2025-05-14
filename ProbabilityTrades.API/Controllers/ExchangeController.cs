namespace ProbabilityTrades.API.Controllers;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExchangeController : BaseController<ExchangeController>
{
    private readonly IExchangeApiService _exchangeApiService;
    private readonly IUserExchangeService _userExchangeService;

    public ExchangeController(IConfiguration config, ILogger<ExchangeController> logger, IExchangeApiService exchangeApiService,
        IUserExchangeService userExchangeService)
        : base(config, logger)
    {
        _exchangeApiService = exchangeApiService ?? throw new ArgumentNullException(nameof(exchangeApiService));
        _userExchangeService = userExchangeService ?? throw new ArgumentNullException(nameof(userExchangeService));
    }

    #region Kucoin

    [HttpGet("kucoin/{userId}/markets")]
    public async Task<IActionResult> GetMarkets(Guid userId)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");
            var markets = await _exchangeApiService.GetMarketsAsync(exchangeTokens);

            response.Data = markets;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpGet("kucoin/{userId}/symbols/{market}")]
    public async Task<IActionResult> GetSymbols(Guid userId, string market)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");
            var symbols = await _exchangeApiService.GetSymbolsAsync(exchangeTokens);

            response.Data = symbols.Where(_ => _.Market.Equals(market));
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    #region OHLCV

    [HttpGet("kucoin/{userId}/15min-ohlcv/{symbol}")]
    public async Task<IActionResult> Get15MinOHLCV(Guid userId, string symbol)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");
            var candleCycles = await _exchangeApiService.Get15MinOHLCVAsync(exchangeTokens, symbol, 60);

            response.Data = candleCycles;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpGet("kucoin/{userId}/hourly-ohlcv/{symbol}")]
    public async Task<IActionResult> GetHourlyOHLCV(Guid userId, string symbol)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");
            var candleCycles = await _exchangeApiService.GetHourlyOHLCVAsync(exchangeTokens, symbol, 4);

            response.Data = candleCycles;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpGet("kucoin/{userId}/daily-ohlcv/{symbol}")]
    public async Task<IActionResult> GetDailyOHLCV(Guid userId, string symbol)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");
            var candleCycles = await _exchangeApiService.GetDailyOHLCVAsync(exchangeTokens, symbol, 4);

            response.Data = candleCycles;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    #endregion

    #region Accounts

    [HttpGet("kucoin/{userId}/accounts")]
    public async Task<IActionResult> GetAccounts(Guid userId, AccountType accountType)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");
            var accounts = await _exchangeApiService.GetAccountsAsync(exchangeTokens, accountType);

            response.Data = accounts;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpGet("kucoin/{userId}/accounts/{accountId}")]
    public async Task<IActionResult> GetAccount(Guid userId, string accountId)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");
            var account = await _exchangeApiService.GetAccountAsync(exchangeTokens, accountId);

            response.Data = account;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    #endregion

    #region Orders

    [HttpGet("kucoin/{userId}/orders")]
    public async Task<IActionResult> GetOrders(Guid userId, TradeType tradeType, string status)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");
            var orders = await _exchangeApiService.GetOrdersAsync(exchangeTokens, tradeType, status);

            response.Data = orders;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpGet("kucoin/{userId}/orders/{orderId}")]
    public async Task<IActionResult> GetOrder(Guid userId, string orderId)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");
            var order = await _exchangeApiService.GetOrderAsync(exchangeTokens, orderId);

            response.Data = order;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    /// <summary>
    ///     CreateMarginMarketOrder
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="createOrderModel"></param>
    /// <param name="addStopLoss"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpPost("kucoin/{userId}/margin-market-order")]
    public async Task<IActionResult> CreateMarginMarketOrder(Guid userId, ExchangeCreateOrderModel createOrderModel)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");

            var funds = createOrderModel.AmountOfCurrentCurrencyToExchange;
            if (createOrderModel.AmountOfCurrentCurrencyToExchange.Equals(0))
            {
                var marginAccounts = await _exchangeApiService.GetAccountsAsync(exchangeTokens, AccountType.Margin);
                var marginAccount = marginAccounts.FirstOrDefault(_ => _.Currency.Equals(createOrderModel.CurrentCurrency.ToString()));
                if (marginAccount is not null && marginAccount.Balance > 0.1m)
                    funds = Math.Round(marginAccount.Balance, 2, MidpointRounding.ToZero);
            }

            var createdMarginOrder = await _exchangeApiService.CreateMarginMarketOrderAsync(exchangeTokens, createOrderModel.MarketAction, createOrderModel.Symbol, funds);
            if (!string.IsNullOrEmpty(createdMarginOrder.OrderId) && createOrderModel.StopLossPercent > 0.0m)
            {
                // TODO: TREY: 2023.01.13 Add Stop Order
            }

            response.Data = createdMarginOrder;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    /// <summary>
    ///     CreateMarginLimitOrder
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="createOrderModel"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpPost("kucoin/{userId}/margin-limit-order")]
    public async Task<IActionResult> CreateMarginLimitOrder(Guid userId, ExchangeCreateOrderModel createOrderModel)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");

            var marketData = await _exchangeApiService.GetMarketDataAsync(exchangeTokens, createOrderModel.Symbol);

            // ENTER LONG: USDT => BTC then use the best ask price
            // EXIT LONG: BTC => USDT then use the best bid price
            var price = createOrderModel.MarketAction switch
            {
                MarketAction.Unknown => 0.0m,
                MarketAction.EnterLong => marketData.BestAsk,
                MarketAction.ExitLong => marketData.BestBid,
                MarketAction.ExitLongStopLoss => 0.0m,
                MarketAction.EnterShort => 0.0m,
                MarketAction.ExitShort => 0.0m,
                MarketAction.ExitShortStopLoss => 0.0m,
                _ => throw new NotImplementedException()
            };

            var funds = createOrderModel.AmountOfCurrentCurrencyToExchange;
            if (createOrderModel.AmountOfCurrentCurrencyToExchange.Equals(0))
            {
                var marginAccounts = await _exchangeApiService.GetAccountsAsync(exchangeTokens, AccountType.Margin);
                var marginAccount = marginAccounts.FirstOrDefault(_ => _.Currency.Equals(createOrderModel.CurrentCurrency.ToString()));
                if (marginAccount is not null && marginAccount.Balance > 0.1m)
                    funds = Math.Round(marginAccount.Balance, 2, MidpointRounding.ToZero);
            }

            var size = Math.Round(funds / price, 8, MidpointRounding.AwayFromZero);
            var createdMarginOrder = await _exchangeApiService.CreateMarginLimitOrderAsync(exchangeTokens, createOrderModel.MarketAction, createOrderModel.Symbol, price, size);
            if (!string.IsNullOrEmpty(createdMarginOrder.OrderId) && createOrderModel.StopLossPercent > 0.0m)
            {
                // TODO: TREY: 2023.01.13 Add Stop Order
            }

            response.Data = createdMarginOrder;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpDelete("kucoin/{userId}/orders/{orderId}")]
    public async Task<IActionResult> CancelOrder(Guid userId, string orderId)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");
            var canceledId = await _exchangeApiService.CancelOrderAsync(exchangeTokens, orderId);

            response.Data = canceledId;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    #endregion

    #region Stop Orders

    [HttpGet("kucoin/{userId}/stop-orders")]
    public async Task<IActionResult> GetStopOrders(Guid userId, TradeType tradeType)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");
            var orders = await _exchangeApiService.GetStopOrdersAsync(exchangeTokens, tradeType);

            response.Data = orders;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpGet("kucoin/{userId}/stop-orders/{orderId}")]
    public async Task<IActionResult> GetStopOrder(Guid userId, string orderId)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");
            var orders = await _exchangeApiService.GetStopOrderAsync(exchangeTokens, orderId);

            response.Data = orders;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpPost("kucoin/{userId}/market-stop-order")]
    public async Task<IActionResult> CreateMarketStopOrder(Guid userId, ExchangeCreateStopOrderModel createStopOrderModel)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");

            var funds = createStopOrderModel.AmountOfCurrentCurrencyToExchange;
            if (createStopOrderModel.AmountOfCurrentCurrencyToExchange.Equals(0))
            {
                var marginAccounts = await _exchangeApiService.GetAccountsAsync(exchangeTokens, AccountType.Margin);
                var marginAccount = marginAccounts.FirstOrDefault(_ => _.Currency.Equals(createStopOrderModel.CurrentCurrency.ToString()));
                if (marginAccount is not null && marginAccount.Balance > 0.1m)
                    funds = Math.Round(marginAccount.Balance, 2, MidpointRounding.ToZero);
            }

            var stopPrice = 0.0m;
            var newStopOrderId = await _exchangeApiService.CreateMarketStopOrderAsync(exchangeTokens, createStopOrderModel.MarketAction, createStopOrderModel.Symbol, stopPrice, funds);

            response.Data = newStopOrderId;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpPost("kucoin/{userId}/limit-stop-order")]
    public async Task<IActionResult> CreateLimitStopOrder(Guid userId, ExchangeCreateStopOrderModel createStopOrderModel)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");

            var marketData = await _exchangeApiService.GetMarketDataAsync(exchangeTokens, createStopOrderModel.Symbol);

            var funds = createStopOrderModel.AmountOfCurrentCurrencyToExchange;
            if (createStopOrderModel.AmountOfCurrentCurrencyToExchange.Equals(0))
            {
                var marginAccounts = await _exchangeApiService.GetAccountsAsync(exchangeTokens, AccountType.Margin);
                var marginAccount = marginAccounts.FirstOrDefault(_ => _.Currency.Equals(createStopOrderModel.CurrentCurrency.ToString()));
                if (marginAccount is not null)
                    funds = Math.Round(marginAccount.Balance, 8, MidpointRounding.ToZero);
            }

            var stopPrice = 19000.0m;
            var price = 18500.0m;
            var size = Math.Round(funds / price, 8, MidpointRounding.AwayFromZero);
            var newStopOrderId = await _exchangeApiService.CreateLimitStopOrderAsync(exchangeTokens, createStopOrderModel.MarketAction, createStopOrderModel.Symbol, stopPrice, price, funds);

            response.Data = newStopOrderId;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpDelete("kucoin/{userId}/stop-orders/{orderId}")]
    public async Task<IActionResult> CancelStopOrder(Guid userId, string orderId)
    {
        try
        {
            var response = new BaseDataResponse();
            var exchangeTokens = await _userExchangeService.GetUserExchangeTokensAsync(userId, "Kucoin");
            var canceledId = await _exchangeApiService.CancelStopOrderAsync(exchangeTokens, orderId);

            response.Data = canceledId;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    #endregion

    #endregion
}