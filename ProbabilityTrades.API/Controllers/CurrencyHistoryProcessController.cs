namespace ProbabilityTrades.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CurrencyHistoryProcessController : BaseController<CurrencyHistoryProcessController>
{
    private readonly ICurrencyHistoryProcessService _currencyHistoryProcessService;

    public CurrencyHistoryProcessController(IConfiguration config, ILogger<CurrencyHistoryProcessController> logger, ICurrencyHistoryProcessService currencyHistoryProcessService)
        : base(config, logger)
    {
        _currencyHistoryProcessService = currencyHistoryProcessService ?? throw new ArgumentNullException(nameof(currencyHistoryProcessService));
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrencyHistoryProcesses(DataSource dataSource)
    {
        try
        {
            var response = new BaseDataResponse();
            var currencyHistoryProcesses = await _currencyHistoryProcessService.GetCurrencyHistoryProcesses(dataSource);

            response.Data = currencyHistoryProcesses;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }
}