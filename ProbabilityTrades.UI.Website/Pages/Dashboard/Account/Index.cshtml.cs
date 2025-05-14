namespace ProbabilityTrades.UI.Website.Pages.Dashboard.Account;

[Authorize]
public class IndexModel : BasePageModel
{
    [BindProperty]
    public AccountModel Account { get; set; }

    public IndexModel(IConfiguration configuration, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(configuration, httpClient, httpContextAccessor) { }

    public async Task OnGetAsync()
    {
        await GetAccountInformationAsync();
    }

    public async Task<IActionResult> OnPostSaveProfileAsync()
    {
        // Handle the form submission here
        // You can access the submitted data using Account.FirstName, Account.LastName, etc.

        // Save changes to your data source or perform necessary actions

        return Page();
    }

    private async Task GetAccountInformationAsync()
    {
        try
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                throw new KeyNotFoundException("Unable to find user id.");

            var responseBody = await SendHttpRequest($"users/{userId}", HttpMethod.Get);
            var dataJson = JObject.Parse(responseBody)["data"].ToString();
            
            Account = JsonConvert.DeserializeObject<AccountModel>(dataJson);
       }
        catch (Exception ex)
        {
            var execptionType = ex.GetType().ToString();
            Console.WriteLine($"{execptionType} Exception: {ex.Message}");
        }
    }
}
