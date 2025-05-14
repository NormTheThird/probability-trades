namespace ProbabilityTrades.Common.Models;

public class BaseDataModel
{
    public DataSource DataSource { get; set; } = DataSource.Kucoin;
    public CandlestickPattern CandlestickPattern { get; set; } = CandlestickPattern.Unknown;
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
}