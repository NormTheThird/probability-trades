namespace ProbabilityTrades.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CurrencyHistoryController : BaseController<CurrencyHistoryController>
{
    private readonly IExchangeApiService _exchangeApiService;
    private readonly ICurrencyHistoryService _currencyHistoryService;

    public CurrencyHistoryController(IConfiguration config, ILogger<CurrencyHistoryController> logger, IExchangeApiService exchangeApiService,
                                     ICurrencyHistoryService currencyHistoryService)
    : base(config, logger)
    {
        _exchangeApiService = exchangeApiService ?? throw new ArgumentNullException(nameof(exchangeApiService));
        _currencyHistoryService = currencyHistoryService ?? throw new ArgumentNullException(nameof(currencyHistoryService));
    }

    /// <summary>
    ///     GetCurrencyHistorySummary
    /// </summary>
    /// <returns>List of BaseCurrencyHistoryModel</returns>
    [HttpGet]
    public async Task<IActionResult> GetCurrencyHistorySummary()
    {
        try
        {
            var response = new BaseDataResponse();
            var summary = await _currencyHistoryService.GetCurrencyHistorySummary();

            response.Data = summary;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    /// <summary>
    ///     GetCurrencyHistorySummaryDetail
    /// </summary>
    /// <param name="dataSource">The data source of the record</param>
    /// <param name="baseCurrency">The base currency to get</param>
    /// <param name="quoteCurrency">The quote currency to get</param>
    /// <returns>List of CurrencyHistorySummaryDetailModel</returns>
    [HttpGet("{dataSource}/{baseCurrency}-{quoteCurrency}")]
    public async Task<IActionResult> GetCurrencyHistorySummaryDetail(string dataSource, string baseCurrency, string quoteCurrency)
    {
        try
        {
            var response = new BaseDataResponse();
            var summaryDetail = await _currencyHistoryService.GetCurrencyHistorySummaryDetail(dataSource, baseCurrency, quoteCurrency);

            response.Data = summaryDetail;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    /// <summary>
    ///     GetCurrencyHistoryDetails
    /// </summary>
    /// <param name="dataSource">The data source of the record</param>
    /// <param name="baseCurrency">The base currency to get</param>
    /// <param name="quoteCurrency">The quote currency to get</param>
    /// <param name="candlePattern">The candle pattern to get</param>
    /// <param name="offset">The number of records to skip</param>
    /// <param name="fetch">The number of records to fetch</param>
    /// <returns>List of CurrencyHistorySummaryDetailModel</returns>
    [HttpGet("{dataSource}/{baseCurrency}-{quoteCurrency}/{candlePattern}/{offset}/{fetch}")]
    public async Task<IActionResult> GetCurrencyHistoryDetails(string dataSource, string baseCurrency, string quoteCurrency, string candlePattern, int offset, int fetch)
    {
        try
        {
            var response = new BaseDataResponse();
            var currencyHistoryDetails = await _currencyHistoryService.GetCurrencyHistoryDetails(dataSource, baseCurrency, quoteCurrency, candlePattern, offset, fetch);

            response.Data = currencyHistoryDetails;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    /// <summary>
    ///     UpdateCurrencyHistory
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> UpdateCurrencyHistory(UpdateCurrencyHistoryModel updateModel)
    {
        try
        {
            throw new NotImplementedException();

            //var response = new BaseResponse();
            //var loggedInUser = GetLoggedInUser();
            //var exchangeTokenModel = GetExchangeTokenModel(loggedInUser.UserId);
            //var symbol = $"{updateModel.BaseCurrency}-{updateModel.QuoteCurrency}";

            //var candleCycleDetails = await _exchangeApiService.GetOHLCVAsync(exchangeTokenModel, updateModel.CandleStickPattern, symbol, updateModel.StartTimeEpoch, updateModel.EndTimeEpoch);
            //if (candleCycleDetails.Count > 0)
            //{
            //    var baseDataModel = new BaseDataModel
            //    {
            //        DataSource = updateModel.DataSource,
            //        BaseCurrency = updateModel.BaseCurrency,
            //        QuoteCurrency = updateModel.QuoteCurrency,
            //        CandlestickPattern = updateModel.CandleStickPattern
            //    };
            //    await _currencyHistoryService.UpdateCurrencyHistoryAsync(baseDataModel, candleCycleDetails);
            //}

            //response.Success = true;
            //return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }
}