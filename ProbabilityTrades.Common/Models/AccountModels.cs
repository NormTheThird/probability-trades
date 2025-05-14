namespace ProbabilityTrades.Common.Models;



public class AccountBalanceDetailModel
{
    public string Asset { get; set; } = string.Empty;
    public string AlternateName { get; set; } = string.Empty;
    public int Decimals { get; set; } = 0;
    public int DisplayDecimals { get; set; } = 0;
    public decimal Balance { get; set; } = 0.0m;
    public decimal AvailableBalance { get; set; } = 0.0m;
    public decimal EstimatedValueUSD { get; set; } = 0.0m;
    public decimal CurrentPriceInUSD { get; set; } = 0.0m;
}