namespace ProbabilityTrades.UI.Website.Services;

public static class ConfigurationService
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddRazorPages();
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSession(options =>
        {
            options.Cookie.Name = "MySessionCookie";
            options.IdleTimeout = TimeSpan.FromDays(180);
        });
        builder.Services.AddAuthentication(options =>
                        {
                            options.DefaultChallengeScheme = "MyCookieAuth";
                            options.DefaultAuthenticateScheme = "MyCookieAuth";
                            options.DefaultSignInScheme = "MyCookieAuth";
                        })
                        .AddCookie("MyCookieAuth", options =>
                        {
                            options.Cookie.Name = "MyCookieAuth";
                            options.LoginPath = "/Security/Login";
                            options.AccessDeniedPath = "/Security/AccessDenied";
                            options.Cookie.SameSite = SameSiteMode.Lax;

                            options.Events = new CookieAuthenticationEvents
                            {
                                OnValidatePrincipal = context =>
                                {
                                    if (context.Principal.Identity.IsAuthenticated && context.Properties?.IsPersistent == true)
                                        context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30));
                                    return Task.CompletedTask;
                                },
                                OnSigningIn = context =>
                                {
                                    context.Properties.IsPersistent = true;
                                    return Task.CompletedTask;
                                }
                            };
                        })
                        .AddOAuth("Discord", options =>
                        {
                            options.AuthorizationEndpoint = "https://discord.com/api/oauth2/authorize";
                            options.Scope.Add("identify");
                            options.Scope.Add("email");
                            options.Scope.Add("guilds.join");

                            options.CallbackPath = new PathString("/oauth/authentication/discord");
                            options.ClientId = builder.Configuration.GetValue<string>("Discord:ClientId");
                            options.ClientSecret = builder.Configuration.GetValue<string>("Discord:ClientSecret");
                            options.TokenEndpoint = "https://discord.com/api/oauth2/token";
                            options.UserInformationEndpoint = "https://discord.com/api/users/@me";

                            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
                            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

                            options.AccessDeniedPath = "/discord-auth-failed";

                            options.Events = new OAuthEvents
                            {
                                OnCreatingTicket = async context =>
                                {
                                    var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                                    var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                                    response.EnsureSuccessStatusCode();

                                    var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
                                    var discordId = user.GetProperty("id").GetString();
                                    var userName = user.GetProperty("username").GetString();
                                    var userEmail = user.GetProperty("email").GetString();

                                    var (userId, isAdmin) = await AddDiscordUserAsync(builder.Configuration, discordId, userName, userEmail, context.AccessToken, context.RefreshToken);

                                    var claims = new List<Claim>
                                    {
                                            new(ClaimTypes.NameIdentifier, userId.ToString()),
                                            new(ClaimTypes.Name, userName),
                                            new(ClaimTypes.Email, userEmail),
                                            new(ClaimTypes.Role, isAdmin ? "Admin" : "User"),
                                            new("DiscordId", discordId),
                                    };

                                    var claimsIdentity = new ClaimsIdentity(claims, context.Scheme.Name);
                                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                                    var authenticationProperties = new AuthenticationProperties
                                    {
                                        AllowRefresh = true,
                                        IsPersistent = true,
                                        IssuedUtc = DateTimeOffset.UtcNow,
                                        ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1),
                                        RedirectUri = "/"
                                    };

                                    context.Principal = claimsPrincipal;
                                    await context.HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, authenticationProperties);
                                }
                            };
                        });
    }


    private static async Task<(Guid UserId, bool IsAdmin)> AddDiscordUserAsync(ConfigurationManager config, string discordId, string username, string email, string accessToken, string refreshToken)
    {
        var requestData = new { DiscordId = discordId, Username = username, Email = email, AccessToken = accessToken, RefreshToken = refreshToken };
        var baseApiUrl = config.GetValue<string>("ApiUrl");
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseApiUrl}users/discord");
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpRequest.Headers.Add("x-api-key", config.GetValue<string>("ApiKeY"));
        httpRequest.Content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

        using var httpClient = new HttpClient();
        var httpResponse = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
        if (!httpResponse.IsSuccessStatusCode)
            return (Guid.Empty, false);

        var responseBody = await httpResponse.Content.ReadAsStringAsync();
        var dataJson = JObject.Parse(responseBody)["data"]?.ToString() ?? "";
        var responseObject = JsonConvert.DeserializeAnonymousType(dataJson, new { UserId = "", IsAdmin = false });
        return (Guid.Parse(responseObject?.UserId ?? ""), responseObject?.IsAdmin ?? false);
    }
}