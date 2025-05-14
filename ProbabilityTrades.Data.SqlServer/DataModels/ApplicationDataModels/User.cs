using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class User
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string DiscordUserId { get; set; } = null!;

    public string DiscordAccessToken { get; set; } = null!;

    public string DiscordRefreshToken { get; set; } = null!;

    public byte[] Salt { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public bool IsAdmin { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<CalculatePumpOrder> CalculatePumpOrders { get; set; } = new List<CalculatePumpOrder>();

    public virtual ICollection<StripeCustomer> StripeCustomers { get; set; } = new List<StripeCustomer>();

    public virtual ICollection<UserExchange> UserExchanges { get; set; } = new List<UserExchange>();

    public virtual ICollection<UserPasswordReset> UserPasswordResets { get; set; } = new List<UserPasswordReset>();

    public virtual ICollection<UserRefreshToken> UserRefreshTokens { get; set; } = new List<UserRefreshToken>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<UserSetting> UserSettings { get; set; } = new List<UserSetting>();
}
