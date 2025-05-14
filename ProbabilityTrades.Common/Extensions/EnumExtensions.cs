namespace ProbabilityTrades.Common.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum @enum)
    {
        var fieldInfo = @enum.GetType().GetField(@enum.ToString());
        var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes != null && attributes.Length > 0)
            return attributes[0].Description;
        else
            return @enum.ToString();
    }

    public static string GetDescription<TEnum>(this object @object) where TEnum : struct
    {
        return Enum.TryParse(@object.ToString(), out TEnum @enum) ? (@enum as Enum).GetDescription() : @object.ToString();
    }

    public static T GetValueFromDescription<T>(this string description) where T : Enum
    {
        foreach (var field in typeof(T).GetFields())
        {
            if (Attribute.GetCustomAttribute(field,
            typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                if (attribute.Description == description)
                    return (T)field.GetValue(null);
            }
            else
            {
                if (field.Name == description)
                    return (T)field.GetValue(null);
            }
        }

        throw new ArgumentException("Not found.", nameof(description));
        // Or return default(T);
    }

    public static string GetDataSourceDescription<T>(this CandlestickPattern candlestickPattern, DataSource dataSource)
    {
        if (dataSource.Equals(DataSource.Unknown))
            throw new NotImplementedException();
        else if (dataSource.Equals(DataSource.Kucoin))
        {
            return candlestickPattern switch
            {
                CandlestickPattern.Unknown => "Unknown",
                CandlestickPattern.OneMinute => "1min",
                CandlestickPattern.ThreeMinute => "3min",
                CandlestickPattern.FiveMinute => "5min",
                CandlestickPattern.FifteenMinute => "15min",
                CandlestickPattern.ThirtyMinute => "30min",
                CandlestickPattern.OneHour => "1hour",
                CandlestickPattern.TwoHour => "2hour",
                CandlestickPattern.FourHour => "4hour",
                CandlestickPattern.SixHour => "6hour",
                CandlestickPattern.EightHour => "8hour",
                CandlestickPattern.TwelveHour => "12hour",
                CandlestickPattern.OneDay => "1day",
                CandlestickPattern.OneWeek => "1week",
                _ => throw new NotImplementedException(),
            };
        }
        else if (dataSource.Equals(DataSource.Binance))
        {
            return candlestickPattern switch
            {
                CandlestickPattern.Unknown => "Unknown",
                CandlestickPattern.OneMinute => "1m",
                CandlestickPattern.ThreeMinute => "3m",
                CandlestickPattern.FiveMinute => "5m",
                CandlestickPattern.FifteenMinute => "15m",
                CandlestickPattern.ThirtyMinute => "30m",
                CandlestickPattern.OneHour => "1h",
                CandlestickPattern.TwoHour => "2h",
                CandlestickPattern.FourHour => "4h",
                CandlestickPattern.SixHour => "6h",
                CandlestickPattern.EightHour => "8h",
                CandlestickPattern.TwelveHour => "12h",
                CandlestickPattern.OneDay => "1d",
                CandlestickPattern.OneWeek => "1w",
                _ => throw new NotImplementedException(),
            };
        }
        else
            throw new NotImplementedException();

    }
}