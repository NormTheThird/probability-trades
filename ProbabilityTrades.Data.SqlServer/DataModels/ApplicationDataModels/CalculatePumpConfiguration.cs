using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class CalculatePumpConfiguration
{
    public Guid Id { get; set; }

    public string DataSource { get; set; } = null!;

    public string BaseCurrency { get; set; } = null!;

    public string QuoteCurrency { get; set; } = null!;

    public string CandlestickPattern { get; set; } = null!;

    public int Period { get; set; }

    public decimal ATRMultiplier { get; set; }

    public decimal VolumeMultiplier { get; set; }

    public bool IsActive { get; set; }

    public bool IsSendDiscordNotification { get; set; }

    public string LastChangedBy { get; set; } = null!;

    public DateTimeOffset DateLastChanged { get; set; }

    public DateTimeOffset DateCreated { get; set; }
}
