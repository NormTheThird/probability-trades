using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class CurrencyHistoryProcess
{
    public Guid Id { get; set; }

    public string DataSource { get; set; } = null!;

    public string BaseCurrency { get; set; } = null!;

    public string QuoteCurrency { get; set; } = null!;

    public string CandlePattern { get; set; } = null!;

    public int IntervalsBack { get; set; }

    public bool IsActive { get; set; }

    public string LastChangedBy { get; set; } = null!;

    public DateTimeOffset DateLastChanged { get; set; }

    public DateTimeOffset DateCreated { get; set; }
}
