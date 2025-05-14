namespace ProbabilityTrades.Common.RequestAndResponses;

public class BaseRequest { }

public class BaseHistoryRequest : BaseRequest
{
    public DataSource DataSource { get; set; }
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
    public CandlestickPattern CandlestickPattern { get; set; } = CandlestickPattern.OneHour;
}

public class BaseResponse
{
    public bool Success { get; set; } = false;
    public string ErrorMessage { get; set; } = string.Empty;
}

public class BaseDataResponse : BaseResponse
{
    public object Data { get; set; } = null;
}