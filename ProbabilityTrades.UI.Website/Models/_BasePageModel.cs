namespace ProbabilityTrades.UI.Website.Models;

public abstract class BasePageModel : PageModel
{
    readonly internal IConfiguration _configuration;
    readonly internal HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BasePageModel(IConfiguration configuration, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    internal async Task<string> SendHttpRequest(string url, HttpMethod httpMethod)
    {
        var baseApiUrl = _configuration.GetValue<string>("ApiUrl");
        var httpRequest = new HttpRequestMessage(httpMethod, $"{baseApiUrl}{url}");
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpRequest.Headers.Add("x-api-key", _configuration.GetValue<string>("ApiKeY"));

        var httpResponse = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
        httpResponse.EnsureSuccessStatusCode(); 

        return await httpResponse.Content.ReadAsStringAsync();
    }

    internal async Task CheckSessionAsync()
    {
        var isNewSession = string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Session.GetString("SessionId"));
        if (isNewSession)
        {
            _httpContextAccessor.HttpContext.Session.SetString("SessionId", Guid.NewGuid().ToString());
            await UpdateLastLoginAsync();
        }
    }

    private async Task UpdateLastLoginAsync()
    {
        //await _userService.UpdateLastLoginTimeAsync(User.Identity.Name);
        //var userId = Guid.NewGuid();
        //var requestData = new { UserId = userId };
        //var baseApiUrl = _configuration.GetValue<string>("ApiUrl");
        //var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseApiUrl}users/discord");
        //httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //httpRequest.Headers.Add("x-api-key", _configuration.GetValue<string>("ApiKeY"));
        //httpRequest.Content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

        //var httpResponse = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
    }
}