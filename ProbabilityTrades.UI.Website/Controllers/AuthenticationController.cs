namespace ProbabilityTrades.UI.Website.Controllers;

[Route("oauth/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    [HttpGet("discord")]
    public object DiscordLogin()
    {
        return new { };
    }
}