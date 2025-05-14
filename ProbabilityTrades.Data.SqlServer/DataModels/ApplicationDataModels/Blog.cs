using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class Blog
{
    public Guid Id { get; set; }

    public Guid CreatedByUserId { get; set; }

    public string Name { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string ShortDescription { get; set; } = null!;

    public string Body { get; set; } = null!;

    public bool IsPosted { get; set; }

    public DateTimeOffset? PostedDate { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    public virtual ICollection<BlogImage> BlogImages { get; set; } = new List<BlogImage>();

    public virtual User CreatedByUser { get; set; } = null!;
}
