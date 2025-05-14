using System.Security.Cryptography.X509Certificates;

namespace ProbabilityTrades.Common.Models;

public class ExchangeTokenModel
{
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string ApiPassphrase { get; set; } = string.Empty;
}

public class ExchangeModel : ExchangeTokenModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid UserId { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset DateCreated { get; set; } = new();
}

public class ExchangeAccountModel
{
    public string Currency { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 0.0m;
    public decimal Available { get; set; } = 0.0m;
    public decimal Holds { get; set; } = 0.0m;
}

public class ExchangeAccountListModel : ExchangeAccountModel
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class ExchangeMarketDataModel
{
    public long time { get; set; } = 0;
    public string Sequence { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0.0m;
    public decimal Size { get; set; } = 0.0m;
    public decimal BestBid { get; set; } = 0.0m;
    public decimal BestBidSize { get; set; } = 0.0m;
    public decimal BestAsk { get; set; } = 0.0m;
    public decimal BestAskSize { get; set; } = 0.0m;
}

public class ExchangeCreateOrderModel
{
    public MarketAction MarketAction { get; set; } = MarketAction.Unknown;
    public string Symbol { get; set; } = string.Empty;
    public CurrencyType CurrentCurrency { get; set; } = CurrencyType.Unknown;
    public decimal AmountOfCurrentCurrencyToExchange { get; set; } = 0.0m;
    public decimal StopLossPercent { get; set; } = 0.0m;
}

public class ExchangeCreateStopOrderModel
{
    public MarketAction MarketAction { get; set; } = MarketAction.Unknown;
    public string Symbol { get; set; } = string.Empty;
    public CurrencyType CurrentCurrency { get; set; } = CurrencyType.Unknown;
    public decimal AmountOfCurrentCurrencyToExchange { get; set; } = 0.0m;
    public decimal StopLossPercent { get; set; } = 0.0m;
}

public class CreatedMarginOrderModel
{
    public string OrderId { get; set; } = string.Empty;
    public decimal? BorrowSize { get; set; } = null;
    public string LoanApplyId { get; set; } = string.Empty;

}

public class ExchangeOrderModel
{
    public string Id { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string TradeType { get; set; } = string.Empty;
    public string Side { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0.0m;
    public decimal Size { get; set; } = 0.0m;
    public decimal DealFunds { get; set; } = 0.0m;
    public decimal DealSize { get; set; } = 0.0m;
    public decimal Fee { get; set; } = 0.0m;
    public string FeeCurrency { get; set; } = string.Empty;
    public string STP { get; set; } = string.Empty;
    public string Stop { get; set; } = string.Empty;
    public string StopTriggered { get; set; } = string.Empty;
    public decimal StopPrice { get; set; } = 0.0m;
    public string TimeInForce { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
    public long CreatedAt { get; set; } = 0;
}

public class ExchangeStopOrderModel
{
    public string Id { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Side { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0.0m;
    public decimal Size { get; set; } = 0.0m;
    public string TimeInForce { get; set; } = string.Empty;
    public long OrderTime { get; set; } = 0;
    public string TradeType { get; set; } = string.Empty;
    public string FeeCurrency { get; set; } = string.Empty;
    public decimal TakerFeeRate { get; set; } = 0.0m;
    public decimal MakerFeeRate { get; set; } = 0.0m;
    public long CreatedAt { get; set; } = 0;
    public string Stop { get; set; } = string.Empty;
    public decimal StopPrice { get; set; } = 0.0m;
}