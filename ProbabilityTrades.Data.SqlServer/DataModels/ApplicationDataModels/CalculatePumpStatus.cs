using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class CalculatePumpStatus
{
    public Guid Id { get; set; }

    public string DataSource { get; set; } = null!;

    public string BaseCurrency { get; set; } = null!;

    public string QuoteCurrency { get; set; } = null!;

    public string CandlestickPattern { get; set; } = null!;

    public int Period { get; set; }

    public decimal ATR { get; set; }

    public decimal AverageVolume { get; set; }

    public decimal VolumeTarget { get; set; }

    public decimal CurrentCandleVolume { get; set; }

    public decimal PriceTarget { get; set; }

    public decimal CurrentCandlePrice { get; set; }

    public bool IsPumping { get; set; }

    public DateTimeOffset DateCreated { get; set; }
}
