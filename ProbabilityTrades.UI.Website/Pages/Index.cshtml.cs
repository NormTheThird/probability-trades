namespace ProbabilityTrades.UI.Website.Pages;

public class IndexModel : BasePageModel
{
    public SubscriptionModel MonthlySubscription { get; set; }
    public SubscriptionModel YearlySubscription { get; set; }

    public IndexModel(IConfiguration configuration, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(configuration, httpClient, httpContextAccessor) { }

    public async Task OnGetAsync()
    {
        await CheckSessionAsync();
        await GetProductsAndPricesFromStripeAsync();
    }

    public async Task<IActionResult> OnGetStripeCheckoutAsync(string priceId)
    {
        try
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                throw new KeyNotFoundException("Unable to find user id.");

            var responseBody = await SendHttpRequest($"stripe/{priceId}/checkout/{userId}", HttpMethod.Get);
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

    private async Task GetProductsAndPricesFromStripeAsync()
    {
        try
        {
            var responseBody = await SendHttpRequest("stripe/products-and-prices/", HttpMethod.Get);
            var dataJson = JObject.Parse(responseBody)["data"].ToString();
            var responseData = JsonConvert.DeserializeObject<List<SubscriptionModel>>(dataJson);

            MonthlySubscription = responseData.FirstOrDefault(x => x.Name.Contains("Monthly"));
            YearlySubscription = responseData.FirstOrDefault(x => x.Name.Contains("Yearly"));
        }
        catch (Exception ex)
        {
            var execptionType = ex.GetType().ToString();
            Console.WriteLine($"{execptionType} Exception: {ex.Message}");
        }
    }
}