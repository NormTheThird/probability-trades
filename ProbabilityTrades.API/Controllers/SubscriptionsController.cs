namespace ProbabilityTrades.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : BaseController<SubscriptionsController>
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(IConfiguration config, ILogger<SubscriptionsController> logger, ISubscriptionService subscriptionService)
        : base(config, logger)
    {
        _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
    }

    [AllowAnonymous]
    [HttpGet()]
    public async Task<IActionResult> GetUsersAndSubscriptions()
    {
        try
        {
            var response = new BaseDataResponse();
            
            var usersAndSubscriptions = await _subscriptionService.GetUsersAndSubscriptionsAsync();

            response.Data = usersAndSubscriptions;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }
}