namespace ProbabilityTrades.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessagingController : BaseController<MessagingController>
{
    private readonly IMailService _mailService;

    public MessagingController(IConfiguration config, ILogger<MessagingController> logger, IMailService mailService) 
        : base(config, logger)
    {
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
    }

    [HttpPost("send-sms")]
    public async Task<IActionResult> SendSMSMessage(SendSMSModel sendSMSModel)
    {
        try
        {
            var response = new BaseResponse();
            //  TODO: TREY: 2023.11.29 We are not sending SMS messages at this time.
            //await _mailService.SendSMSMessageAsync(sendSMSModel.PhoneNumber, sendSMSModel.Message);

            response.Success = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            return ExceptionResult(ex, MethodBase.GetCurrentMethod());
        }
    }
}