namespace ProbabilityTrades.Common.Models;

public class CurrencyHistoryProcessModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public string DataSource { get; set; } = string.Empty;
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
    public CandlestickPattern CandlestickPattern { get; set; } = CandlestickPattern.Unknown;
    public int IntervalsBack { get; set; } = 0;
    public bool IsActive { get; set; } = false;
    public string LastChangedBy { get; set; } = string.Empty;
    public DateTimeOffset LastChangedAt { get; set; } = DateTime.MinValue;
    public DateTimeOffset DateCreated { get; set; } = DateTime.MinValue;
}