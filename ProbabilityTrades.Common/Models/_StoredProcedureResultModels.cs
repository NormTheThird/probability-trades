namespace ProbabilityTrades.Common.Models;

public class GetCalculatePumpConfigurations_Result
{
    public Guid Id { get; set; } = Guid.Empty;
    public string DataSource { get; set; } = string.Empty;
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
    public string CandlestickPattern { get; set; } = string.Empty;
    public int Period { get; set; } = 0;
    public decimal ATRMultiplier { get; set; } = 0.0m;
    public decimal VolumeMultiplier { get; set; } = 0.0m;
    public bool IsActive { get; set; } = false;
    public bool IsSendDiscordNotification { get; set; } = false;
    public bool IsCurrentlyPumping { get; set; } = false;
    public string LastChangedBy { get; set; } = string.Empty;
    public DateTimeOffset DateLastChanged { get; set; } = new();
    public DateTimeOffset DateCreated { get; set; } = new();
}