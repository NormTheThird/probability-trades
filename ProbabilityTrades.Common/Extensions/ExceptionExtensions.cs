namespace ProbabilityTrades.Common.Extensions;

public static class ExceptionExtensions
{
    public static string GetFullMessage(this Exception ex)
    {
        var messageBuilder = new StringBuilder();
        var innerExceptionMessage = "See the inner exception for details.";

        // Add the exception message
        if (!ex.Message.Contains(innerExceptionMessage))
            messageBuilder.Append(ex.Message);

        // Check if there's an inner exception
        if (ex.InnerException != null)
        {
            if (messageBuilder.Length > 0)
                messageBuilder.Append(Environment.NewLine);
            messageBuilder.Append(ex.InnerException.GetFullMessage());
        }

        return messageBuilder.ToString();
    }
}