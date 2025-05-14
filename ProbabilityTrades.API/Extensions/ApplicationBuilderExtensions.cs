namespace ProbabilityTrades.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApiKey(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UseApiKeyMiddleware>();
    }
}