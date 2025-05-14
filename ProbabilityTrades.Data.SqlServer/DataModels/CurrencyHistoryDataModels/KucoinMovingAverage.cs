using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.CurrencyHistoryDataModels;

public partial class KucoinMovingAverage
{
    public Guid Id { get; set; }

    public Guid KucoinId { get; set; }

    public decimal? MovingAverage3 { get; set; }

    public decimal? MovingAverage5 { get; set; }

    public decimal? MovingAverage8 { get; set; }

    public decimal? MovingAverage9 { get; set; }

    public decimal? MovingAverage13 { get; set; }

    public decimal? MovingAverage21 { get; set; }

    public decimal? MovingAverage34 { get; set; }

    public decimal? MovingAverage50 { get; set; }

    public decimal? MovingAverage55 { get; set; }

    public decimal? MovingAverage89 { get; set; }

    public decimal? MovingAverage144 { get; set; }

    public decimal? MovingAverage233 { get; set; }

    public string LastChangedBy { get; set; } = null!;

    public DateTimeOffset DateLastChanged { get; set; }

    public DateTimeOffset DateCreated { get; set; }
}
