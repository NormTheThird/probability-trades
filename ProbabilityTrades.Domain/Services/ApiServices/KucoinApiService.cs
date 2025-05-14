namespace ProbabilityTrades.Domain.Services.ApiServices;

/// <summary>
///     Documentation: https://docs.kucoin.com/#general
///     
///     Kucoin HTTP Error Status Codes:
///         400	Bad Request -- Invalid request format.
///         401	Unauthorized -- Invalid API Key.
///         403	Forbidden or Too Many Requests -- The request is forbidden or Access limit breached.
///         404	Not Found -- The specified resource could not be found.
///         405	Method Not Allowed -- You tried to access the resource with an invalid method.
///         415	Unsupported Media Type.You need to use: application/json.
///         500	Internal Server Error -- We had a problem with our server.Try again later.
///         Service Unavailable -- We're temporarily offline for maintenance. Please try again later.
///         
///     Kucoin System Error Codes:
///         200001	Order creation for this pair suspended
///         200002	Order cancel for this pair suspended
///         200003	Number of orders breached the limit
///         200009	Please complete the KYC verification before you trade XX
///         200004	Balance insufficient
///         400001	Any of KC-API-KEY, KC-API-SIGN, KC-API-TIMESTAMP, KC-API-PASSPHRASE is missing in your request header
///         400002	KC-API-TIMESTAMP Invalid
///         400003	KC-API-KEY not exists
///         400004	KC-API-PASSPHRASE error
///         400005	Signature error
///         400006	The requested ip address is not in the api whitelist
///         400007	Access Denied
///         404000	Url Not Found
///         400100	Parameter Error
///         400200	Forbidden to place an order
///         400500	Your located country/region is currently not supported for the trading of this token
///         400700	Transaction restricted, there's a risk problem in your account
///         400800	Leverage order failed
///         411100	User are frozen
///         500000	Internal Server Error
///         900001	symbol not exists
/// </summary>
public class KucoinApiService : IExchangeApiService
{
    private readonly string _baseUrl;
    private readonly string _apiVersion;

    public KucoinApiService()
    {
        _baseUrl = "https://api.kucoin.com";
        _apiVersion = "v1";
    }

    public async Task<IEnumerable<AccountBalanceDetailModel>> GetAccountBalanceDetailsAsync(ExchangeTokenModel tokenModel)
    {
        throw new NotImplementedException();
    }

    public async Task TestAuthenticationAsync(ExchangeTokenModel tokenModel)
    {
        throw new NotImplementedException();
    }

    public async Task<List<string>> GetMarketsAsync(ExchangeTokenModel tokenModel)
    {
        using var client = new HttpClient();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Get, tokenModel, $"/api/{_apiVersion}/markets");

        using var httpResponseMessage = await client.SendAsync(httpRequestMessage);
        var retval = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var json = JObject.Parse(retval);
        var jsonData = json["data"];
        var markets = new List<string>();
        if (jsonData != null)
        {
            foreach (var item in jsonData)
            {
                var market = item.Value<string>() ?? string.Empty;
                if (!string.IsNullOrEmpty(market))
                    markets.Add(market);
            }
        }

        return markets;
    }

    public async Task<List<SymbolDetail>> GetSymbolsAsync(ExchangeTokenModel tokenModel)
    {
        using var client = new HttpClient();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Get, tokenModel, $"/api/{_apiVersion}/symbols");

        using var httpResponseMessage = await client.SendAsync(httpRequestMessage);
        var retval = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var json = JObject.Parse(retval);
        var jsonData = json["data"];
        var symbols = new List<SymbolDetail>();
        if (jsonData != null)
        {
            foreach (var item in jsonData)
            {
                try
                {
                    var symbolDetail = new SymbolDetail
                    {
                        Symbol = item["symbol"]?.Value<string>() ?? string.Empty,
                        Name = item["name"]?.Value<string>() ?? string.Empty,
                        BaseCurrency = item["baseCurrency"]?.Value<string>() ?? string.Empty,
                        QuoteCurrency = item["quoteCurrency"]?.Value<string>() ?? string.Empty,
                        FeeCurrency = item["feeCurrency"]?.Value<string>() ?? string.Empty,
                        Market = item["market"]?.Value<string>() ?? string.Empty,
                        BaseMinSize = item["baseMinSize"]?.Value<decimal>() ?? 0.0m,
                        QuoteMinSize = item["quoteMinSize"]?.Value<decimal>() ?? 0.0m,
                        BaseMaxSize = item["baseMaxSize"]?.Value<decimal>() ?? 0.0m,
                        QuoteMaxSize = item["quoteMaxSize"]?.Value<decimal>() ?? 0.0m,
                        BaseIncrement = item["baseIncrement"]?.Value<decimal>() ?? 0.0m,
                        QuoteIncrement = item["quoteIncrement"]?.Value<decimal>() ?? 0.0m,
                        PriceIncrement = item["priceIncrement"]?.Value<decimal>() ?? 0.0m,
                        PriceLimitRate = item["priceLimitRate"]?.Value<decimal>() ?? 0.0m,
                        IsMarginEnabled = item["isMarginEnabled"]?.Value<bool>() ?? false,
                        EnableTrading = item["enableTrading"]?.Value<bool>() ?? false
                    };
                    symbols.Add(symbolDetail);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }

        return symbols.OrderBy(_ => _.BaseCurrency).ToList();
    }


    #region OHLC

    public async Task<List<CandleCycleDetail>> Get1MinOHLCVAsync(ExchangeTokenModel tokenModel, string symbol, int intervalsBack)
    {
        intervalsBack++;
        var minutesPerInverval = 1;
        var startTimeEpoch = DateTime.UtcNow.AddMinutes(-intervalsBack * minutesPerInverval).ToEpochSeconds();
        var endTimeEpoch = DateTime.UtcNow.AddMinutes(-minutesPerInverval).ToEpochSeconds();
        return await GetOHLCVAsync(tokenModel, CandlestickPattern.OneMinute, symbol, startTimeEpoch, endTimeEpoch);
    }

    public async Task<List<CandleCycleDetail>> Get5MinOHLCVAsync(ExchangeTokenModel tokenModel, string symbol, int intervalsBack)
    {
        intervalsBack++;
        var minutesPerInverval = 5;
        var startTimeEpoch = DateTime.UtcNow.AddMinutes(-intervalsBack * minutesPerInverval).ToEpochSeconds();
        var endTimeEpoch = DateTime.UtcNow.AddMinutes(-minutesPerInverval).ToEpochSeconds();
        return await GetOHLCVAsync(tokenModel, CandlestickPattern.FiveMinute, symbol, startTimeEpoch, endTimeEpoch);
    }

    public async Task<List<CandleCycleDetail>> Get15MinOHLCVAsync(ExchangeTokenModel tokenModel, string symbol, int intervalsBack)
    {
        intervalsBack++;
        var minutesPerInverval = 15;
        var startTimeEpoch = DateTime.UtcNow.AddMinutes(-intervalsBack * minutesPerInverval).ToEpochSeconds();
        var endTimeEpoch = DateTime.UtcNow.AddMinutes(-minutesPerInverval).ToEpochSeconds();
        return await GetOHLCVAsync(tokenModel, CandlestickPattern.FifteenMinute, symbol, startTimeEpoch, endTimeEpoch);
    }

    public async Task<List<CandleCycleDetail>> GetHourlyOHLCVAsync(ExchangeTokenModel tokenModel, string symbol, int intervalsBack)
    {
        intervalsBack++;
        var startTimeEpoch = DateTime.UtcNow.AddHours(-intervalsBack).ToEpochSeconds();
        var endTimeEpoch = DateTime.UtcNow.AddHours(-1).ToEpochSeconds();
        return await GetOHLCVAsync(tokenModel, CandlestickPattern.OneHour, symbol, startTimeEpoch, endTimeEpoch);
    }

    public async Task<List<CandleCycleDetail>> GetDailyOHLCVAsync(ExchangeTokenModel tokenModel, string symbol, int intervalsBack)
    {
        intervalsBack++;
        var startTimeEpoch = DateTime.UtcNow.AddDays(-intervalsBack).ToEpochSeconds();
        var endTimeEpoch = DateTime.UtcNow.AddDays(-1).ToEpochSeconds();
        return await GetOHLCVAsync(tokenModel, CandlestickPattern.OneDay, symbol, startTimeEpoch, endTimeEpoch);
    }

    public async Task<List<CandleCycleDetail>> GetOHLCVAsync(ExchangeTokenModel tokenModel, CandlestickPattern candlestick, string symbol, long startTimeEpoch, long endTimeEpoch)
    {
        var endTime = endTimeEpoch;
        var candleCycles = new List<CandleCycleDetail>();
        while (true)
        {
            var endpoint = $"/api/{_apiVersion}/market/candles?type={candlestick.GetDataSourceDescription<CandlestickPattern>(DataSource.Kucoin)}&symbol={symbol}&startAt={startTimeEpoch}&endAt={endTime}";
            using var client = new HttpClient();
            var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Get, tokenModel, endpoint);

            using var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            var retval = httpResponseMessage.Content.ReadAsStringAsync().Result;
            var json = JObject.Parse(retval);
            var jsonCode = json["code"].Value<int>();

            if (jsonCode.Equals(429000))
            {
                Thread.Sleep(500);
                continue;
            }
            else if (jsonCode != 200000)
                throw new Exception($"Kucoin API Error: [{jsonCode}] {json["msg"].Value<string>()}");

            var jsonData = json["data"]
                ?? throw new Exception($"Kucoin API Error: No json data to parse");

            var lastEpochTime = startTimeEpoch;
            foreach (var item in jsonData)
            {
                var candleCycleDetail = new CandleCycleDetail
                {
                    CycleTimeEpoch = item[0]?.Value<long>() ?? 0,
                    OpeningPrice = item[1]?.Value<decimal>() ?? 0.0m,
                    ClosingPrice = item[2]?.Value<decimal>() ?? 0.0m,
                    HighestPrice = item[3]?.Value<decimal>() ?? 0.0m,
                    LowestPrice = item[4]?.Value<decimal>() ?? 0.0m,
                    Volume = item[5]?.Value<decimal>() ?? 0.0m,
                    Turnover = item[6]?.Value<decimal>() ?? 0.0m,
                };
                candleCycles.Add(candleCycleDetail);
                lastEpochTime = candleCycleDetail.CycleTimeEpoch;
            }

            if (startTimeEpoch != lastEpochTime)
                endTime = lastEpochTime;
            else
                break;
        }

        return candleCycles;
    }

    #endregion



    #region OTHER

    public async Task<List<ExchangeAccountListModel>> GetAccountsAsync(ExchangeTokenModel tokenModel, AccountType accountType)
    {
        using var client = new HttpClient();
        var accountTypeQuery = accountType.Equals(AccountType.Unknown) ? "" : $"?type={accountType.ToString().ToLower()}";
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Get, tokenModel, $"/api/{_apiVersion}/accounts{accountTypeQuery}");

        using var httpResponseMessage = await client.SendAsync(httpRequestMessage);
        var retval = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var json = JObject.Parse(retval);
        var jsonData = json["data"];
        var accounts = new List<ExchangeAccountListModel>();
        if (jsonData != null)
        {
            foreach (var item in jsonData)
            {
                var account = new ExchangeAccountListModel
                {
                    Id = item["id"]?.Value<string>() ?? string.Empty,
                    Currency = item["currency"]?.Value<string>() ?? string.Empty,
                    Type = item["type"]?.Value<string>() ?? string.Empty,
                    Balance = item["balance"]?.Value<decimal>() ?? 0.0m,
                    Available = item["available"]?.Value<decimal>() ?? 0.0m,
                    Holds = item["holds"]?.Value<decimal>() ?? 0.0m
                };
                accounts.Add(account);
            }
        }

        return accounts;
    }

    public async Task<ExchangeAccountModel> GetAccountAsync(ExchangeTokenModel tokenModel, string accountId)
    {
        using var client = new HttpClient();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Get, tokenModel, $"/api/{_apiVersion}/accounts/{accountId}");

        using var httpResponseMessage = await client.SendAsync(httpRequestMessage);
        var retval = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var json = JObject.Parse(retval);
        var jsonData = json["data"];
        if (jsonData == null)
            return null;

        return JsonConvert.DeserializeObject<ExchangeAccountModel>(jsonData.ToString());
    }

    public async Task<ExchangeMarketDataModel> GetMarketDataAsync(ExchangeTokenModel tokenModel, string symbol)
    {
        using var client = new HttpClient();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Get, tokenModel, $"/api/{_apiVersion}/market/orderbook/level1?symbol={symbol}");

        using var httpResponseMessage = await client.SendAsync(httpRequestMessage);
        var retval = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var json = JObject.Parse(retval);
        var jsonData = json["data"];
        if (jsonData == null)
            return null;

        return JsonConvert.DeserializeObject<ExchangeMarketDataModel>(jsonData.ToString());
    }

    public async Task<List<ExchangeOrderModel>> GetOrdersAsync(ExchangeTokenModel tokenModel, TradeType tradeType, string status)
    {
        var orders = new List<ExchangeOrderModel>();
        while (true)
        {
            var statusQuery = string.IsNullOrEmpty(status) ? "" : $"&status={status}";
            var endpoint = $"/api/{_apiVersion}/orders?tradeType={tradeType}{statusQuery}";
            using var client = new HttpClient();
            var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Get, tokenModel, endpoint);

            using var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            var retval = httpResponseMessage.Content.ReadAsStringAsync().Result;
            var json = JObject.Parse(retval);
            var jsonData = json["data"];
            if (jsonData != null)
            {
                var currentPage = jsonData["currentPage"];
                foreach (var item in jsonData["items"])
                {
                    var order = JsonConvert.DeserializeObject<ExchangeOrderModel>(item.ToString());
                    orders.Add(order);
                }
                Thread.Sleep(500);
                //if (startTimeEpoch != lastEpochTime)
                //    endTime = lastEpochTime;
                //else
                //    break;
            }

            break;
        }

        return orders;
    }

    public async Task<ExchangeOrderModel> GetOrderAsync(ExchangeTokenModel tokenModel, string orderId)
    {
        using var client = new HttpClient();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Get, tokenModel, $"/api/{_apiVersion}/orders/{orderId}");

        using var httpResponseMessage = await client.SendAsync(httpRequestMessage);
        var retval = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var json = JObject.Parse(retval);
        var jsonData = json["data"];
        if (jsonData == null)
            return null;

        return JsonConvert.DeserializeObject<ExchangeOrderModel>(jsonData.ToString());
    }

    public async Task<CreatedMarginOrderModel> CreateMarginMarketOrderAsync(ExchangeTokenModel tokenModel, MarketAction marketAction, string symbol, decimal funds)
    {
        if (funds <= 0)
            throw new ArgumentOutOfRangeException("funds cannot be less than or equal to 0");

        var data = new Dictionary<string, string>
        {
            { "clientOid", Guid.NewGuid().ToString() },
            { "side", GetTradeSide(marketAction) },
            { "symbol", symbol },
            { "type", "market" },
            { "autoBorrow", "false" },
            { "funds", funds.ToString() }
        };

        return await CreateMarginOrderAsync(tokenModel, data);
    }

    public async Task<CreatedMarginOrderModel> CreateMarginMarketOrderAsync(ExchangeTokenModel tokenModel, bool isBuy, string symbol, decimal funds)
    {
        if (funds <= 0)
            throw new ArgumentOutOfRangeException("funds cannot be less than or equal to 0");

        var data = new Dictionary<string, string>
        {
            { "clientOid", Guid.NewGuid().ToString() },
            { "side", isBuy ? "buy" : "sell" },
            { "symbol", symbol },
            { "type", "market" },
            { "autoBorrow", "false" }
        };

        if (isBuy)
            data.Add("funds", funds.ToString());
        else
            data.Add("size", funds.ToString());

        return await CreateMarginOrderAsync(tokenModel, data);
    }

    public async Task<CreatedMarginOrderModel> CreateMarginLimitOrderAsync(ExchangeTokenModel tokenModel, MarketAction marketAction, string symbol, decimal price, decimal size)
    {
        if (price <= 0)
            throw new ArgumentOutOfRangeException("Price cannot be less than or equal to 0");

        if (size <= 0)
            throw new ArgumentOutOfRangeException("Size cannot be less than or equal to 0");


        var data = new Dictionary<string, string>
        {
            { "clientOid", Guid.NewGuid().ToString() },
            { "side", GetTradeSide(marketAction) },
            { "symbol", symbol },
            { "type", "limit" },
            { "autoBorrow", "false" },
            { "price", price.ToString() },
            { "size", size.ToString() }
        };

        return await CreateMarginOrderAsync(tokenModel, data);
    }

    public async Task<CreatedMarginOrderModel> CreateMarginMarketBuyOrderAsync(ExchangeTokenModel tokenModel, string symbol, decimal size)
    {
        if (size <= 0)
            throw new ArgumentOutOfRangeException("Sice cannot be less than or equal to 0");

        var symbolDetail = await GetSymbolDetailAsync(tokenModel, symbol);
        var decimalPlaces = symbolDetail.PriceIncrement.GetDecimalPlaces();
        var roundedFunds = size.RoundToPrecision(decimalPlaces);
        var data = new Dictionary<string, string>
        {
            { "clientOid", Guid.NewGuid().ToString() },
            { "side", "buy" },
            { "symbol", symbol },
            { "type", "market" },
            { "autoBorrow", "false" },
            { "funds", roundedFunds.ToString() }
        };

        return await CreateMarginOrderAsync(tokenModel, data);
    }

    public async Task<CreatedMarginOrderModel> CreateMarginLimitSellOrderAsync(ExchangeTokenModel tokenModel, string symbol, decimal price, decimal size)
    {
        if (price <= 0)
            throw new ArgumentOutOfRangeException("Price cannot be less than or equal to 0");

        if (size <= 0)
            throw new ArgumentOutOfRangeException("Size cannot be less than or equal to 0");

        var symbolDetail = await GetSymbolDetailAsync(tokenModel, symbol);
        var decimalPlaces = symbolDetail.PriceIncrement.GetDecimalPlaces();
        var roundedFunds = size.RoundToPrecision(decimalPlaces);
        var data = new Dictionary<string, string>
        {
            { "clientOid", Guid.NewGuid().ToString() },
            { "side", "sell" },
            { "symbol", symbol },
            { "type", "limit" },
            { "autoBorrow", "false" },
            { "price", price.ToString() },
            { "size", roundedFunds.ToString() }
        };

        return await CreateMarginOrderAsync(tokenModel, data);
    }

    public async Task<string> CancelOrderAsync(ExchangeTokenModel tokenModel, string orderId)
    {
        throw new NotImplementedException();
        //using var client = new HttpClient();
        //var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Get, tokenModel, $"/api/{_apiVersion}/orders/{orderId}");

        //using var httpResponseMessage = await client.SendAsync(httpRequestMessage);
        //var retval = httpResponseMessage.Content.ReadAsStringAsync().Result;
        //var json = JObject.Parse(retval);
        //var jsonData = json["data"];
        //if (jsonData == null)
        //    return null;

        //return JsonConvert.DeserializeObject<string>(jsonData.ToString());
    }

    public async Task<List<ExchangeStopOrderModel>> GetStopOrdersAsync(ExchangeTokenModel tokenModel, TradeType tradeType)
    {
        var orders = new List<ExchangeStopOrderModel>();
        while (true)
        {
            var endpoint = $"/api/{_apiVersion}/stop-order?tradeType={tradeType}";
            using var client = new HttpClient();
            var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Get, tokenModel, endpoint);

            using var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            var retval = httpResponseMessage.Content.ReadAsStringAsync().Result;
            var json = JObject.Parse(retval);
            var jsonData = json["data"];
            if (jsonData != null)
            {
                foreach (var item in jsonData["items"])
                {
                    var order = JsonConvert.DeserializeObject<ExchangeStopOrderModel>(item.ToString());
                    orders.Add(order);
                }
                //Thread.Sleep(500);
                //if (startTimeEpoch != lastEpochTime)
                //    endTime = lastEpochTime;
                //else
                //    break;
            }

            break;
        }

        return orders;
    }

    public async Task<ExchangeStopOrderModel> GetStopOrderAsync(ExchangeTokenModel tokenModel, string orderId)
    {
        using var client = new HttpClient();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Get, tokenModel, $"/api/{_apiVersion}/stop-order/{orderId}");

        using var httpResponseMessage = await client.SendAsync(httpRequestMessage);
        var retval = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var json = JObject.Parse(retval);
        var jsonData = json["data"];
        if (jsonData == null)
            return null;

        return JsonConvert.DeserializeObject<ExchangeStopOrderModel>(jsonData.ToString());
    }

    public async Task<string?> CreateMarketStopOrderAsync(ExchangeTokenModel tokenModel, MarketAction marketAction, string symbol, decimal stopPrice, decimal funds)
    {
        if (funds <= 0)
            throw new ArgumentOutOfRangeException("funds cannot be less than or equal to 0");

        var data = new Dictionary<string, string>
        {
            { "clientOid", Guid.NewGuid().ToString() },
            { "side", GetTradeSide(marketAction) },
            { "symbol", symbol },
            { "type", "limit" },
            { "stop", "loss" },
            { "stopPrice", stopPrice.ToString() },
            { "tradeType", TradeType.MARGIN_TRADE.ToString() },
            { "funds", funds.ToString() }
        };

        return await CreateStopOrderAsync(tokenModel, data);
    }

    public async Task<string> CreateMarketStopOrderAsync(ExchangeTokenModel tokenModel, bool isBuy, string symbol, decimal stopPrice, decimal funds)
    {
        if (funds <= 0)
            throw new ArgumentOutOfRangeException("funds cannot be less than or equal to 0");

        var data = new Dictionary<string, string>
        {
            { "clientOid", Guid.NewGuid().ToString() },
            { "side", isBuy ? "buy" : "sell" },
            { "symbol", symbol },
            { "type", "limit" },
            { "stop", "loss" },
            { "stopPrice", stopPrice.ToString() },
            { "tradeType", TradeType.MARGIN_TRADE.ToString() },
            { "funds", funds.ToString() }
        };

        return await CreateStopOrderAsync(tokenModel, data);
    }

    public async Task<string> CreateLimitStopOrderAsync(ExchangeTokenModel tokenModel, MarketAction marketAction, string symbol, decimal stopPrice, decimal price, decimal size)
    {
        if (price <= 0)
            throw new ArgumentOutOfRangeException("Price cannot be less than or equal to 0");

        if (size <= 0)
            throw new ArgumentOutOfRangeException("Size cannot be less than or equal to 0");

        var clientOid = Guid.NewGuid().ToString();
        var data = new Dictionary<string, string>
        {
            { "clientOid", clientOid },
            { "side", GetTradeSide(marketAction) },
            { "symbol", symbol },
            { "type", "limit" },
            { "stop", "loss" },
            { "stopPrice", stopPrice.ToString() },
            { "tradeType", TradeType.MARGIN_TRADE.ToString() },
            { "price", price.ToString() },
            { "size", size.ToString() }
        };

        return await CreateStopOrderAsync(tokenModel, data);
    }

    public async Task<string> CreateLimitStopOrderAsync(ExchangeTokenModel tokenModel, bool isBuy, string symbol, decimal stopPrice, decimal size)
    {
        if (size <= 0)
            throw new ArgumentOutOfRangeException("Size cannot be less than or equal to 0");

        var clientOid = Guid.NewGuid().ToString();
        var data = new Dictionary<string, string>
        {
            { "clientOid", clientOid },
            { "side", isBuy ? "buy" : "sell" },
            { "symbol", symbol },
            { "type", "limit" },
            { "stop", "loss" },
            { "stopPrice", stopPrice.ToString() },
            { "tradeType", TradeType.MARGIN_TRADE.ToString() },
            { "price", stopPrice.ToString() },
            { "size", size.ToString() }
        };

        return await CreateStopOrderAsync(tokenModel, data);
    }

    public async Task<string> CancelStopOrderAsync(ExchangeTokenModel tokenModel, string orderId)
    {
        using var client = new HttpClient();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Delete, tokenModel, $"/api/{_apiVersion}/stop-order/{orderId}");
        using var httpResponseMessage = await client.SendAsync(httpRequestMessage);

        var retval = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var json = JObject.Parse(retval);
        return json["data"]["cancelledOrderIds"][0].Value<string>();
    }

    #endregion



    /// <summary>
    ///     Place A New Margin Order
    ///     
    ///     Documentation: https://docs.kucoin.com/#place-a-margin-order
    ///                    This endpoint requires the "Trade" permission.
    ///                    
    ///     Parameters:
    ///         clientOid (string): Unique order id created by users to identify their orders, e.g. UUID
    ///         side (string): buy or sell
    ///         symbol (string): valid trading symbol code. e.g. ETH-BTC
    ///         type (string): [Optional] limit or market (default is limit)
    ///         remark (string): [Optional] remark for the order, length cannot exceed 100 utf8 characters
    ///         stp (string): [Optional] self trade prevention , CN, CO, CB or DC
    ///         marginModel (string): [Optional] The type of trading, including cross (cross mode) and isolated (isolated mode). It is set at cross by default.
    ///         autoBorrow (boolean): [Optional] Auto-borrow to place order. The system will first borrow you funds at the optimal interest rate and then place 
    ///                                          an order for you. Currently autoBorrow parameter only supports cross mode, not isolated mode
    ///     
    ///     Limit Order Parameters:  
    ///         price (string): price per base currency
    ///         size (string): amount of base currency to buy or sell
    ///         timeInForce (string): [Optional] GTC, GTT, IOC, or FOK (default is GTC), read Time In Force
    ///         cancelAfter (long): [Optional] cancel after n seconds, requires timeInForce to be GTT
    ///         postOnly (boolean): [Optional] Post only flag, invalid when timeInForce is IOC or FOK
    ///         hidden (boolean): [Optional] Order will not be displayed in the order book
    ///         iceberg (boolean): [Optional] Only aportion of the order is displayed in the order book
    ///         visibleSize (long): [Optional] The maximum visible size of an iceberg order
    ///         
    ///     Market Order Parameters: (It is required that you use one of the two parameters, size or funds)
    ///         size (string): [Optional] Desired amount in base currency
    ///         funds (string): [Optional] The desired amount of quote currency to use   
    /// </summary>
    /// <param name="tokenModel"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    private async Task<CreatedMarginOrderModel> CreateMarginOrderAsync(ExchangeTokenModel tokenModel, Dictionary<string, string> data)
    {
        using var client = new HttpClient();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Post, tokenModel, $"/api/{_apiVersion}/margin/order", data);
        using var httpResponseMessage = await client.SendAsync(httpRequestMessage);

        var retval = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var json = JObject.Parse(retval);
        if (!json["code"].Value<int>().Equals(200000))
            throw new HttpRequestException(json.ToString(Formatting.None));

        var jsonData = json["data"];
        if (jsonData == null)
            return null;

        return JsonConvert.DeserializeObject<CreatedMarginOrderModel>(jsonData.ToString()); ;
    }

    /// <summary>
    ///     Place A New Stop Order
    ///     
    ///     Documentation: https://docs.kucoin.com/#place-a-new-order-2
    ///                    This endpoint requires the "Trade" permission.
    ///                    
    ///     Parameters:
    ///         clientOid (string): Unique order id created by users to identify their orders, e.g. UUID
    ///         side (string): buy or sell
    ///         symbol (string): valid trading symbol code. e.g. ETH-BTC
    ///         type (string): [Optional] limit or market (default is limit)
    ///         remark (string): [Optional] remark for the order, length cannot exceed 100 utf8 characters
    ///         stop (string): [Optional] Either loss or entry, the default is loss. Requires stopPrice to be defined.
    ///         stopPrice (string): Need to be defined if stop is specified.
    ///         stp (string): [Optional] self trade prevention , CN, CO, CB or DC
    ///         tradeType (string): [Optional] The type of trading : TRADE（Spot Trade）, MARGIN_TRADE (Margin Trade). Default is TRADE 
    ///
    ///     Limit Order Parameters:  
    ///         price (string): price per base currency
    ///         size (string): amount of base currency to buy or sell
    ///         timeInForce (string): [Optional] GTC, GTT, IOC, or FOK (default is GTC), read Time In Force
    ///         cancelAfter (long): [Optional] cancel after n seconds, requires timeInForce to be GTT
    ///         postOnly (boolean): [Optional] Post only flag, invalid when timeInForce is IOC or FOK
    ///         hidden (boolean): [Optional] Order will not be displayed in the order book
    ///         iceberg (boolean): [Optional] Only aportion of the order is displayed in the order book
    ///         visibleSize (long): [Optional] The maximum visible size of an iceberg order
    ///         
    ///     Market Order Parameters: (It is required that you use one of the two parameters, size or funds)
    ///         size (string): [Optional] Desired amount in base currency
    ///         funds (string): [Optional] The desired amount of quote currency to use 
    /// </summary>
    /// <param name="tokenModel"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    private async Task<string> CreateStopOrderAsync(ExchangeTokenModel tokenModel, Dictionary<string, string> data)
    {

        using var client = new HttpClient();
        var httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Post, tokenModel, $"/api/{_apiVersion}/stop-order", data);
        using var httpResponseMessage = await client.SendAsync(httpRequestMessage);

        var retval = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var json = JObject.Parse(retval);
        if (!json["code"].Value<int>().Equals(200000))
            throw new HttpRequestException(json.ToString(Formatting.None));

        return json["data"]?["orderId"].Value<string>();
    }

    private static string GetTradeSide(MarketAction marketAction)
    {
        return marketAction switch
        {
            MarketAction.Unknown => string.Empty,
            MarketAction.EnterLong => "buy",
            MarketAction.ExitLong => "sell",
            MarketAction.ExitLongStopLoss => "sell",
            MarketAction.EnterShort => "sell",
            MarketAction.ExitShort => "buy",
            MarketAction.ExitShortStopLoss => "buy",
            _ => throw new NotImplementedException()
        };
    }

    /// <summary>
    /// All private REST requests must contain the following headers:
    ///     - KC-API-KEY The API key as a string.
    ///     - KC-API-SIGN The base64-encoded signature(see Signing a Message).
    ///     - KC-API-TIMESTAMP A timestamp for your request.
    ///     - KC-API-PASSPHRASE The passphrase you specified when creating the API key.
    ///     - KC-API-KEY-VERSION You can check the version of API key on the page of API Management
    ///     
    /// For the header of KC-API-SIGN:
    ///     - Use API-Secret to encrypt the prehash string { timestamp+ method+ endpoint + body } with sha256 HMAC.
    ///     - The request body is a JSON string and need to be the same with the parameters passed by the API.
    ///     - After that, use base64-encode to encrypt the result in step 1 again.
    /// For the KC-API-PASSPHRASE of the header:
    ///     - For API key-V1.0, please pass requests in plaintext.
    ///     - For API key-V2.0, please Specify KC-API-KEY-VERSION as 2 --> Encrypt passphrase with HMAC-sha256 via API-Secret --> Encode contents by base64 before you pass the request."
    /// Notice:
    ///     - The encrypted timestamp shall be consistent with the KC-API-TIMESTAMP field in the request header.
    ///     - The body to be encrypted shall be consistent with the content of the Request Body.
    ///     - The Method should be UPPER CASE.
    ///     - For GET, DELETE request, the endpoint needs to contain the query string. e.g. /api/v1/deposit-addresses? currency = XBT.The body is "" if there is no request body (typically for GET requests).
    /// </summary>
    /// <param name="httpMethod"></param>
    /// <param name="endpoint"></param>
    /// <returns></returns>
    private HttpRequestMessage CreateHttpRequestMessage(HttpMethod httpMethod, ExchangeTokenModel tokenModel, string endpoint)
    {
        return CreateHttpRequestMessage(httpMethod, tokenModel, endpoint, null);
    }
    private HttpRequestMessage CreateHttpRequestMessage(HttpMethod httpMethod, ExchangeTokenModel tokenModel, string endpoint, Dictionary<string, string> data)
    {
        var timeStamp = DateTime.UtcNow.ToEpochMilliseconds().ToString();
        var stringToSign = timeStamp + httpMethod.ToString() + endpoint;
        if (data is not null)
            stringToSign += JsonConvert.SerializeObject(data);

        var signature = Convert.ToBase64String(stringToSign.GetHash(tokenModel.ApiSecret));
        var passphrase = Convert.ToBase64String(tokenModel.ApiPassphrase.GetHash(tokenModel.ApiSecret));

        var httpRequestMessage = new HttpRequestMessage
        {
            Method = httpMethod,
            RequestUri = new Uri($"{_baseUrl}{endpoint}"),
            Headers = {
                { HttpRequestHeader.Accept.ToString(), "application/json" },
                { "KC-API-SIGN", signature },
                { "KC-API-PASSPHRASE", passphrase},
                { "KC-API-TIMESTAMP", timeStamp },
                { "KC-API-KEY", tokenModel.ApiKey },
                { "KC-API-KEY-VERSION", "2" } }
        };

        if (data is not null)
            httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

        return httpRequestMessage;
    }

    private async Task<SymbolDetail> GetSymbolDetailAsync(ExchangeTokenModel tokenModel, string symbol)
    {

        var symbolDetails = await GetSymbolsAsync(tokenModel);
        var symbolDetail = symbolDetails.FirstOrDefault(_ => _.Symbol.Equals(symbol));

        return symbolDetail is null
            ? throw new KeyNotFoundException($"Unable to find symbol {symbol}")
            : symbolDetail;
    }

    public Task GetAccountBalanceDetails(ExchangeTokenModel tokenModel)
    {
        throw new NotImplementedException();
    }
}