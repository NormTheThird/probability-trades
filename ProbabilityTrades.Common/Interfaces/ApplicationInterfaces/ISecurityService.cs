namespace ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

public interface ISecurityService
{ 
    Task<(Guid NewUserId, string Message)> RegisterUserAsync(RegisterRequest registerRequest);
    Task<UserAuthenticationModel> AuthenticateUserAsync(string username, string password);
    Task<UserAuthenticationModel> RefreshUserAuthenticationAsync(string refreshToken);
    Task ChangeUserPasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task ResetUserPasswordAsync(Guid userId, string newPassword);
    Task SaveUserRefreshToken(Guid userId, string refreshToken);
}