namespace ProbabilityTrades.Common.Models;

public class BaseCurrencyHistoryModel
{
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
}

public class CreateCurrencyHistoryModel
{
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
    public CandlestickPattern CandlestickPattern { get; set; } = CandlestickPattern.Unknown;
    public long ChartTimeEpoch { get; set; } = 0;
    public DateTimeOffset ChartTimeUTC { get; set; } = DateTime.MinValue;
    public decimal OpeningPrice { get; set; } = 0.0m;
    public decimal ClosingPrice { get; set; } = 0.0m;
    public decimal HighestPrice { get; set; } = 0.0m;
    public decimal LowestPrice { get; set; } = 0.0m;
    public decimal Volume { get; set; } = 0.0m;
    public decimal Turnover { get; set; } = 0.0m;
}


public class UpdateCurrencyHistoryModel : BaseCurrencyHistoryModel
{
    public CandlestickPattern CandleStickPattern { get; set; } = CandlestickPattern.Unknown;
    public long StartTimeEpoch { get; set; } = 0;
    public long EndTimeEpoch { get; set; } = 0;
}

public class CurrencyHistoryModel : BaseCurrencyHistoryModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public CandlestickPattern CandlestickPattern { get; set; } = CandlestickPattern.Unknown;
    public long ChartTimeEpoch { get; set; } = 0;
    public DateTimeOffset ChartTimeUTC { get; set; } = new();
    public DateTimeOffset ChartTimeCST { get; set; } = new();
    public decimal OpeningPrice { get; set; } = 0.0m;
    public decimal ClosingPrice { get; set; } = 0.0m;
    public decimal HighestPrice { get; set; } = 0.0m;
    public decimal LowestPrice { get; set; } = 0.0m;
    public decimal Volume { get; set; } = 0.0m;
    public decimal Turnover { get; set; } = 0.0m;
    public string LastChangedBy { get; set; } = string.Empty;
    public DateTimeOffset DateLastChanged { get; set; } = new();
    public DateTimeOffset DateCreated { get; set; }= new();
}

public class CurrencyHistorySummaryDetailModel : BaseCurrencyHistoryModel
{
    public string CandlePattern { get; set; } = string.Empty;
    public long MinTimeEpoch { get; set; } = 0;
    public DateTime MinDate { get; set; } = new();
    public long MaxTimeEpoch { get; set; } = 0;
    public DateTime MaxDate { get; set; } = new();
    public DateTime LastUpdatedAt { get; set; } = new();
    public int NumberOfRecords { get; set; } = 0;
    public int MaxRecords { get; set; } = 0;
}

public class NullMovingAveragesModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public long ChartTimeEpoch { get; set; } = 0;
    public decimal ClosePrice { get; set; } = 0.0m;
    public decimal? MovingAverage3 { get; set; } = null;
    public decimal? MovingAverage5 { get; set; } = null;
    public decimal? MovingAverage8 { get; set; } = null;
    public decimal? MovingAverage9 { get; set; } = null;
    public decimal? MovingAverage13 { get; set; } = null;
    public decimal? MovingAverage21 { get; set; } = null;
    public decimal? MovingAverage34 { get; set; } = null;
    public decimal? MovingAverage50 { get; set; } = null;
    public decimal? MovingAverage55 { get; set; } = null;
    public decimal? MovingAverage89 { get; set; } = null;
    public decimal? MovingAverage144 { get; set; } = null;
    public decimal? MovingAverage233 { get; set; } = null;
}