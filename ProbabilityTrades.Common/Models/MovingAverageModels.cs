namespace ProbabilityTrades.Common.Models;

public class MovingAverageConfigurationModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public DataSource DataSource { get; set; } = DataSource.Kucoin;
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
    public CandlestickPattern CandlestickPattern { get; set; } = CandlestickPattern.Unknown;
    public int ShortMovingAverageDays { get; set; } = 0;
    public int LongMovingAverageDays { get; set; } = 0;
    public decimal StopLossPercentage { get; set; } = 0.0m;
    public bool IsActive { get; set; } = false;
    public bool IsSendSMSNotification { get; set; } = false;
    public bool IsSendDiscordNotification { get; set; } = false;
    public string LastChangedBy { get; set; } = string.Empty;
    public DateTimeOffset DateLastChanged { get; set; } = new();
    public DateTimeOffset DateCreated { get; set; } = new();
}

public class MovingAveragePositionModel
{
    public MarketPosition MarketPosition { get; set; } = MarketPosition.Unknown;
    public decimal ShortMovingAverage { get; set; } = 0.0m;
    public decimal LongMovingAverage { get; set; } = 0.0m;
}

public class MovingAverageStatusModel : MovingAveragePositionModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public DataSource DataSource { get; set; } = DataSource.Kucoin;
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
    public DateOnly CloseDate { get; set; } = new();
    public long ChartTimeEpoch { get; set; } = 0;
    public CandlestickPattern CandlestickPattern { get; set; } = CandlestickPattern.Unknown;
    public decimal ClosePrice { get; set; } = 0.0m;
    public bool IsActionChange { get; set; } = false;
    public int ShortMovingAverageDays { get; set; } = 0;
    public int LongMovingAverageDays { get; set; } = 0;
}

public class MovingAveragePositionStatusModel 
{
    public DataSource DataSource { get; set; } = DataSource.Kucoin;
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
    public DateOnly CloseDate { get; set; } = new();
    public decimal ClosePrice { get; set; } = 0.0m;
    public MarketPosition MarketPosition { get; set; } = MarketPosition.Unknown;
}

public class MovingAverageCurrentPositionModel
{
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
    public MarketPosition MarketPosition { get; set; } = MarketPosition.Unknown;
    public DateOnly CloseDate { get; set; } = new();
}