using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class UserPasswordReset
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    public virtual User User { get; set; } = null!;
}
