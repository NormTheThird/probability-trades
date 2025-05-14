namespace ProbabilityTrades.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AnalysisController : BaseController<AnalysisController>
{
    private readonly IIndicatorAnalysisService _indicatorAnalysisService;

    public AnalysisController(IConfiguration config, ILogger<AnalysisController> logger, IIndicatorAnalysisService indicatorAnalysisService)
        : base(config, logger)
    {
        _indicatorAnalysisService = indicatorAnalysisService ?? throw new ArgumentNullException(nameof(indicatorAnalysisService));
    }

    /// <summary>
    ///     Calculate Pump Status
    /// </summary>
    /// <returns></returns>
    [HttpGet("calculate-pump")]
    public async Task<IActionResult> CalculatePumpStatus()
    {
        try
        {
            var response = new BaseDataResponse();

            var calculatePumpModel = new CalculatePumpModel
            {
                BaseCurrency = "BTC",
                QuoteCurrency = "USDT",
                CandlestickPattern = CandlestickPattern.OneDay,
                Period = 14,
                ATRMultiplier = 1.5m,
                VolumeMultiplier = 1.5m,
                CurrentCandlePrice = 0,
                CurrentCandleVolume = 0
            };
            var isPumping = await _indicatorAnalysisService.CalculatePumpStatusAsync(calculatePumpModel);

            response.Data = isPumping;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }
}