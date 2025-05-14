using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class Subscription
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Title { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal DiscountPrice { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset DateCreated { get; set; }
}
