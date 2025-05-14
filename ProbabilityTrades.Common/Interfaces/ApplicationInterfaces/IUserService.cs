namespace ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

public interface IUserService
{
    Task<IEnumerable<UserListModel>> GetUsersAsync();
    Task<UserModel> GetUserAsync(Guid userId);
    Task<(Guid UserId, bool IsAdmin)> UpsertUserAsync(DiscordUserModel discordUserModel);
    Task UpdateUserAsync(UserModel userModel);
    Task DeactivateUserAsync(Guid userId);
    Task DeleteUserAsync(Guid userId);
}