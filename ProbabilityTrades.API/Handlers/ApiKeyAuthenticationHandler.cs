namespace ProbabilityTrades.API.Handlers;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    internal readonly IConfiguration _config;

    public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder,
                                       ISystemClock clock, IConfiguration config)
        : base(options, logger, encoder, clock)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {

        var apiKeyFromHeader = Request.Headers["x-api-key"];
        if (apiKeyFromHeader.IsNullOrEmpty())
            return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));

        var apiKey = _config.GetValue<string>("ApiKey");
        if (apiKey.Equals(apiKeyFromHeader, StringComparison.CurrentCultureIgnoreCase))
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, "ApiKey"),
                    new Claim("ApiKey", apiKey),
                };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
    }
}