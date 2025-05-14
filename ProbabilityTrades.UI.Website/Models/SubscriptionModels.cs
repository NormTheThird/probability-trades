namespace ProbabilityTrades.UI.Website.Models;

public class SubscriptionModel
{
    public string PriceId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0.0m;
    public List<string> Features { get; set; } = new();
    public bool IsSendToStripeCheckout { get; set; } = false;
}