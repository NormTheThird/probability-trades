namespace ProbabilityTrades.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseController<UsersController>
{
    private readonly IUserService _userService;
    private readonly IUserSettingsService _userSettingsService;
    private readonly IUserExchangeService _userExchangeService;
    private readonly IStripeService _stripeService;
    private readonly IStripeApiService _stripeApiService;

    public UsersController(IConfiguration config, ILogger<UsersController> logger, IUserService userService, IUserSettingsService userSettingsService, 
                           IUserExchangeService userExchangeService, IStripeService stripeService, IStripeApiService stripeApiService)
        : base(config, logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        _userExchangeService = userExchangeService ?? throw new ArgumentNullException(nameof(userExchangeService));
        _stripeService = stripeService ?? throw new ArgumentNullException(nameof(stripeService));
        _stripeApiService = stripeApiService ?? throw new ArgumentNullException(nameof(stripeApiService));
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var response = new BaseDataResponse();
            var users = await _userService.GetUsersAsync();
            
            response.Data = users;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        try
        {
            var response = new BaseDataResponse();
            var user = await _userService.GetUserAsync(userId);

            response.Data = user;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpPost("discord")]
    public async Task<IActionResult> AddDiscordUser(DiscordUserModel discordUserModel)
    {
        try
        {
            var response = new BaseDataResponse();

            var (userId, isAdmin) = await _userService.UpsertUserAsync(discordUserModel);

            var stripeCustomer = await _stripeService.GetStripeCustomerIdAsync(userId);
            if(!stripeCustomer.HasCustomerId)
            {
                var newStripeCustomer = await _stripeApiService.CreateCustomerAsync(userId, discordUserModel.Username, discordUserModel.Email);
                await _stripeService.CreateStripeCustomerAsync(userId, newStripeCustomer.Id);
            }

            response.Data = new { UserId = userId, IsAdmin = isAdmin };
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser(UserModel user)
    {
        try
        {
            var response = new BaseResponse();
            await _userService.UpdateUserAsync(user);

            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpPut("{userId}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid userId)
    {
        try
        {
            var response = new BaseResponse();
            await _userService.DeactivateUserAsync(userId);

            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        try
        {
            var response = new BaseResponse();
            
            await _userService.DeleteUserAsync(userId);

            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }


    #region User Exchanges

    [HttpGet("{userId}/user-exchanges")]
    public async Task<IActionResult> GetUserExchanges(Guid userId)
    {
        try
        {
            var response = new BaseDataResponse();
            var userExchanges = await _userExchangeService.GetUserExchangesAsync(userId);

            response.Data = userExchanges;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpPost("{userId}/user-exchanges")]
    public async Task<IActionResult> CreateUserExchange(ExchangeModel userExchangeModel)
    {
        try
        {
            var response = new BaseDataResponse();
            var newId = await _userExchangeService.CreateUserExchangeAsync(userExchangeModel);

            response.Data = newId;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpPut("{userId}/user-exchanges")]
    public async Task<IActionResult> UpdateUserExchange(ExchangeModel exchange)
    {
        try
        {
            var response = new BaseResponse();
            await _userExchangeService.UpdateUserExchangeAsync(exchange);

            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    #endregion

    #region User Settings

    [HttpGet("{userId}/settings")]
    public async Task<IActionResult> GetUserSettings(Guid userId)
    {
        try
        {
            var response = new BaseDataResponse();
            var userSettings = await _userSettingsService.GetUserSettingsAsync(userId);

            response.Data = userSettings;
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpPut("{userId}/settings/toggle-dark-mode")]
    public async Task<IActionResult> ToggleDarkMode(Guid userId)
    {
        try
        {
            var response = new BaseResponse();
            await _userSettingsService.ToggleDarkModeAsync(userId);

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