namespace ProbabilityTrades.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MovingAverageController : BaseController<MovingAverageController>
{
    private readonly IMovingAverageService _movingAverageService;

    public MovingAverageController(IConfiguration config, ILogger<MovingAverageController> logger, IMovingAverageService movingAverageService)
    : base(config, logger)
    {
        _movingAverageService = movingAverageService ?? throw new ArgumentNullException(nameof(movingAverageService));
    }

    #region Moving Average

    /// <summary>
    ///     GetMovingAverageConfigurations
    /// </summary>
    /// <param name="dataSource">The data source of the record</param>
    /// <returns>List of MovingAverageConfigurationModel</returns>
    [HttpGet("{dataSource}")]
    public async Task<IActionResult> GetMovingAverageConfigurations(DataSource dataSource)
    {
        try
        {
            var response = new BaseDataResponse();
            var summary = await _movingAverageService.GetMovingAverageConfigurationsAsync(dataSource);

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
    ///     GetMovingAverageConfiguration
    /// </summary>
    /// <param name="maConfigurationId">The id of the record</param>
    /// <returns>List of MovingAverageConfigurationModel</returns>
    [HttpGet("kucoin/{maConfigurationId}")]
    public async Task<IActionResult> GetMovingAverageConfiguration(Guid maConfigurationId)
    {
        try
        {
            var response = new BaseDataResponse();
            var configuration = await _movingAverageService.GetMovingAverageConfigurationAsync(maConfigurationId);

            response.Data = configuration;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    /// <summary>
    ///     CreateMovingAverageConfiguration
    /// </summary>
    /// <param name="maConfigurationModel">The moving average configuration model to create</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateMovingAverageConfiguration(MovingAverageConfigurationModel maConfigurationModel)
    {
        try
        {
            var response = new BaseDataResponse();
            var newId = await _movingAverageService.CreateMovingAverageConfigurationAsync(maConfigurationModel, GetLoggedInUser().Username);

            response.Data = newId;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    /// <summary>
    ///     UpdateMovingAverageConfiguration
    /// </summary>
    /// <param name="maConfigurationId">The id of the record</param>
    /// <param name="maConfigurationModel">The moving average configuration model to update</param>
    /// <returns></returns>
    [HttpPut("{maConfigurationId}")]
    public async Task<IActionResult> UpdateMovingAverageConfiguration(Guid maConfigurationId, MovingAverageConfigurationModel maConfigurationModel)
    {
        try
        {
            var response = new BaseResponse();
            if (maConfigurationId != maConfigurationModel.Id)
            {
                response.ErrorMessage = "Configuration id in the URL does not match the id in the body";
                return Conflict(response);
            }

            await _movingAverageService.UpdateMovingAverageConfigurationAsync(maConfigurationModel, GetLoggedInUser().Username);

            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    /// <summary>
    ///     DeleteMovingAverageConfiguration
    /// </summary>
    /// <param name="maConfigurationId"></param>
    /// <returns></returns>
    [HttpDelete("{maConfigurationId}")]
    public async Task<IActionResult> DeleteMovingAverageConfiguration(Guid maConfigurationId)
    {
        try
        {
            var response = new BaseResponse();

            await _movingAverageService.DeleteMovingAverageConfigurationAsync(maConfigurationId);

            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    /// <summary>
    ///     Get moving average statuses
    /// </summary>
    /// <param name="baseCurrency">The base currency to get</param>
    /// <param name="quoteCurrency">The quote currency to get</param>
    /// <param name="numberOfPositions">The number of statuses to get</param>
    /// <returns>List of MovingAverageStatusModel</returns>
    [HttpGet("kucoin/{baseCurrency}-{quoteCurrency}/{numberOfPositions}")]
    public async Task<IActionResult> GetMovingAverageStatuses(string baseCurrency, string quoteCurrency, int numberOfPositions)
    {
        try
        {
            var response = new BaseDataResponse();
            var baseDataModel = new BaseDataModel
            {
                DataSource = DataSource.Kucoin,
                BaseCurrency = baseCurrency,
                QuoteCurrency = quoteCurrency,
                CandlestickPattern = CandlestickPattern.OneDay
            };
            var positions = await _movingAverageService.GetMovingAverageStatusesAsync(baseDataModel, numberOfPositions);

            response.Data = positions;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    /// <summary>
    ///     Get moving average position statuses
    /// </summary>
    /// <param name="baseCurrency">The base currency to get</param>
    /// <param name="quoteCurrency">The quote currency to get</param>
    /// <param name="numberOfPositions">The number of statuses to get</param>
    /// <returns>List of MovingAveragePositionStatusModel</returns>
    [HttpGet("kucoin/{baseCurrency}-{quoteCurrency}/postitions/{numberOfPositions}")]
    public async Task<IActionResult> GetMovingAveragePositionStatusesAsync(string baseCurrency, string quoteCurrency, int numberOfPositions)
    {
        try
        {
            var response = new BaseDataResponse();
            var baseDataModel = new BaseDataModel
            {
                DataSource = DataSource.Kucoin,
                BaseCurrency = baseCurrency,
                QuoteCurrency = quoteCurrency,
                CandlestickPattern = CandlestickPattern.OneDay
            };
            var positions = await _movingAverageService.GetMovingAveragePositionStatusesAsync(baseDataModel, numberOfPositions);

            response.Data = positions;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    #endregion
}