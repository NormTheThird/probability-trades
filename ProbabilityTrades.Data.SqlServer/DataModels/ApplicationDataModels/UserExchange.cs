using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class UserExchange
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; } = null!;

    public string ApiKey { get; set; } = null!;

    public string ApiSecret { get; set; } = null!;

    public string ApiPassphrase { get; set; } = null!;

    public DateTimeOffset DateCreated { get; set; }

    public virtual User User { get; set; } = null!;
}
