namespace ProbabilityTrades.Common.Enumerations;

public enum MarketAction
{
    [Description("Position Unknown")] Unknown = 0,
    [Description("Enter Long Position")] EnterLong = 1,
    [Description("Exit Long Position")] ExitLong = 2,
    [Description("Exit Long Position Due To Stop Loss")] ExitLongStopLoss = 3,
    [Description("Enter Short Position")] EnterShort = 4,
    [Description("Exit Short Position")] ExitShort = 5,
    [Description("Exit Short Position Due To Stop Loss")] ExitShortStopLoss = 6
}