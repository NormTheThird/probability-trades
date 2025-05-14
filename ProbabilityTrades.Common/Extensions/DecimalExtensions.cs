namespace ProbabilityTrades.Common.Extensions;

public static class DecimalExtensions
{
    public static int GetDecimalPlaces(this decimal number)
    {
        var numberString = number.ToString();
        var decimalPointIndex = numberString.IndexOf(".");
        return decimalPointIndex >= 0 ? numberString.Length - decimalPointIndex - 1 : 0;
    }

    public static decimal RoundToPrecision(this decimal number, int precision)
    {
        if (precision < 0 || precision > 12)
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must be between 0 and 12.");

        return decimal.Round(number, precision, MidpointRounding.ToZero);
    }

    /// <summary>
    ///     Rounds the decimal to the precision compatible with the currency provided
    /// </summary>
    /// <param name="number">The number to round</param>
    /// <param name="baseCurrency">The currency to determine the rounding precision, etc. 'BTC'</param>
    /// <returns></returns>
    public static decimal GetCurrencySpecificRounding(this decimal number, string baseCurrency)
    {
        if (baseCurrency.Equals("BTC", StringComparison.InvariantCultureIgnoreCase))
            return decimal.Round(number, 2, MidpointRounding.ToZero);
        
        return number;
    }
}