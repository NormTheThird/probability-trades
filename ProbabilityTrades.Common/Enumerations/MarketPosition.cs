namespace ProbabilityTrades.Common.Enumerations;

public enum MarketPosition
{
    [Description("Position Unknown")] Unknown = 0,
    [Description("Cash Position")] Cash = 1,
    [Description("Long Position")] Long = 2,
    [Description("Short Position")] Short = 3
}