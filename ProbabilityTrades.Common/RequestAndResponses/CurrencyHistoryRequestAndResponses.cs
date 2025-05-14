namespace ProbabilityTrades.Common.RequestAndResponses;


public class UpdateCurrencyHistoryRequest : BaseRequest
{
    public DataSource DataSource { get; set; }
    public string BaseCurrency { get; set; } = "";
    public string QuoteCurrency { get; set; } = "";
    public CandlestickPattern CandlestickPattern { get; set; }

    public IEnumerable<CandleCycleDetail> CandleCycles { get; set; } = new List<CandleCycleDetail>();
}


public class GetNullMovingAveragesRequest : BaseRequest
{
    public DataSource DataSource { get; set; }
    public string BaseCurrency { get; set; } = "";
    public string QuoteCurrency { get; set; } = "";
    public CandlestickPattern CandlePattern { get; set; }
}



public class UpdateNullMovingAverageRequest : BaseRequest
{
    public Guid HistoryId { get; set; } = Guid.Empty;

    public long CurrentChartTimeEpoch { get; set; } = 0;
    public int Interval { get; set; } = 0;
}