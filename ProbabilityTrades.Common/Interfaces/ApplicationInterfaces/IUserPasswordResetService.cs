namespace ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

public interface IUserPasswordResetService
{
    Task<Guid?> CreatePasswordResetAsync(string email);
    Task<bool> ValidatePasswordResetAsync(Guid userId);
}