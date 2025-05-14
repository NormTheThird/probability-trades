using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class MovingAverageStatus
{
    public Guid Id { get; set; }

    public string DataSource { get; set; } = null!;

    public string BaseCurrency { get; set; } = null!;

    public string QuoteCurrency { get; set; } = null!;

    public DateOnly CloseDate { get; set; }

    public long ChartTimeEpoch { get; set; }

    public string CandlestickPattern { get; set; } = null!;

    public string MarketPosition { get; set; } = null!;

    public bool IsPositionChange { get; set; }

    public decimal ClosePrice { get; set; }

    public int ShortMovingAverageDays { get; set; }

    public decimal ShortMovingAverage { get; set; }

    public int LongMovingAverageDays { get; set; }

    public decimal LongMovingAverage { get; set; }

    public DateTimeOffset DateCreated { get; set; }
}
