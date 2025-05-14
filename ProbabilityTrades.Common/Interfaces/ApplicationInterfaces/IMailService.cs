namespace ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

public interface IMailService
{
    Task SendConfirmationEmailAsync(string email, CancellationToken cancellationToken = default);
    Task SendForgotPasswordEmailAsync(string email, CancellationToken cancellationToken = default);
}