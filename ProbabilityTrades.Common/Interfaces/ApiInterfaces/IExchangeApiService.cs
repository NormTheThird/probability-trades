namespace ProbabilityTrades.Common.Interfaces.ApiInterfaces;

public interface IExchangeApiService
{
    Task<IEnumerable<AccountBalanceDetailModel>> GetAccountBalanceDetailsAsync(ExchangeTokenModel tokenModel);


    Task<List<string>> GetMarketsAsync(ExchangeTokenModel tokenModel);
    Task<List<SymbolDetail>> GetSymbolsAsync(ExchangeTokenModel tokenModel);


    #region OHLC

    Task<List<CandleCycleDetail>> Get1MinOHLCVAsync(ExchangeTokenModel tokenModel, string symbol, int intervalsBack);
    Task<List<CandleCycleDetail>> Get5MinOHLCVAsync(ExchangeTokenModel tokenModel, string symbol, int intervalsBack);
    Task<List<CandleCycleDetail>> Get15MinOHLCVAsync(ExchangeTokenModel tokenModel, string symbol, int intervalsBack);
    Task<List<CandleCycleDetail>> GetHourlyOHLCVAsync(ExchangeTokenModel tokenModel, string symbol, int intervalsBack);
    Task<List<CandleCycleDetail>> GetDailyOHLCVAsync(ExchangeTokenModel tokenModel, string symbol, int intervalsBack);
    Task<List<CandleCycleDetail>> GetOHLCVAsync(ExchangeTokenModel tokenModel, CandlestickPattern candlestick, string symbol, long startTimeEpoch, long endTimeEpoch);

    #endregion


    Task<List<ExchangeAccountListModel>> GetAccountsAsync(ExchangeTokenModel tokenModel, AccountType accountType);
    Task<ExchangeAccountModel> GetAccountAsync(ExchangeTokenModel tokenModel, string accountId);
    Task<ExchangeMarketDataModel> GetMarketDataAsync(ExchangeTokenModel tokenModel, string symbol);
    Task<List<ExchangeOrderModel>> GetOrdersAsync(ExchangeTokenModel tokenModel, TradeType tradeType, string status);
    Task<ExchangeOrderModel> GetOrderAsync(ExchangeTokenModel tokenModel, string orderId);
    Task<CreatedMarginOrderModel> CreateMarginMarketOrderAsync(ExchangeTokenModel tokenModel, MarketAction marketAction, string symbol, decimal funds);
    Task<CreatedMarginOrderModel> CreateMarginMarketOrderAsync(ExchangeTokenModel tokenModel, bool isBuy, string symbol, decimal funds);
    Task<CreatedMarginOrderModel> CreateMarginLimitOrderAsync(ExchangeTokenModel tokenModel, MarketAction marketAction, string symbol, decimal price, decimal amount);


    Task<CreatedMarginOrderModel> CreateMarginMarketBuyOrderAsync(ExchangeTokenModel tokenModel, string symbol, decimal size);
    Task<CreatedMarginOrderModel> CreateMarginLimitSellOrderAsync(ExchangeTokenModel tokenModel, string symbol, decimal price, decimal size);


    Task<string> CancelOrderAsync(ExchangeTokenModel tokenModel, string orderId);
    Task<List<ExchangeStopOrderModel>> GetStopOrdersAsync(ExchangeTokenModel tokenModel, TradeType tradeType);
    Task<ExchangeStopOrderModel> GetStopOrderAsync(ExchangeTokenModel tokenModel, string orderId);
    Task<string> CreateMarketStopOrderAsync(ExchangeTokenModel tokenModel, MarketAction marketAction, string symbol, decimal stopPrice, decimal funds);
    Task<string> CreateMarketStopOrderAsync(ExchangeTokenModel tokenModel, bool IsBuy, string symbol, decimal stopPrice, decimal funds);
    Task<string> CreateLimitStopOrderAsync(ExchangeTokenModel tokenModel, MarketAction marketAction, string symbol, decimal stopPrice, decimal price, decimal size);
    Task<string> CreateLimitStopOrderAsync(ExchangeTokenModel tokenModel, bool isBuy, string symbol, decimal stopPrice, decimal size);
    Task<string> CancelStopOrderAsync(ExchangeTokenModel tokenModel, string orderId);
}