namespace ProbabilityTrades.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MarketsController : BaseController<MarketsController>
{
    private readonly IMarketService _marketService;

    public MarketsController(IConfiguration config, ILogger<MarketsController> logger, IMarketService marketService)
    : base(config, logger)
    {
        _marketService = marketService ?? throw new ArgumentNullException(nameof(marketService));
    }

    [HttpGet]
    public async Task<IActionResult> GetMarkets()
    {
        try
        {
            var response = new BaseDataResponse();
            var markets = await _marketService.GetMarketsAsync();

            response.Data = markets;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveMarketsList()
    {
        try
        {
            var response = new BaseDataResponse();
            var markets = await _marketService.GetActiveMarketListAsync();

            response.Data = markets;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMarket(MarketModel market)
    {
        try
        {
            var response = new BaseDataResponse();
            market.LastChangedBy = GetLoggedInUser().Username;         
            await _marketService.UpdateMarketAsync(market);

            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }
}