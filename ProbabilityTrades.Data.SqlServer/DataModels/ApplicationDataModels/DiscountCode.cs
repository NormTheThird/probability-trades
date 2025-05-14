using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class DiscountCode
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public decimal Discount { get; set; }

    public bool IsMultiUse { get; set; }

    public bool IsPercentage { get; set; }

    public DateTimeOffset DateCreated { get; set; }
}
