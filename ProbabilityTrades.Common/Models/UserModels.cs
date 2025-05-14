namespace ProbabilityTrades.Common.Models;

public class UserBaseModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

public class UserAuthenticationModel : UserBaseModel
{
    public List<string> Roles { get; set; } = new();
}

public class UserForgotPasswordModel
{
    public string Email { get; set; } = string.Empty;
}

public class ChangeUserPasswordModel
{
    public Guid UserId { get; set; } = Guid.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class ResetUserPasswordModel
{
    public Guid UserId { get; set; } = Guid.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class UserModel : UserBaseModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset DateCreated { get; set; } = new();
}

public class UserListModel : UserBaseModel
{
    public string Name { get; set; } = string.Empty;
    public long DiscordUserId { get; set; } = 0;
    public string StripeCustomerId { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
    public bool IsActive { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset DateCreated { get; set; } = new();
}

public class UserRoleModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid UserId { get; set; } = Guid.Empty;
    public string Role { get; set; } = string.Empty;
}

public class UserSettignsModel
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid UserId { get; set; } = Guid.Empty;
    public bool IsDarkMode { get; set; } = false;
}

public class DiscordUserModel
{
    public string DiscordId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}