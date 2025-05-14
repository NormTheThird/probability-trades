using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.CurrencyHistoryDataModels;

public partial class KucoinArchive
{
    public Guid Id { get; set; }

    public string BaseCurrency { get; set; } = null!;

    public string QuoteCurrency { get; set; } = null!;

    public string CandlestickPattern { get; set; } = null!;

    public long ChartTimeEpoch { get; set; }

    public DateTimeOffset ChartTimeUTC { get; set; }

    public DateTimeOffset? ChartTimeCST { get; set; }

    public decimal OpeningPrice { get; set; }

    public decimal ClosingPrice { get; set; }

    public decimal HighestPrice { get; set; }

    public decimal LowestPrice { get; set; }

    public decimal Volume { get; set; }

    public decimal Turnover { get; set; }

    public string LastChangedBy { get; set; } = null!;

    public DateTimeOffset DateLastChanged { get; set; }

    public DateTimeOffset DateCreated { get; set; }
}
