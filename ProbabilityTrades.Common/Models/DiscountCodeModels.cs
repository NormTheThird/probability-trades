namespace ProbabilityTrades.Common.Models;

public class ValidateDiscountCodeModel
{
    public Guid DiscountCodeId { get; set; } = Guid.Empty;
    public bool IsValid { get; set; } = false;
    public decimal Discount { get; set; } = 0.0m;
    public bool IsPercentage { get; set; } = false;
}