namespace ProbabilityTrades.Common.Extensions;

public static class ClassExtensions
{
    public static string GetStringValue<T>(this T classObject) where T : class
    {
        var propertyInfos = classObject.GetType().GetProperties();
        var retval = string.Empty;
        foreach (PropertyInfo pInfo in propertyInfos)
        {
            var propertyName = pInfo.Name;
            var propertyValue = pInfo.GetValue(classObject, null);
            retval += $"[{propertyName}: {propertyValue}] ";
        }
        return retval.Trim();
    }
}