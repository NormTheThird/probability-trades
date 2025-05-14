namespace ProbabilityTrades.Domain.Services.ApplicationServices;

public class SecurityService : BaseApplicationService, ISecurityService
{
    public SecurityService(IConfiguration configuration, ApplicationDbContext db) : base(configuration, db) { }

    public async Task<(Guid NewUserId, string Message)> RegisterUserAsync(RegisterRequest registerRequest)
    {
        var now = DateTime.UtcNow;
        var emailInUse = await _db.Users.AnyAsync(_ => _.Email.Equals(registerRequest.Email));
        if (emailInUse)
            throw new ArgumentException($"Unable to use this email. {GetSupportMessage()}");

        var usernameInUser = await _db.Users.AnyAsync(_ => _.Username.Equals(registerRequest.Username));
        if (usernameInUser)
            throw new ArgumentException($"Unable to use this username. {GetSupportMessage()}");

        var retval = CreatePasswordHashAndSalt(registerRequest.Password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "",
            LastName = "",
            Email = registerRequest.Email,
            Username = registerRequest.Username,
            PhoneNumber = "",
            IsActive = true,
            IsDeleted = false,
            PasswordHash = retval.PasswordHash,
            Salt = retval.Salt,
            DateCreated = now
        };
        await _db.Users.AddAsync(user);

        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Role = "User",
            DateCreated = now
        };
        await _db.UserRoles.AddAsync(userRole);

        var userSettings = new UserSetting
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            IsDarkMode = false
        };
        await _db.UserSettings.AddAsync(userSettings);

        _db.SaveChanges();

        return (user.Id, null);
    }

    public async Task<UserAuthenticationModel> AuthenticateUserAsync(string username, string password)
    {
        var user = await _db.Users.AsNoTracking()
                                  .Include(_ => _.UserRoles)
                                  .FirstOrDefaultAsync(_ => _.Username.Equals(username) || _.Email.Equals(username));

        if (user == null || !user.IsActive || user.IsDeleted)
            throw new AuthenticationException($"Unable to authenticate user.");

        var verified = VerifyPasswordHash(password, user.PasswordHash, user.Salt);
        if (!verified)
            throw new AuthenticationException($"Unable to authenticate user.");

        var userAuthenticationModel = new UserAuthenticationModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
        };

        foreach (var role in user.UserRoles)
            userAuthenticationModel.Roles.Add(role.Role);

        return userAuthenticationModel;
    }

    public async Task<UserAuthenticationModel> RefreshUserAuthenticationAsync(string refreshToken)
    {
        var userRefreshToken = await _db.UserRefreshTokens.AsNoTracking()
                                                          .Include(_ => _.User)
                                                          .Include(_ => _.User.UserRoles)
                                                          .FirstOrDefaultAsync(_ => _.RefreshToken.Equals(refreshToken));
        if (userRefreshToken == null || userRefreshToken.DateExpired < DateTime.Now)
            throw new AuthenticationException($"Unable to authenticate user.");

        var userAuthenticationModel = new UserAuthenticationModel
        {
            Id = userRefreshToken.User.Id,
            Username = userRefreshToken.User.Username,
            Email = userRefreshToken.User.Email,
        };

        foreach (var role in userRefreshToken.User.UserRoles)
            userAuthenticationModel.Roles.Add(role.Role);

        return userAuthenticationModel;
    }

    public async Task ChangeUserPasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = _db.Users.FirstOrDefault(_ => _.Id.Equals(userId)) 
            ?? throw new KeyNotFoundException($"Unable to find user for id {userId}");
        
        var verified = VerifyPasswordHash(currentPassword, user.PasswordHash, user.Salt);
        if (!verified)
            throw new ValidationException("Unable to verify current password.");

        var retval = CreatePasswordHashAndSalt(newPassword);
        user.PasswordHash = retval.PasswordHash;
        user.Salt = retval.Salt;

        await _db.SaveChangesAsync();
    }

    public async Task ResetUserPasswordAsync(Guid userId, string newPassword)
    {
        var user = _db.Users.Include(_ => _.UserPasswordResets).FirstOrDefault(_ => _.Id.Equals(userId)) 
            ?? throw new KeyNotFoundException($"Unable to find user for id {userId}");
        
        var retval = CreatePasswordHashAndSalt(newPassword);
        user.PasswordHash = retval.PasswordHash;
        user.Salt = retval.Salt;

        foreach (var activePasswordReset in user.UserPasswordResets.Where(_ => _.IsActive))
            activePasswordReset.IsActive = false;

        await _db.SaveChangesAsync();
    }

    public async Task SaveUserRefreshToken(Guid userId, string refreshToken)
    {
        var userRefreshToken = await _db.UserRefreshTokens.FirstOrDefaultAsync(_ => _.UserId.Equals(userId));
        if (userRefreshToken == null)
        {
            userRefreshToken = new UserRefreshToken { Id = Guid.NewGuid(), UserId = userId };
            _db.UserRefreshTokens.Add(userRefreshToken);
        }

        var nowInCst = DateTime.Now.InCst();
        userRefreshToken.RefreshToken = refreshToken;
        userRefreshToken.DateIssued = nowInCst;
        userRefreshToken.DateExpired = nowInCst.AddMonths(6);
        await _db.SaveChangesAsync();
    }



    private static (byte[] PasswordHash, byte[] Salt) CreatePasswordHashAndSalt(string password)
    {
        using var hmac = new HMACSHA512();
        return new(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)), hmac.Key);
    }

    private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computeHast = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computeHast.SequenceEqual(passwordHash);
    }
}