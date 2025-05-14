namespace ProbabilityTrades.Common.Models;

public class StripeNewCustomerModel
{
    public Guid UserId { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class StripeApiProductAndPriceModel
{
    public string PriceId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new List<string>();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0.0m;
}

public class CheckoutSessionRequest
{
    public Guid UserId { get; set; } = Guid.Empty;
    public string PriceId { get; set; } = string.Empty;
}