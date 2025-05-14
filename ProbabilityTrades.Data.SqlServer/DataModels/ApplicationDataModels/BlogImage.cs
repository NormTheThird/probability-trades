using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class BlogImage
{
    public Guid Id { get; set; }

    public Guid BlogId { get; set; }

    public string Name { get; set; } = null!;

    public string Url { get; set; } = null!;

    public bool IsMainImage { get; set; }

    public int ImageOrder { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    public virtual Blog Blog { get; set; } = null!;
}
