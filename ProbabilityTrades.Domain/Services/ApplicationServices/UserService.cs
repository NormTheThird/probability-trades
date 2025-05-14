namespace ProbabilityTrades.Domain.Services.ApplicationServices;

public class UserService : BaseApplicationService, IUserService
{
    public UserService(IConfiguration configuration, ApplicationDbContext db) : base(configuration, db) { }

    public async Task<IEnumerable<UserListModel>> GetUsersAsync()
    {
        return await _db.Database.SqlQuery<UserListModel>($"EXEC GetCompleteListOfUsers").ToListAsync();
    }

    public async Task<UserModel> GetUserAsync(Guid userId)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(_ => _.Id.Equals(userId));
        if (user == null)
            throw new KeyNotFoundException($"Unable to get user for id {userId}");

        var userModel = new UserModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Username = user.Username,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            IsDeleted = user.IsDeleted,
            DateCreated = user.DateCreated
        };

        return userModel;
    }

    public async Task<(Guid UserId, bool IsAdmin)> UpsertUserAsync(DiscordUserModel discordUserModel)
    {
        var user = await _db.Users.FirstOrDefaultAsync(_ => _.DiscordUserId.Equals(discordUserModel.DiscordId));
        if (user is null)
        {
            var now = DateTime.Now.InCst();
            user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "",
                LastName = "",
                Email = discordUserModel.Email,
                Username = discordUserModel.Username,
                DiscordUserId = discordUserModel.DiscordId,
                PhoneNumber = "",
                IsActive = true,
                IsDeleted = false,
                PasswordHash = new byte[0],
                Salt = new byte[0],
                DateCreated = now
            };

            await _db.Users.AddAsync(user);
        }

        user.DiscordAccessToken = await discordUserModel.AccessToken.EncryptAsync(_configuration.GetValue<string>("EncryptionKey"));
        user.DiscordRefreshToken = await discordUserModel.RefreshToken.EncryptAsync(_configuration.GetValue<string>("EncryptionKey"));

        await _db.SaveChangesAsync();

        return (user.Id, user.IsAdmin);
    }

    public async Task UpdateUserAsync(UserModel userModel)
    {
        var user = await _db.Users.FirstOrDefaultAsync(_ => _.Id.Equals(userModel.Id));
        if (user == null)
            throw new KeyNotFoundException($"Unable to get user for id {userModel}");

        user.FirstName = userModel.FirstName;
        user.LastName = userModel.LastName;
        user.PhoneNumber = userModel.PhoneNumber;
        await _db.SaveChangesAsync();
    }

    public async Task DeactivateUserAsync(Guid userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(_ => _.Id.Equals(userId));
        if (user == null)
            throw new KeyNotFoundException($"Unable to get user for id {userId}");

        user.IsActive = false;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(_ => _.Id.Equals(userId));
        if (user == null)
            throw new KeyNotFoundException($"Unable to get user for id {userId}");

        user.IsDeleted = true;
        user.IsActive = false;
        await _db.SaveChangesAsync();
    }
}