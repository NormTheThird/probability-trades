using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class Market
{
    public Guid Id { get; set; }

    public string DataSource { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public string LastChangedBy { get; set; } = null!;

    public DateTimeOffset DateLastChanged { get; set; }

    public DateTimeOffset DateCreated { get; set; }
}
