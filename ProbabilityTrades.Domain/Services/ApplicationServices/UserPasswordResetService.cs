namespace ProbabilityTrades.Domain.Services.ApplicationServices;

public class UserPasswordResetService : BaseApplicationService, IUserPasswordResetService
{
    public UserPasswordResetService(IConfiguration configuration, ApplicationDbContext db) : base(configuration, db) { }

    public async Task<Guid?> CreatePasswordResetAsync(string email)
    {
        var user = await _db.Users.FirstOrDefaultAsync(_ => _.Email.Equals(email));
        if (user == null || !user.IsActive || user.IsDeleted)
            return null;

        var activePasswordResets = _db.UserPasswordResets.Where(_ => _.UserId.Equals(user.Id) && _.IsActive);
        foreach (var activePasswordReset in activePasswordResets)
            activePasswordReset.IsActive = false;

        var userPasswordReset = new UserPasswordReset
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            IsActive = true,
            DateCreated = DateTime.Now
        };
        _db.UserPasswordResets.Add(userPasswordReset);

        await _db.SaveChangesAsync();
        return userPasswordReset.Id;
    }

    public async Task<bool> ValidatePasswordResetAsync(Guid userId)
    {
        var activePasswordReset = await _db.UserPasswordResets.Include(_ => _.User).FirstOrDefaultAsync(_ => _.UserId.Equals(userId) && _.IsActive);
        if (activePasswordReset?.User == null)
            return false;

        if (activePasswordReset.User.IsDeleted || !activePasswordReset.User.IsActive)
            return false;

        var minutes = (DateTime.Now - activePasswordReset.DateCreated).TotalMinutes;
        if (minutes > 60)
            return false;

        return true;
    }
}