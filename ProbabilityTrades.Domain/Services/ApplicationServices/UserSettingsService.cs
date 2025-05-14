namespace ProbabilityTrades.Domain.Services.ApplicationServices;

public class UserSettingsService : BaseApplicationService, IUserSettingsService
{
    public UserSettingsService(IConfiguration configuration, ApplicationDbContext db) : base(configuration, db) { }

    public async Task<UserSettignsModel> GetUserSettingsAsync(Guid userId)
    {
        var userSettings = await _db.UserSettings.AsNoTracking().FirstOrDefaultAsync(_ => _.UserId.Equals(userId));
        if (userSettings == null)
            throw new KeyNotFoundException($"Unable to get user settings for id {userId}");

        var userSettingsModel = new UserSettignsModel
        {
            Id = userSettings.Id,
            UserId = userSettings.UserId,
            IsDarkMode = userSettings.IsDarkMode
        };

        return userSettingsModel;
    }

    public async Task ToggleDarkModeAsync(Guid userId)
    {
        var userSettings = await _db.UserSettings.FirstOrDefaultAsync(_ => _.UserId.Equals(userId));
        if (userSettings == null)
            throw new KeyNotFoundException($"Unable to get user settings for id {userId}");

        userSettings.IsDarkMode = !userSettings.IsDarkMode;
        await _db.SaveChangesAsync();
    }
}