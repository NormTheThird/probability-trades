namespace ProbabilityTrades.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MailController : BaseController<MailController>
{
    private readonly IMailService _mailService;

    public MailController(IConfiguration config, ILogger<MailController> logger, IMailService mailService)
        : base(config, logger)
    {
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
    }

    [HttpPost("send-confirmation-email/{email}")]   
    public async Task<IActionResult> SendConfirmationEmail(string email)
    {
        try
        {
            var response = new BaseResponse();

            await _mailService.SendConfirmationEmailAsync(email);

            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }
}