using Stripe;

namespace ProbabilityTrades.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StripeController : BaseController<StripeController>
{
    private readonly IStripeApiService _stripeApiService;
    private readonly IStripeService _stripeService;
    private readonly IUserService _userService;


    public StripeController(IConfiguration config, ILogger<StripeController> logger, IStripeApiService stripeApiService,
                            IStripeService stripeService, IUserService userService)
        : base(config, logger)
    {
        _stripeApiService = stripeApiService ?? throw new ArgumentNullException(nameof(stripeApiService));
        _stripeService = stripeService ?? throw new ArgumentNullException(nameof(stripeService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [HttpGet("products-and-prices")]
    public async Task<IActionResult> GetProductsWithPrices()
    {
        try
        {
            var response = new BaseDataResponse();

            var productsAndPrices = await _stripeApiService.GetProductsWithPrices();

            response.Data = productsAndPrices;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpGet("{priceId}/checkout/{userId}")]
    public async Task<IActionResult> GetCheckoutSessionUrl(string priceId, Guid userId)
    {
        try
        {
            var response = new BaseDataResponse();

            // if user does not have a stripe customer id then get one.
            var (CustomerId, HasCustomerId) = await _stripeService.GetStripeCustomerIdAsync(userId);
            if (!HasCustomerId)
            {
                var user = await _userService.GetUserAsync(userId);

                var newStripeCustomer = await _stripeApiService.CreateCustomerAsync(user.Id, user.Username, user.Email);

                CustomerId = newStripeCustomer.Id;
                await _stripeService.CreateStripeCustomerAsync(user.Id, newStripeCustomer.Id);
            }

            var sessionUrl = await _stripeApiService.CreateCheckoutSessionUrl(priceId, CustomerId);

            response.Data = sessionUrl;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpGet("manage-subscription/{userId}")]
    public async Task<IActionResult> GetCheckoutSessionUrl(Guid userId)
    {
        try
        {
            var response = new BaseDataResponse();

            // if user does not have a stripe customer id then get one.
            var (CustomerId, HasCustomerId) = await _stripeService.GetStripeCustomerIdAsync(userId);
            if (!HasCustomerId)
                throw new KeyNotFoundException("Unable to find customer id.");

            var sessionUrl = await _stripeApiService.CreateCustomerPortalSessionUrlAsync(CustomerId);

            response.Data = sessionUrl;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Index()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _config.GetValue<string>("Stripe:WebhookSecret"));
            if (stripeEvent.Type == Events.CustomerSubscriptionCreated)
            {
                var subscription = (Subscription)stripeEvent.Data.Object;
                await _stripeService.UpdateStripeCustomerSubscriptionAsync(subscription.CustomerId, true);
            }
            else if (stripeEvent.Type == Events.CustomerSubscriptionDeleted)
            {
                var subscription = (Subscription)stripeEvent.Data.Object;
                await _stripeService.UpdateStripeCustomerSubscriptionAsync(subscription.CustomerId, false);
            }
            else if (stripeEvent.Type == Events.CustomerSubscriptionPaused)
            {
                var subscription = (Subscription)stripeEvent.Data.Object;
                await _stripeService.UpdateStripeCustomerSubscriptionAsync(subscription.CustomerId, false);
            }
            else if (stripeEvent.Type == Events.CustomerSubscriptionResumed)
            {
                var subscription = (Subscription)stripeEvent.Data.Object;
                await _stripeService.UpdateStripeCustomerSubscriptionAsync(subscription.CustomerId, true);
            }
            else
            {
                Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
            }

            return Ok();
        }
        catch (StripeException sEx)
        {
            return BadRequest(sEx);
        }
    }
}