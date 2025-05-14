namespace ProbabilityTrades.API.Controllers;

/// <summary>
/// Link: https://en.wikipedia.org/wiki/List_of_HTTP_status_codes
/// 
/// Success:
///     return Ok() = Http status code 200
///     return Created() = Http status code 201
///     return NoContent() = Http status code 204
/// Client Error:
///     return BadRequest() = Http status code 400
///     return Unauthorized() = Http status code 401
///     return NotFound() = Http status code 404
/// Server Error:
///     return InternalServerError() = Http status code 500
///     
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseController<T> : Controller
{
    readonly internal IConfiguration _config;
    readonly internal ILogger<T> _logger;

    public BaseController(IConfiguration config, ILogger<T> logger)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    internal IActionResult ExceptionResult(Exception ex, MethodBase? methodBase)
    {
        var response = new BaseResponse { ErrorMessage = ex.Message };
        // var name = methodBase?.ReflectedType?.FullName ?? "Unknown";
        // _logger.LogError(name + " Exception: {@exception}", ex);

        if (ex is KeyNotFoundException)
            return NotFound(response);
        else if (ex is DuplicateNameException)
            return Conflict(response);
        else
            return BadRequest(response);
    }

    internal (Guid UserId, string Username) GetLoggedInUser()
    {
        if (User is null)
            return (Guid.Empty, "Unknown");

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var username = User.FindFirstValue(ClaimTypes.Name);
        return (userId, username);
    }

    internal ExchangeTokenModel GetExchangeTokenModel(Guid userId)
    {
        // TODO: TREY: 2022.11.10 This needs to come from the database for the logged in user.
        return new ExchangeTokenModel
        {
            ApiKey = "629fc9bd5e351b0001f924e5",
            ApiSecret = "f2024e72-4ebc-4602-942d-55fc81da9a6e",
            ApiPassphrase = "7f&@O$35q!ZoJjyjbX*W8s3tI9tZ1n9s"
        };
    }
}