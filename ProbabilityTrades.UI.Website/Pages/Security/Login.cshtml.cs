namespace ProbabilityTrades.UI.Website.Pages.Security;

public class LoginModel : PageModel
{
    public IActionResult OnPost()
    {
        return Challenge(new AuthenticationProperties { RedirectUri = "/" }, "Discord");
    }
}