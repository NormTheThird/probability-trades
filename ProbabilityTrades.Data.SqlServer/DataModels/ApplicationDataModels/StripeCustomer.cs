using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class StripeCustomer
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string CustomerId { get; set; } = null!;

    public bool HasSubscription { get; set; }

    public DateTimeOffset LastSubscriptionDateChange { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    public virtual User User { get; set; } = null!;
}
