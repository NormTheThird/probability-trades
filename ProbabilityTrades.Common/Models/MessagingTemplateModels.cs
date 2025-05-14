namespace ProbabilityTrades.Common.Models;

public class ForgotPasswordEmailModel
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("forgot-password-url")]
    public string Url { get; set; }
}