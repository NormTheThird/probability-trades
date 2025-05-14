namespace ProbabilityTrades.Common.Enumerations;

public enum DiscordChannel : long
{
    // Probability Trades Subscriber
    [Description("Trade Signals")] TradeSignals = 1038518893469237419,

    // Free Public Channels
    [Description("Rules And Acknoledgements")] RulesAndAcknoledgements = 1179182351000031362,
    [Description("Crypto Pump Alerts")] CryptoPumpAlerts = 1038527377157914736,
    [Description("Blog Posts")] BlogPosts = 1039744272821395526,

    // Admin Channels
    [Description("Audit Log")] AuditLog = 1179202618250252359,
}

/// <summary>
///     Everyone: open to the public and can see nothing until acknowledgments
///     Verified: after acknowledgment
///     PT-Member: they have active paid subscription.
/// </summary>
public enum DiscordRoles : long
{
    [Description("Everyone")] Everyone = 1038518893012070400,
    [Description("Verified")] Verified = 1179183593566765151,
    [Description("PT-Member")] Subscriber = 1039208847446908988,
}