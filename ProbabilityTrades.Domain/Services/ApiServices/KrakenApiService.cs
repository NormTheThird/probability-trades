using CryptoExchange.Net.Authentication;
using Kraken.Net.Clients;

namespace ProbabilityTrades.Domain.Services.ApiServices;

/// <summary>
///     Documentation: https://docs.kraken.com/rest/
///                    https://docs.kraken.com/api/
///                    
///     Examples: https://support.kraken.com/hc/en-us/articles/4407678695316-Example-code-for-C-REST-and-WebSocket-API
/// </summary>
public class KrakenApiService : IExchangeApiService
{
    private readonly string _baseUrl;
    private readonly string _apiVersion;

    public KrakenApiService()
    {
        _baseUrl = "https://api.kraken.com";
        _apiVersion = "0";
    }

    public async Task<IEnumerable<AccountBalanceDetailModel>> GetAccountBalanceDetailsAsync(ExchangeTokenModel tokenModel)
    {
        var krakenRestClient = new KrakenRestClient(options =>
        {
            options.ApiCredentials = new ApiCredentials(tokenModel.ApiKey, tokenModel.ApiSecret);
            options.RequestTimeout = TimeSpan.FromSeconds(60);
        });

        var assets = await krakenRestClient.SpotApi.ExchangeData.GetAssetsAsync();

        var accountBalanceDetails = new List<AccountBalanceDetailModel>();
        var balances = await krakenRestClient.SpotApi.Account.GetBalancesAsync();
        foreach (var balance in balances.Data)
        {
            if (balance.Key.Equals("ZUSD"))
            {
                var usdBalance = Math.Round(balance.Value, 2, MidpointRounding.AwayFromZero);
                accountBalanceDetails.Add(new AccountBalanceDetailModel
                {
                    Asset = balance.Key,
                    AlternateName = "USD",
                    Decimals = 2,
                    DisplayDecimals = 2,
                    Balance = usdBalance,
                    EstimatedValueUSD = usdBalance,
                    CurrentPriceInUSD = usdBalance,
                });
                continue;
            }

            var exists = accountBalanceDetails.FirstOrDefault(_ => _.Asset.Equals(balance.Key));
            if (exists is null)
            {
                // get symbol info such as alternate name and display decimals
                var assetInfo = assets.Data.FirstOrDefault(_ => _.Key.Equals(balance.Key));
                var alternateName = assetInfo.Value?.AlternateName ?? "";
                var displayDecimals = assetInfo.Value?.DisplayDecimals ?? 0;

                // get adjusted balance based on display decimals and remove any zero balances
                var adjustedBalance = Math.Round(balance.Value, displayDecimals, MidpointRounding.AwayFromZero);
                if (adjustedBalance.Equals(0))
                    continue;

                // get the current price of the asset based on the last trade price
                var currentPrice = await krakenRestClient.SpotApi.ExchangeData.GetTickerAsync($"{alternateName}USD");
                var lastTrade = currentPrice.Data.FirstOrDefault().Value?.LastTrade.Price ?? 0;

                var symbol = await krakenRestClient.SpotApi.ExchangeData.GetSymbolsAsync([$"{alternateName}USD"]);
                var pairDecimals = symbol.Data.FirstOrDefault().Value?.Decimals ?? 0;

                accountBalanceDetails.Add(new AccountBalanceDetailModel
                {
                    Asset = balance.Key,
                    AlternateName = GetAlternateName(alternateName ?? ""),
                    Decimals = assetInfo.Value?.Decimals ?? 0,
                    DisplayDecimals = displayDecimals,
                    Balance = adjustedBalance,
                    EstimatedValueUSD = Math.Round(adjustedBalance * lastTrade, 2, MidpointRounding.AwayFromZero),
                    CurrentPriceInUSD = Math.Round(lastTrade, pairDecimals, MidpointRounding.AwayFromZero),
                });
            }
        }

        var availableBbalances = await krakenRestClient.SpotApi.Account.GetAvailableBalancesAsync();
        foreach (var availableBalance in availableBbalances.Data)
        {
            var accountBalance = accountBalanceDetails.FirstOrDefault(_ => _.Asset.Equals(availableBalance.Key));
            if (accountBalance is not null)
                accountBalance.AvailableBalance = Math.Round(availableBalance.Value.Available, accountBalance.DisplayDecimals, MidpointRounding.AwayFromZero);
        }

        return accountBalanceDetails;
    }

    private string GetAlternateName(string assetAlternateName)
    {
        return assetAlternateName switch
        {
            "XBT" => "BTC",
            _ => assetAlternateName,
        };
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
        throw new NotImplementedException();
    }

    #endregion

    #region OTHER

    public Task<string> CancelOrderAsync(ExchangeTokenModel tokenModel, string orderId)
    {
        throw new NotImplementedException();
    }

    public Task<string> CancelStopOrderAsync(ExchangeTokenModel tokenModel, string orderId)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreateLimitStopOrderAsync(ExchangeTokenModel tokenModel, MarketAction marketAction, string symbol, decimal stopPrice, decimal price, decimal size)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreateLimitStopOrderAsync(ExchangeTokenModel tokenModel, bool isBuy, string symbol, decimal stopPrice, decimal size)
    {
        throw new NotImplementedException();
    }

    public Task<CreatedMarginOrderModel> CreateMarginLimitOrderAsync(ExchangeTokenModel tokenModel, MarketAction marketAction, string symbol, decimal price, decimal amount)
    {
        throw new NotImplementedException();
    }

    public Task<CreatedMarginOrderModel> CreateMarginLimitSellOrderAsync(ExchangeTokenModel tokenModel, string symbol, decimal price, decimal size)
    {
        throw new NotImplementedException();
    }

    public Task<CreatedMarginOrderModel> CreateMarginMarketBuyOrderAsync(ExchangeTokenModel tokenModel, string symbol, decimal size)
    {
        throw new NotImplementedException();
    }

    public Task<CreatedMarginOrderModel> CreateMarginMarketOrderAsync(ExchangeTokenModel tokenModel, MarketAction marketAction, string symbol, decimal funds)
    {
        throw new NotImplementedException();
    }

    public Task<CreatedMarginOrderModel> CreateMarginMarketOrderAsync(ExchangeTokenModel tokenModel, bool isBuy, string symbol, decimal funds)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreateMarketStopOrderAsync(ExchangeTokenModel tokenModel, MarketAction marketAction, string symbol, decimal stopPrice, decimal funds)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreateMarketStopOrderAsync(ExchangeTokenModel tokenModel, bool IsBuy, string symbol, decimal stopPrice, decimal funds)
    {
        throw new NotImplementedException();
    }

    public Task<ExchangeAccountModel> GetAccountAsync(ExchangeTokenModel tokenModel, string accountId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ExchangeAccountListModel>> GetAccountsAsync(ExchangeTokenModel tokenModel, AccountType accountType)
    {
        throw new NotImplementedException();
    }

    public Task<ExchangeMarketDataModel> GetMarketDataAsync(ExchangeTokenModel tokenModel, string symbol)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> GetMarketsAsync(ExchangeTokenModel tokenModel)
    {
        throw new NotImplementedException();
    }

    public Task<ExchangeOrderModel> GetOrderAsync(ExchangeTokenModel tokenModel, string orderId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ExchangeOrderModel>> GetOrdersAsync(ExchangeTokenModel tokenModel, TradeType tradeType, string status)
    {
        throw new NotImplementedException();
    }

    public Task<ExchangeStopOrderModel> GetStopOrderAsync(ExchangeTokenModel tokenModel, string orderId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ExchangeStopOrderModel>> GetStopOrdersAsync(ExchangeTokenModel tokenModel, TradeType tradeType)
    {
        throw new NotImplementedException();
    }

    public Task<List<SymbolDetail>> GetSymbolsAsync(ExchangeTokenModel tokenModel)
    {
        throw new NotImplementedException();
    }

    #endregion



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
}