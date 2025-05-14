namespace ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

public interface IUserSettingsService
{
    Task<UserSettignsModel> GetUserSettingsAsync(Guid userId);
    Task ToggleDarkModeAsync(Guid userId);
}