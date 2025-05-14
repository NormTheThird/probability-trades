namespace ProbabilityTrades.Domain.Formulas;

/// <summary>
///     Ichimoku Cloud is a technical analysis indicator developed by Japanese journalist Goichi Hosoda 
///     in the late 1930s. It is also known as Ichimoku Kinko Hyo, which means "equilibrium chart at a 
///     glance" in Japanese.
///
///     The Ichimoku Cloud is a comprehensive indicator that consists of several lines and a shaded area 
///     on a price chart. These lines and areas represent support and resistance levels, trend direction, 
///     and momentum. The indicator is widely used in trading to identify potential trend reversals, trend 
///     direction, and entry and exit points.
///
///     The components of the Ichimoku Cloud include:
///
///     1. Tenkan-sen (Conversion Line): A moving average of the high and low prices over the past 9 periods.
///     2. Kijun-sen (Base Line): A moving average of the high and low prices over the past 26 periods.
///     3. Senkou Span A (Leading Span A): The average of the Tenkan-sen and Kijun-sen, plotted 26 periods ahead.
///     4. Senkou Span B (Leading Span B): The average of the high and low prices over the past 52 periods, plotted 26 periods ahead.
///     5. Chikou Span (Lagging Span): The closing price of the current period, plotted 26 periods back.
///
///     The area between Senkou Span A and Senkou Span B is shaded and is known as the "cloud." The color 
///     of the cloud changes based on the trend direction: bullish if it's green and bearish if it's red. 
///     The thickness of the cloud indicates the strength of the trend. Traders use the Ichimoku Cloud to 
///     identify potential support and resistance levels, as well as to generate buy and sell signals.
///     
///     The formulas for each of the components are as follows:
///     
///     1. Tenkan-sen(Conversion Line) = (Highest high over the past 9 periods + Lowest low over the past 9 periods) / 2
///     2. Kijun-sen(Base Line) = (Highest high over the past 26 periods + Lowest low over the past 26 periods) / 2
///     3. Senkou Span A(Leading Span A) = (Tenkan-sen + Kijun-sen) / 2, plotted 26 periods ahead
///     4. Senkou Span B(Leading Span B) = (Highest high over the past 52 periods + Lowest low over the past 52 periods) / 2, plotted 26 periods ahead
///     5. Chikou Span(Lagging Span) = Closing price of the current period, plotted 26 periods behind
///
///     The shaded area between Senkou Span A and Senkou Span B is the Ichimoku Cloud.The thickness and color of 
///     the cloud change based on the direction of the trend.
/// </summary>
public static class IchimokuCloud
{
    /// <summary>
    ///     Calculates the Tenkan-sen line of the Ichimoku Cloud indicator.
    ///     The Tenkan-sen is a moving average of the high and low prices over the past 9 periods.
    ///     
    ///     Formula:    
    ///     Tenkan-sen(Conversion Line) = (Highest high over the past 9 periods + Lowest low over the past 9 periods) / 2
    /// </summary>
    /// <param name="highPrices">A list of high prices for the specified period.</param>
    /// <param name="lowPrices">A list of low prices for the specified period.</param>
    /// <returns>A list of decimal values representing the Tenkan-sen line.</returns>
    public static decimal CalculateTenkanSen(List<decimal> highPrices, List<decimal> lowPrices)
    {
        var highestHigh = decimal.MinValue;
        var lowestLow = decimal.MaxValue;

        for (int i = highPrices.Count - 9; i < highPrices.Count; i++)
        {
            if (highPrices[i] > highestHigh)
                highestHigh = highPrices[i];
            if (lowPrices[i] < lowestLow)
                lowestLow = lowPrices[i];
        }

        return (highestHigh + lowestLow) / 2;
    }

    /// <summary>
    ///     Calculates the Kijun-sen (Base Line) of the Ichimoku Cloud.
    ///     
    ///     Formula:   
    ///     Kijun-sen(Base Line) = (Highest high over the past 26 periods + Lowest low over the past 26 periods) / 2
    /// </summary>
    /// <param name="highs">A list of high prices for the past periods.</param>
    /// <param name="lows">A list of low prices for the past periods.</param>
    /// <returns>The Kijun-sen value for the current period.</returns>
    public static decimal CalculateKijunSen(List<decimal> highPrices, List<decimal> lowPrices)
    {
        var period = 26;

        var highestHigh = highPrices.Skip(highPrices.Count - period).Max();
        var lowestLow = lowPrices.Skip(lowPrices.Count - period).Min();

        return (highestHigh + lowestLow) / 2;
    }

    /// <summary>
    ///     Calculates the Senkou Span A (Leading Span A) of the Ichimoku Cloud.
    ///     
    ///     Formula:
    ///     Senkou Span B(Leading Span B) = (Highest high over the past 52 periods + Lowest low over the past 52 periods) / 2, plotted 26 periods ahead
    /// </summary>
    /// <param name="tenkanSen">A list of Tenkan-sen values for the past periods.</param>
    /// <param name="kijunSen">A list of Kijun-sen values for the past periods.</param>
    /// <returns>The Senkou Span A value for the current period, plotted 26 periods ahead.</returns>
    public static decimal CalculateSenkouSpanA(decimal tenkanSen, decimal kijunSen)
    {
        return (tenkanSen + kijunSen) / 2;
    }

    /// <summary>
    ///     Calculates the Senkou Span B (Leading Span B) value of an Ichimoku Kinko Hyo chart.
    ///     
    ///     Formula:
    ///     Senkou Span A(Leading Span A) = (Tenkan-sen + Kijun-sen) / 2, plotted 26 periods ahead
    /// </summary>
    /// <param name="highs">The List of decimal values representing the highest prices over a period of time.</param>
    /// <param name="lows">The List of decimal values representing the lowest prices over a period of time.</param>
    /// <returns>A decimal value representing the Senkou Span B (Leading Span B) value plotted 26 periods ahead.</returns>
    public static decimal CalculateSenkouSpanB(List<decimal> highPrices, List<decimal> lowPrices)
    {
        var lookbackPeriods = 52;
        if (highPrices.Count < lookbackPeriods)
            throw new ArgumentException("The number of elements in the highs and lows lists must be at least 52.");
        if (highPrices.Count != lowPrices.Count)
            throw new ArgumentException("The number of elements in the highs and lows lists must be equal.");

        var highestHigh = highPrices.GetRange(highPrices.Count - lookbackPeriods, lookbackPeriods).Max();
        var lowestLow = lowPrices.GetRange(lowPrices.Count - lookbackPeriods, lookbackPeriods).Min();

        return (highestHigh + lowestLow) / 2;
    }

    /// <summary>
    ///     Calculates the Chikou Span (Lagging Span) value of an Ichimoku Kinko Hyo chart.
    ///     
    ///     Formula:    
    ///     Chikou Span(Lagging Span) = Closing price of the current period, plotted 26 periods behind
    /// </summary>
    /// <param name="closedPrices">The List of decimal values representing the closing prices over a period of time.</param>
    /// <returns>A List of decimal values representing the Chikou Span (Lagging Span) values plotted 26 periods behind.</returns>
    public static decimal CalculateChikouSpan(List<decimal> closedPrices)
    {
        var laggingPeriods = 26;
        if (closedPrices.Count < laggingPeriods)
            throw new ArgumentException("The number of elements in the closes list must be at least 26.");

        return closedPrices[closedPrices.Count - laggingPeriods - 1];
        //var chikouSpan = new List<decimal>();
        //for (int i = 0; i < closedPrices.Count - laggingPeriods; i++)
        //    chikouSpan.Add(closedPrices[i + laggingPeriods]);

        //return chikouSpan;
    }
}