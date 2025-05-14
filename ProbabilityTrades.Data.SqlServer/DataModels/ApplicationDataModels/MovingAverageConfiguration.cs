using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class MovingAverageConfiguration
{
    public Guid Id { get; set; }

    public string DataSource { get; set; } = null!;

    public string BaseCurrency { get; set; } = null!;

    public string QuoteCurrency { get; set; } = null!;

    public string CandlestickPattern { get; set; } = null!;

    public int ShortMovingAverageDays { get; set; }

    public int LongMovingAverageDays { get; set; }

    public decimal StopLossPercentage { get; set; }

    public bool IsActive { get; set; }

    public bool IsSendSMSNotification { get; set; }

    public bool IsSendDiscordNotification { get; set; }

    public string LastChangedBy { get; set; } = null!;

    public DateTimeOffset DateLastChanged { get; set; }

    public DateTimeOffset DateCreated { get; set; }
}
