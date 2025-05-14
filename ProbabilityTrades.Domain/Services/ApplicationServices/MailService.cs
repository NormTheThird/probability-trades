namespace ProbabilityTrades.Domain.Services.ApplicationServices;

public class MailService : BaseApplicationService, IMailService
{
    private readonly AmazonSimpleEmailServiceClient _amazonEmailServiceClient;

    public MailService(IConfiguration configuration, ApplicationDbContext db)
        : base(configuration, db)
    {
        _amazonEmailServiceClient = new AmazonSimpleEmailServiceClient(_configuration["Amazon:SES:AccessKey"], _configuration["Amazon:SES:SecretKey"], region: RegionEndpoint.USEast1);
    }

    public Task SendConfirmationEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        // TODO: TREY: 2023.12.14 Fix This
        throw new NotImplementedException();
        //var user = await _db.Users.FirstOrDefaultAsync(_ => _.Email.Equals(email));
        //if (user != null && user.IsActive && !user.IsDeleted && !user.IsEmailVerified)
        //{
        //    var template = "confirm-email";
        //    var templateData = new Dictionary<string, string>
        //    {
        //        { "email_to", email },
        //        { "name", string.IsNullOrEmpty(user.FirstName) ? user.Username : user.FirstName },
        //        { "action_url", $"{_config["BaseUrl"]}security?type=verified&id={user.Id}" }
        //    };
        //    await SendTemplatedEmailAsync(template, templateData, cancellationToken);
        //}
    }

    public Task SendForgotPasswordEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        // TODO: TREY: 2023.12.14 Fix This
        throw new NotImplementedException();
        //var user = await _db.Users.FirstOrDefaultAsync(_ => _.Email.Equals(email));
        //if (user != null && user.IsActive && !user.IsDeleted && user.IsEmailVerified)
        //{
        //    var template = "password-reset";
        //    var templateData = new Dictionary<string, string>
        //    {
        //        { "email_to", email },
        //        { "name", string.IsNullOrEmpty(user.FirstName) ? user.Username : user.FirstName },
        //        { "action_url", $"{_config["BaseUrl"]}security?type=reset&id={user.Id}" }
        //    };
        //    await SendTemplatedEmailAsync(template, templateData, cancellationToken);
        //}
    }

    private async Task<SendTemplatedEmailResponse> SendTemplatedEmailAsync(string template, Dictionary<string, string> templateData, CancellationToken cancellationToken)
    {
        try
        {
            var sendTemplatedEmailRequest = new SendTemplatedEmailRequest
            {
                Source = "support@probabilitytrades.com",
                Destination = new Destination { ToAddresses = new List<string> { templateData["email_to"] } },
                Template = template,
                TemplateData = JsonConvert.SerializeObject(templateData)
            };

            return await _amazonEmailServiceClient.SendTemplatedEmailAsync(sendTemplatedEmailRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}