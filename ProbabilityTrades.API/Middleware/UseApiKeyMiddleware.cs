namespace ProbabilityTrades.API.Middleware;

public class UseApiKeyMiddleware
{
    private readonly RequestDelegate _next;

    public UseApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("x-api-key"))
        {
            var authResult = await context.AuthenticateAsync("ApiKey");
            if (authResult.Succeeded)
                context.User = authResult.Principal;
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("Invalid API Key");
                await context.Response.CompleteAsync();
            }
        }

        await _next(context);
    }
}