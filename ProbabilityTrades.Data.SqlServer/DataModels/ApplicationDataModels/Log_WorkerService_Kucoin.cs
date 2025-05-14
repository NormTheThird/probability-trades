using System;
using System.Collections.Generic;

namespace ProbabilityTrades.Data.SqlServer.DataModels.ApplicationDataModels;

public partial class Log_WorkerService_Kucoin
{
    public int Id { get; set; }

    public string? Message { get; set; }

    public string? MessageTemplate { get; set; }

    public string? Level { get; set; }

    public DateTime? TimeStamp { get; set; }

    public string? Exception { get; set; }

    public string? Properties { get; set; }
}
