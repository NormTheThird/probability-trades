namespace ProbabilityTrades.Common.Models;

public class CalculatePumpBaseModel
{
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
    public CandlestickPattern CandlestickPattern { get; set; } = CandlestickPattern.Unknown;
    public int Period { get; set; } = 0;
    public decimal ATRMultiplier { get; set; } = 0.0m;
    public decimal VolumeMultiplier { get; set; } = 0.0m;
}

public class CalculatePumpModel : CalculatePumpBaseModel
{
    public decimal CurrentCandlePrice { get; set; } = 0.0m;
    public decimal CurrentCandleVolume { get; set; } = 0.0m;
}

public class CalculatePumpConfigurationModel : CalculatePumpBaseModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public DataSource DataSource { get; set; } = DataSource.Kucoin;
    public bool IsActive { get; set; } = false;
    public bool IsSendDiscordNotification { get; set; } = false;
    public bool IsCurrentlyPumping { get; set; } = false;
    public string LastChangedBy { get; set; } = string.Empty;
    public DateTimeOffset DateLastChanged { get; set; } = new();
    public DateTimeOffset DateCreated { get; set; } = new();
}

public class CalculatePumpStatusBaseModel
{
    public bool IsPumping { get; set; } = false;
    public int Period { get; set; }
    public decimal ATR { get; set; } = 0.0m;
    public decimal AverageVolume { get; set; } = 0.0m;
    public decimal VolumeTarget { get; set; } = 0.0m;
    public decimal CurrentCandleVolume { get; set; } = 0.0m;
    public decimal PriceTarget { get; set; } = 0.0m;
    public decimal CurrentCandlePrice { get; set; } = 0.0m;
}

public class CalculatePumpStatusModel : CalculatePumpStatusBaseModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public DataSource DataSource { get; set; } = DataSource.Kucoin;
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
    public CandlestickPattern CandlestickPattern { get; set; } = CandlestickPattern.Unknown;
}

public class CalculatePumpOrderBaseModel
{
    public Guid UserId { get; set; } = Guid.Empty;
    public DataSource DataSource { get; set; } = DataSource.Kucoin;
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
    public string OpenedOrderId { get; set; } = string.Empty;
    public DateTimeOffset OpenedTimeUTC { get; set; } = new();
    public decimal OpenedAmount { get; set; } = 0.0m;
    public decimal OpenedMarketPrice { get; set; } = 0.0m;
    public string StopOrderId { get; set; } = string.Empty;
    public decimal StopPrice { get; set; } = 0.0m;
    public decimal OrderQuantity { get; set; } = 0.0m;
}

public class CalculatePumpOrderModel : CalculatePumpOrderBaseModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public string ClosedOrderId { get; set; } = string.Empty;
    public DateTimeOffset? ClosedTimeUTC { get; set; } = null;
    public decimal? ClosedAmount { get; set; } = null;
    public decimal? ClosedMarketPrice { get; set; } = null;
    public bool ExecutedStop { get; set; } = false;
}

public class CalculatePumpDataModel
{
    public long ChartTimeEpoch { get; set; } = 0;
    public decimal HighPrice { get; set; } = 0.0m;
    public decimal LowPrice { get; set; } = 0.0m;
    public decimal ClosePrice { get; set; } = 0.0m;
    public decimal Volume { get; set; } = 0.0m;
}