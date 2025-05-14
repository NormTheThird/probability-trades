using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class UserRefreshToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string RefreshToken { get; set; } = null!;

    public DateTimeOffset DateIssued { get; set; }

    public DateTimeOffset? DateExpired { get; set; }

    public virtual User User { get; set; } = null!;
}
