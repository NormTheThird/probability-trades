namespace ProbabilityTrades.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SecurityController : BaseController<SecurityController>
{
    private readonly IMailService _mailService;
    private readonly ISecurityService _securityService;
    private readonly IUserPasswordResetService _userPasswordResetService;

    public SecurityController(IConfiguration config, ILogger<SecurityController> logger, IMailService mailService,
                              ISecurityService securityService, IUserPasswordResetService userPasswordResetService)
        : base(config, logger)
    {
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
        _userPasswordResetService = userPasswordResetService ?? throw new ArgumentNullException(nameof(userPasswordResetService));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var response = new RegisterResponse();
            var (NewUserId, Message) = await _securityService.RegisterUserAsync(request);
            if (NewUserId.Equals(Guid.Empty))
                throw new ApplicationException(Message);

            var authenticatedUser = new UserAuthenticationModel
            {
                Id = NewUserId,
                Username = request.Username,
                Email = request.Email,
                Roles = new List<string> { "User" }
            };

            var key = _config["Jwt:Key"];
            response.UserId = NewUserId;
            response.AccessToken = TokenService.GenerateJwtToken(authenticatedUser, key);
            response.RefreshToken = TokenService.GenerateRefreshToken(64);

            var hashedRefreshToken = TokenService.HashRefreshToken(response.RefreshToken, key);
            await _securityService.SaveUserRefreshToken(authenticatedUser.Id, hashedRefreshToken);

            await _mailService.SendConfirmationEmailAsync(request.Email);
            
            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }


    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate(AuthenticateRequest request)
    {
        try
        {
            var response = new AuthenticateResponse();
            var authenticatedUser = await _securityService.AuthenticateUserAsync(request.Username, request.Password);
            if (authenticatedUser is null)
                return Unauthorized(new BaseResponse { ErrorMessage = "Unable to authenticate user" });

            var key = _config["Jwt:Key"];
            response.UserId = authenticatedUser.Id;
            response.Username = authenticatedUser.Username;
            response.IsAdmin = authenticatedUser.Roles.Any(_ => _.Equals("Admin"));
            response.AccessToken = TokenService.GenerateJwtToken(authenticatedUser, key);
            response.RefreshToken = TokenService.GenerateRefreshToken(64);

            var hashedRefreshToken = TokenService.HashRefreshToken(response.RefreshToken, key);
            await _securityService.SaveUserRefreshToken(authenticatedUser.Id, hashedRefreshToken);

            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpPost("refresh-authentication")]
    public async Task<IActionResult> RefreshAuthentication(RefreshAuthenticationRequest request)
    {
        try
        {
            var response = new AuthenticateResponse();
            var key = _config["Jwt:Key"];
            var hashedRefreshToken = TokenService.HashRefreshToken(request.RefreshToken, key);

            var authenticatedUser = await _securityService.RefreshUserAuthenticationAsync(hashedRefreshToken);
            if (authenticatedUser is null)
                return Unauthorized(new BaseResponse { ErrorMessage = "Unable to authenticate user" });

            response.UserId = authenticatedUser.Id;
            response.Username = authenticatedUser.Username;
            response.IsAdmin = authenticatedUser.Roles.Any(_ => _.Equals("Admin"));
            response.AccessToken = TokenService.GenerateJwtToken(authenticatedUser, key);
            response.RefreshToken = TokenService.GenerateRefreshToken(64);

            var newHashedRefreshToken = TokenService.HashRefreshToken(response.RefreshToken, key);
            await _securityService.SaveUserRefreshToken(authenticatedUser.Id, newHashedRefreshToken);

            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpPut("forgot-password")]
    public async Task<IActionResult> SendForgotPassword(UserForgotPasswordModel forgotPasswordModel)
    {
        try
        {
            var response = new BaseResponse();
            var newId = await _userPasswordResetService.CreatePasswordResetAsync(forgotPasswordModel.Email);
            if (newId != null)
                await _mailService.SendForgotPasswordEmailAsync(forgotPasswordModel.Email);

            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordModel request)
    {
        try
        {
            var response = new BaseResponse();
            await _securityService.ChangeUserPasswordAsync(request.UserId, request.CurrentPassword, request.NewPassword);

            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpGet("{userId}/validate-password-reset")]
    public async Task<IActionResult> ValidatePasswordReset(Guid userId)
    {
        try
        {
            var response = new BaseResponse();
            var canChangePassword = await _userPasswordResetService.ValidatePasswordResetAsync(userId);

            response.Success = canChangePassword;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }

    [HttpPut("reset-password")]
    public async Task<IActionResult> ResetUserPassword(ResetUserPasswordModel request)
    {
        try
        {
            var response = new BaseResponse();
            var canChangePassword = await _userPasswordResetService.ValidatePasswordResetAsync(request.UserId);
            if (canChangePassword)
                await _securityService.ResetUserPasswordAsync(request.UserId, request.NewPassword);

            response.Success = canChangePassword;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }
}