namespace ProbabilityTrades.Common.Models;

public class CandleCycleDetail
{
    public long CycleTimeEpoch { get; set; } = 0;
    public decimal OpeningPrice { get; set; } = 0.0m;
    public decimal ClosingPrice { get; set; } = 0.0m;
    public decimal HighestPrice { get; set; } = 0.0m;
    public decimal LowestPrice { get; set; } = 0.0m;
    public decimal Volume { get; set; } = 0.0m;
    public decimal Turnover { get; set; } = 0.0m;
}

public class SymbolDetail
{
    public string Symbol { get; set; } = "";
    public string Name { get; set; } = "";
    public string BaseCurrency { get; set; } = "";
    public string QuoteCurrency { get; set; } = "";
    public string FeeCurrency { get; set; } = "";
    public string Market { get; set; } = "";
    public decimal BaseMinSize { get; set; } = 0.0m;
    public decimal QuoteMinSize { get; set; } = 0.0m;
    public decimal BaseMaxSize { get; set; } = 0.0m;
    public decimal QuoteMaxSize { get; set; } = 0.0m;
    public decimal BaseIncrement { get; set; } = 0.0m;
    public decimal QuoteIncrement { get; set; } = 0.0m;
    public decimal PriceIncrement { get; set; } = 0.0m;
    public decimal PriceLimitRate { get; set; } = 0.0m;
    //public decimal? MinFunds { get; set; } = null;
    public bool IsMarginEnabled { get; set; } = false;
    public bool EnableTrading { get; set; } = false;
}