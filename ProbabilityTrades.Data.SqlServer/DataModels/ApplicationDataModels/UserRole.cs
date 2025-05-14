using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class UserRole
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Role { get; set; } = null!;

    public DateTimeOffset DateCreated { get; set; }

    public virtual User User { get; set; } = null!;
}
