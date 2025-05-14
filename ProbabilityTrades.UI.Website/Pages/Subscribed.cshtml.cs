namespace ProbabilityTrades.UI.Website.Pages
{
    public class SubscribedModel : PageModel
    {
        public IActionResult OnGet(string sessionId)
        {
            if(string.IsNullOrEmpty(sessionId))
               return RedirectToPage("/Index");
            
            return Page();
        }
    }
}
