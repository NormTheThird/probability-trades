namespace ProbabilityTrades.UI.Website.Pages.Dashboard.Users;

[Authorize(Roles = "Admin")]
public class IndexModel : BasePageModel
{
    [BindProperty]
    public List<UserListModel> Users { get; set; }

    public IndexModel(IConfiguration configuration, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(configuration, httpClient, httpContextAccessor) { }

    public async Task OnGetAsync()
    {
        await GetUsersAsync();
    }


    private async Task GetUsersAsync()
    {
        try
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                throw new KeyNotFoundException("Unable to find user id.");

            var responseBody = await SendHttpRequest("users", HttpMethod.Get);
            var dataJson = JObject.Parse(responseBody)["data"].ToString();

            Users = JsonConvert.DeserializeObject<List<UserListModel>>(dataJson);
        }
        catch (Exception ex)
        {
            var execptionType = ex.GetType().ToString();
            Console.WriteLine($"{execptionType} Exception: {ex.Message}");
        }
    }
}