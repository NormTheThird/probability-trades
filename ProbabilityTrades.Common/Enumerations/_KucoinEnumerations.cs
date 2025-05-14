namespace ProbabilityTrades.Common.Enumerations;

public enum AccountType
{
    [Description("Show All Account Types")] Unknown,
    [Description("Main Account")] Main,
    [Description("Trading Account")] Trade,
    [Description("Margin Account")] Margin
}

public enum OrderType
{
    Unknown,
    Limit,
    Market,
    Limit_Stop,
    Market_Stop
}

public enum TradeType
{
    [Description("Show All Trading Types")] Unknown,
    [Description("Spot Trading")] TRADE,
    [Description("Cross Margin Trading")] MARGIN_TRADE,
    [Description("Isolated Margin Trading")] MARGIN_ISOLATED_TRADE,
}