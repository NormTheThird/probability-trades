namespace ProbabilityTrades.UI.Website.Pages.Dashboard.Subscription;

[Authorize]
public class IndexModel : BasePageModel
{
    public IndexModel(IConfiguration configuration, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(configuration, httpClient, httpContextAccessor) { }

    public async Task<IActionResult> OnPostShowStripePortalAsync()
    {
        try
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                throw new KeyNotFoundException("Unable to find user id.");

            var responseBody = await SendHttpRequest($"stripe/manage-subscription/{userId}", HttpMethod.Get);
            var url = JObject.Parse(responseBody)["data"].ToString();
            return Redirect(url);
        }
        catch (Exception ex)
        {
            var execptionType = ex.GetType().ToString();
            Console.WriteLine($"{execptionType} Exception: {ex.Message}");
            return Page();
        }
    }
}