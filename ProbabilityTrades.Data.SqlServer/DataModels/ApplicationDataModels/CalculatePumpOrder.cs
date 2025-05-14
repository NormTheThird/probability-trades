using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class CalculatePumpOrder
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string DataSource { get; set; } = null!;

    public string BaseCurrency { get; set; } = null!;

    public string QuoteCurrency { get; set; } = null!;

    public string OpenedOrderId { get; set; } = null!;

    public DateTimeOffset OpenedTimeUTC { get; set; }

    public decimal OpenedAmount { get; set; }

    public decimal OpenedMarketPrice { get; set; }

    public string StopOrderId { get; set; } = null!;

    public decimal StopPrice { get; set; }

    public decimal OrderQuantity { get; set; }

    public string? ClosedOrderId { get; set; }

    public DateTimeOffset? ClosedTimeUTC { get; set; }

    public decimal? ClosedAmount { get; set; }

    public decimal? ClosedMarketPrice { get; set; }

    public bool ExecutedStop { get; set; }

    public string LastChangedBy { get; set; } = null!;

    public DateTimeOffset DateLastChanged { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    public virtual User User { get; set; } = null!;
}
