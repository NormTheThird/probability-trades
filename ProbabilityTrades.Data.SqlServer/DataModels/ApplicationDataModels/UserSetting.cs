using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class UserSetting
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public bool IsDarkMode { get; set; }

    public virtual User User { get; set; } = null!;
}
