namespace ProbabilityTrades.Domain.Formulas;

/// <summary>
///     Average Directional Index (ADX)
///     
///     The Average Directional Index (ADX) is a technical indicator used to measure 
///     the strength of a trend in a financial market.
///     
///     * The ADX value ranges from 0 to 100, with higher values indicating stronger 
///       trends and lower values indicating weaker trends or a market that is not trending.
///     * The ADX is often used in conjunction with the Plus Directional Indicator (+DI) and the Minus 
///       Directional Indicator (-DI), which are calculated as part of the Directional Movement Index 
///       (DMI) system. The +DI measures upward movement and the -DI measures downward movement.
///     * Traders may look for certain levels of the ADX to signal when to enter or exit a trade. For example, 
///       a crossover of the +DI and -DI lines accompanied by a rising ADX value above a certain threshold 
///       (such as 25 or 30) may be considered a bullish signal.
///     * However, it's important to note that the ADX is a lagging indicator, meaning it may not be effective 
///       in predicting trend reversals or sudden market movements. As with any technical indicator, it should 
///       be used in conjunction with other analysis techniques and risk management strategies.
///
///     The formula for the Average Directional Index (ADX) is as follows:
///     
///     1. Calculate the True Range (TR) for each period:
///        TR is the greater of the current high - current low, current high - previous close, or current low - previous close.
///        TR = max(current high - current low, abs(current high - previous close), abs(curretn low - previous close))
///     2. Calculate the Directional Movement (+DM and -DM) for each period:
///        +DM = current high - previous high if (current high - previous high) > (previous low - current low), otherwise 0
///        -DM = previous low - current low if (previous low - currnet low) > (current high - previous high), otherwise 0
///     3. Calculate the 14-period Average True Range (ATR):
///        ATR = (TR1 + TR2 + ... + TR14) / 14
///     4. Calculate the 14-period Directional Movement Index (DX):
///        +DI14 = 100 * (EMA(+DM, 14) / ATR)
///        -DI14 = 100 * (EMA(-DM, 14) / ATR)
///        DX = 100 * (ABS(+DI14 - -DI14) / (+DI14 + -DI14))
///        * where EMA is the Exponential Moving Average and 14 is the smoothing period.
///     5. Calculate the 14-period Average Directional Index (ADX):
///        ADX = EMA(DX, 14)
///        * where EMA is the Exponential Moving Average and 14 is the smoothing period.
///        
/// 
///     1.  Calculate +DM, -DM, and the true range (TR) for each period. Fourteen periods are typically used.
///     2.  +DM = current high - previous high.
///     3.  -DM = previous low - current low.
///     4.  Use +DM when current high - previous high > previous low - current low.
///         Use -DM when previous low - current low > current high - previous high.
///     5.  TR is the greater of the current high - current low, current high - previous close, or current low - previous close.
///     6.  Smooth the 14-period averages of +DM, -DM, and TR—the TR formula is below. Insert the -DM and +DM values to calculate the smoothed averages of those.
///     7.  First 14TR = sum of first 14 TR readings.
///     8.  Next 14TR value = first 14TR - (prior 14TR/14) + current TR.
///     9.  Next, divide the smoothed +DM value by the smoothed TR value to get +DI. Multiply by 100.
///     10. Divide the smoothed -DM value by the smoothed TR value to get -DI. Multiply by 100.
///     11. The directional movement index (DMI) is +DI minus -DI, divided by the sum of +DI and -DI (all absolute values). Multiply by 100.
///     12. To get the ADX, continue to calculate DX values for at least 14 periods. Then, smooth the results to get ADX.
///     13. First ADX = sum 14 periods of DX / 14.
///     14. After that, ADX = ((prior ADX * 13) + current DX) / 14.
/// </summary>
public static class AverageDirectionalIndex
{
    /// <summary>
    ///     Calculates The Average Directional Index (ADX)
    /// </summary>
    /// <param name="high">A list of high prices for each period.</param>
    /// <param name="low">A list of low prices for each period.</param>
    /// <param name="close">A list of closing prices for each period.</param>
    /// <param name="period"></param>
    /// <returns>The Average Directional Index (ADX) as a decimal</returns>
    public static decimal CalculateADX(List<decimal> high, List<decimal> low, List<decimal> close, int period = 14)
    {
        // First, we need to calculate the True Range (TR) for each period.
        // We'll use a helper method to do this.
        //var tr = CalculateTrueRange(high, low, close);

        // Next, we need to calculate the Directional Movement (+DM and -DM) for each period.
        // Again, we'll use a helper method to do this.
        var pdm = CalculatePlusDM(high, low);
        var ndm = CalculateMinusDM(high, low);

        // Calculate the 14-period Average True Range (ATR).
        var atr = CalculateAverageTrueRange(high, low, close, period);

        // Finally, we'll calculate the 14-period Directional Movement Index (DX) and the ADX.
        var diPlus = new List<decimal>();
        var diMinus = new List<decimal>();
        var dx = new List<decimal>();
        var adx = new List<decimal>();

        for (int i = 0; i < high.Count; i++)
        {
            if (i < period)
            {
                diPlus.Add(0.0m);
                diMinus.Add(0.0m);
                dx.Add(0.0m);
                adx.Add(0.0m);
            }
            else
            {
                var diPlusValue = 100 * (CalculateEMA(pdm.Skip(i - period).Take(period).ToList(), period) / atr);
                var diMinusValue = 100 * (CalculateEMA(ndm.Skip(i - period).Take(period).ToList(), period) / atr);
                diPlus.Add(diPlusValue);
                diMinus.Add(diMinusValue);

                var dxValue = 100 * Math.Abs(diPlusValue - diMinusValue) / (diPlusValue + diMinusValue);
                dx.Add(dxValue);

                if (i == period)
                {
                    adx.Add(dx.Skip(i - period).Take(period).Average());
                }
                else if (i > period)
                {
                    var adxValue = CalculateEMA(dx.Skip(i - period).Take(period).ToList(), period);
                    adx.Add(adxValue);
                }
            }
        }

        // The ADX is the last value in the ADX list.
        return adx.Last();
    }

    public static (decimal ADX, decimal DIPlus, decimal DIMinus) CalculateADXandDX(List<decimal> high, List<decimal> low, List<decimal> close, int period = 14)
    {
        // First, we need to calculate the True Range (TR) for each period.
        //var tr = CalculateTrueRange(high, low, close);

        // Now we'll calculate the 14-period Average True Range (ATR).
        var atr = CalculateAverageTrueRange(high, low, close, period);

        // Next, we need to calculate the Directional Movement (+DM and -DM) for each period.
        // Again, we'll use a helper method to do this.
        var pdm = CalculatePlusDM(high, low);
        var ndm = CalculateMinusDM(high, low);



        // Finally, we'll calculate the 14-period Directional Movement Index (DX) and the ADX.
        var diPlus = new List<decimal>();
        var diMinus = new List<decimal>();
        var dx = new List<decimal>();
        var adx = new List<decimal>();

        for (int i = 0; i < high.Count; i++)
        {
            if (i < period)
            {
                diPlus.Add(0.0m);
                diMinus.Add(0.0m);
                dx.Add(0.0m);
                adx.Add(0.0m);
            }
            else
            {
                var diPlusValue = 100 * (CalculateEMA(pdm.Skip(i - period).Take(period).ToList(), period) / atr);
                var diMinusValue = 100 * (CalculateEMA(ndm.Skip(i - period).Take(period).ToList(), period) / atr);
                diPlus.Add(diPlusValue);
                diMinus.Add(diMinusValue);

                var dxValue = 100 * Math.Abs(diPlusValue - diMinusValue) / (diPlusValue + diMinusValue);
                dx.Add(dxValue);

                if (i == period)
                {
                    adx.Add(dx.Skip(i - period).Take(period).Average());
                }
                else if (i > period)
                {
                    var adxValue = CalculateEMA(dx.Skip(i - period).Take(period).ToList(), period);
                    adx.Add(adxValue);
                }
            }
        }

        // The ADX, +DI, and -DI values are the last values in their respective lists.
        return (adx.Last(), diPlus.Last(), diMinus.Last());
    }

    /// <summary>
    ///     Calculates the True Range (TR) for each period in a financial market, given a list of high, low, and close prices.
    ///     The True Range is the greatest of the following:
    ///         * The difference between the current high and low prices.
    ///         * The absolute value of the difference between the current high price and the previous closing price.
    ///         * The absolute value of the difference between the current low price and the previous closing price.
    /// 
    ///     Formula:
    ///     TR = max(high - low, abs(high - previous close), abs(low - previous close))
    /// </summary>
    /// <param name="high">A list of high prices for each period.</param>
    /// <param name="low">A list of low prices for each period.</param>
    /// <param name="close">A list of closing prices for each period.</param>
    /// <returns>A list of True Range (TR) values for each period.</returns>
    private static List<decimal> CalculateTrueRange(List<decimal> high, List<decimal> low, List<decimal> close)
    {
        var trueRanges = new List<decimal>();
        for (int i = 0; i < high.Count; i++)
        {
            if (i == 0)
                trueRanges.Add(high[i] - low[i]);
            else
            {
                var trValue = Math.Max(high[i] - low[i], Math.Max(Math.Abs(high[i] - close[i - 1]), Math.Abs(low[i] - close[i - 1])));
                trueRanges.Add(trValue);
            }
        }

        return trueRanges;
    }

    /// <summary>
    ///     Calculates the Average True Range (ATR) over a given period.
    ///     
    ///     Formula:
    ///     ATR = (TR1 + TR2 + ... + TR14) / 14
    /// </summary>
    /// <param name="trueRange">A list of True Range values.</param>
    /// <param name="period">The number of periods to use in the calculation.</param>
    /// <returns>The Average True Range value.</returns>
    //private static decimal CalculateAverageTrueRange(List<decimal> trueRange, int period)
    //{
    //    var atr = trueRange.Take(period).Average();
    //    for (int i = period; i < trueRange.Count; i++)
    //        atr = ((period - 1) * atr + trueRange[i]) / period;

    //    return atr;
    //}

    public static decimal CalculateAverageTrueRange(List<decimal> high, List<decimal> low, List<decimal> close, int period)
    {
        if (high.Count != low.Count || high.Count != close.Count)
            throw new ArgumentException("Input lists (highs, lows, closes) must have the same count.");

        decimal atr = 0.0m;
        for (var i = 0; i < period; i++)
        {
            var trueRange = 0.0m;
            if (i == 0)
            {
                trueRange = high[i] - low[i];
            }
            else
            {
                var highMinusLow = high[i] - low[i];
                var highMinusPrevClose = Math.Abs(high[i] - close[i - 1]);
                var lowMinusPrevClose = Math.Abs(low[i] - close[i - 1]);
                trueRange = Math.Max(highMinusLow, Math.Max(highMinusPrevClose, lowMinusPrevClose));
            }
            atr += trueRange;
        }

        atr /= period;

        for (var i = period; i < high.Count; i++)
        {
            var trueRange = 0.0m;
            var highMinusLow = high[i] - low[i];
            var highMinusPrevClose = Math.Abs(high[i] - close[i - 1]);
            var lowMinusPrevClose = Math.Abs(low[i] - close[i - 1]);
            trueRange = Math.Max(highMinusLow, Math.Max(highMinusPrevClose, lowMinusPrevClose));
            atr = ((period - 1) * atr + trueRange) / period;
        }

        return atr;
    }



    /// <summary>
    ///     Calculates the Plus Directional Movement (+DM) for each period in a financial market based on the given highs and lows.
    ///     The Plus Directional Movement (+DM) is a component of the Average Directional Index (ADX) technical indicator used
    ///     to measure the strength of a trend in a financial market. 
    ///     
    ///     Formula:
    ///     +DM = high - previous high if (high - previous high) > (previous low - low), otherwise 0
    /// </summary>
    /// <param name="high">List of high prices for each period in the market.</param>
    /// <param name="low">List of low prices for each period in the market.</param>
    /// <returns>List of Plus Directional Movement (+DM) values for each period in the market.</returns>
    private static List<decimal> CalculatePlusDM(List<decimal> high, List<decimal> low)
    {
        var plusDM = new List<decimal>();
        for (int i = 0; i < high.Count; i++)
        {
            if (i == 0)
                plusDM.Add(0);
            else
            {
                var upMove = high[i] - high[i - 1];
                var downMove = low[i - 1] - low[i];
                var plusDMValue = upMove > downMove && upMove > 0 ? upMove : 0;
                plusDM.Add(plusDMValue);
            }
        }

        return plusDM;
    }

    /// <summary>
    ///     Calculates the Minus Directional Movement (-DM) for each period.
    ///     
    ///     Formula:
    ///     -DM = previous low - low if (previous low - low) > (high - previous high), otherwise 0
    /// </summary>
    /// <param name="high">List of high prices for each period.</param>
    /// <param name="low">List of low prices for each period.</param>
    /// <returns>List of Minus Directional Movement (-DM) values for each period.</returns>
    private static List<decimal> CalculateMinusDM(List<decimal> high, List<decimal> low)
    {
        var minusDM = new List<decimal>();
        for (int i = 0; i < high.Count; i++)
        {
            if (i == 0)
                minusDM.Add(0);
            else
            {
                var downMove = high[i - 1] - low[i];
                var upMove = 0.0m;

                if (i > 1)
                {
                    var prevClose = i == 1 ? (high[i - 1] + low[i - 1]) / 2 : (high[i - 2] + low[i - 2]) / 2;
                    upMove = Math.Max(0, high[i - 1] - prevClose);
                    downMove = Math.Max(downMove, Math.Abs(low[i] - high[i - 1]));
                }

                var minusDMValue = downMove > upMove ? downMove : 0;
                minusDM.Add(minusDMValue);
            }
        }

        return minusDM;
    }


    /// <summary>
    ///     Calculates the Exponential Moving Average (EMA) for a given period using the provided data.
    ///     
    ///     Formula:
    ///     EMA = (P * (C - Pn)) + Pn, where:
    ///         - P = the multiplier (2 / (period + 1))
    ///         - C = the current closing price
    ///         - Pn = the previous period's EMA
    /// </summary>
    /// <param name="data">The list of decimal values to calculate the EMA for.</param>
    /// <param name="period">The period to use for calculating the EMA.</param>
    /// <returns>The calculated EMA for the given period.</returns>
    private static decimal CalculateEMA(List<decimal> data, int period)
    {
        var multiplier = 2.0m / (period + 1);
        var ema = data.Take(period).Average();

        for (int i = period; i < data.Count; i++)
            ema = (data[i] - ema) * multiplier + ema;

        return ema;
    }
}

/// <summary>
///     The AverageDirectionalIndex class calculates the Average Directional Index (ADX), 
///     along with the Positive Directional Index (+DI) and Negative Directional Index (-DI) 
///     for the given high, low, and close price data.
/// </summary>
/// <remarks>
///     Usage:
///     Instantiate the AverageDirectionalIndex class with the high, low, and close price data as arguments.
///     Then, call the RunAverageDirection method to get the +DI, -DI, and ADX values.
/// </remarks>
public class AverageDirectionalIndex2
{
    private List<decimal> testHigh;
    private List<decimal> testLow;
    private List<decimal> testClose;

    private List<decimal> trueRange = new();
    private List<decimal> plusDirectionalMov = new();
    private List<decimal> negaDirectionalMov = new();
    private List<decimal> trueRangeSmoothed = new();
    private List<decimal> plusDirectionalMovSmoothed = new();
    private List<decimal> negaDirectionalMovSmoothed = new();
    private List<decimal> posDirectionalIndex = new();
    private List<decimal> negDirectionalIndex = new();
    private List<decimal> directionalMovementIndex = new();
    private List<decimal> avgDirectionIndex = new();

    public AverageDirectionalIndex2(List<decimal> high, List<decimal> low, List<decimal> close)
    {
        testHigh = high;
        testLow = low;
        testClose = close;
    }

    /// <summary>
    ///     Calculates the Average Directional Index (ADX) along with the Positive Directional Index (+DI) 
    ///     and Negative Directional Index (-DI) for the given data.
    /// </summary>
    /// <remarks>
    ///     The method performs the following steps:
    ///     1. Calculate the True Range, Positive Directional Movement, and Negative Directional Movement.
    ///     2. Apply Wilder's Smoothing Technique to smooth the True Range, Positive Directional Movement, and Negative Directional Movement values.
    ///     3. Calculate the Positive Directional Index (+DI) and Negative Directional Index (-DI).
    ///     4. Compute the Directional Movement Index (DX).
    ///     5. Calculate the Average Directional Index (ADX) using the Directional Movement Index values.
    ///
    ///     Usage:
    ///     Instantiate the AverageDirectionalIndex class with the high, low, and close price data as arguments. 
    ///     Then, call the RunAverageDirection method to get the +DI, -DI, and ADX values.
    ///
    ///     Formulas:
    ///     True Range (TR) = max(High - Low, abs(High - Previous Close), abs(Low - Previous Close))
    ///     Positive Directional Movement (+DM) = max(High - Previous High, 0) if High - Previous High > Previous Low - Low, otherwise 0
    ///     Negative Directional Movement (-DM) = max(Previous Low - Low, 0) if Previous Low - Low > High - Previous High, otherwise 0
    ///     Smoothed TR, +DM, and -DM using Wilder's Smoothing Technique.
    ///     +DI = 100 * (Smoothed +DM / Smoothed TR)
    ///     -DI = 100 * (Smoothed -DM / Smoothed TR)
    ///     Directional Movement Index (DX) = 100 * (abs(+DI - -DI) / (+DI + -DI))
    ///     Average Directional Index (ADX) = The smoothed average of DX values using Wilder's Smoothing Technique.
    /// </remarks>
    /// <returns>
    ///     A tuple containing lists of the calculated Positive Directional Index (+DI), Negative Directional Index (-DI), 
    ///     and Average Directional Index (ADX) values.
    /// </returns>
    public (List<decimal> PosDirectionalIndex, List<decimal> NegDirectionalIndex, List<decimal> AvgDirectionIndex) RunAverageDirection()
    {
        CalculateTrueRange();

        trueRangeSmoothed = MovingWilderSmoothing(trueRange);
        plusDirectionalMovSmoothed = MovingWilderSmoothing(plusDirectionalMov);
        negaDirectionalMovSmoothed = MovingWilderSmoothing(negaDirectionalMov);

        (posDirectionalIndex, negDirectionalIndex, directionalMovementIndex) = FindDirectionalIndex(trueRangeSmoothed, plusDirectionalMovSmoothed, negaDirectionalMovSmoothed);

        avgDirectionIndex = AverageDirectionalIndex(directionalMovementIndex);

        return (posDirectionalIndex, negDirectionalIndex, avgDirectionIndex);
    }

    /// <summary>
    ///     Calculates the True Range for the given high, low, and previous close price data.
    /// </summary>
    /// <returns>The calculated True Range value.</returns>
    private decimal TrueRangeCalculate(decimal high, decimal low, decimal yesterdayClose)
    {
        var hlDiff = high - low;
        var hYc = Math.Abs(high - yesterdayClose);
        var lYc = Math.Abs(low - yesterdayClose);

        var trueRange = 0.0m;
        if (hYc <= hlDiff && hlDiff >= lYc)
            trueRange = hlDiff;
        else if (hlDiff <= hYc && hYc >= lYc)
            trueRange = hYc;
        else
            trueRange = lYc;

        return trueRange;
    }

    /// <summary>
    ///     Calculates the Positive and Negative Directional Movement for the given high and low price data.
    /// </summary>
    /// <returns>
    ///     A tuple containing the calculated Negative Directional Movement and Positive Directional Movement values.
    /// </returns>
    private (decimal NegativeDirectionalMov, decimal PositiveDirectionalMov) DirectionalMovement(decimal todHigh, decimal todLow, decimal yestHigh, decimal yestLow)
    {
        var moveUp = todHigh - yestHigh;
        var moveDown = yestLow - todLow;
        var positiveDirectionalMov = 0.0m;
        var negativeDirectionalMov = 0.0m;

        if (0 < moveUp && moveUp > moveDown)
            positiveDirectionalMov = moveUp;

        if (0 < moveDown && moveDown > moveUp)
            negativeDirectionalMov = moveDown;

        return (negativeDirectionalMov, positiveDirectionalMov);
    }

    /// <summary>
    ///     Calculates the True Range and Directional Movement values for the given high, low, and close price data.
    /// </summary>
    private void CalculateTrueRange()
    {
        for (int ind = 1; ind < testHigh.Count; ind++)
        {
            trueRange.Add(TrueRangeCalculate(testHigh[ind], testLow[ind], testClose[ind - 1]));

            (decimal ngDirMov, decimal plDirMov) = DirectionalMovement(testHigh[ind], testLow[ind], testHigh[ind - 1], testLow[ind - 1]);
            plusDirectionalMov.Add(plDirMov);
            negaDirectionalMov.Add(ngDirMov);
        }
    }

    /// <summary>
    ///     Applies Wilder's Smoothing Technique to smooth the given list of moving values.
    /// </summary>
    /// <returns>A list of smoothed moving values.</returns>
    private List<decimal> MovingWilderSmoothing(List<decimal> movingValues)
    {
        var smoothedMovingValues = new List<decimal> { movingValues.Take(14).Sum() };
        for (int ind = 14; ind < movingValues.Count; ind++)
        {
            var previousSmoothedMovValue = smoothedMovingValues[ind - 14];
            var currentMovingValue = movingValues[ind];
            var currentSmoothedMovValue = previousSmoothedMovValue - (previousSmoothedMovValue / 14) + currentMovingValue;
            smoothedMovingValues.Add(currentSmoothedMovValue);
        }

        return smoothedMovingValues;
    }

    /// <summary>
    ///     Calculates the Positive Directional Index (+DI), Negative Directional Index (-DI), 
    ///     and Directional Movement Index (DX) using the smoothed True Range, Positive Directional 
    ///     Movement, and Negative Directional Movement values.
    /// </summary>
    /// <returns>
    ///     A tuple containing lists of the calculated Positive Directional Index (+DI), 
    ///     Negative Directional Index (-DI), and Directional Movement Index (DX) values.
    /// </returns>
    private (List<decimal> posDirInd, List<decimal> negDirInd, List<decimal> directionalIndex) FindDirectionalIndex(
        List<decimal> smoothedTrueRange, List<decimal> posDirectionalMov, List<decimal> negDirectionalMov)
    {
        var posDirInd = new List<decimal>();
        var negDirInd = new List<decimal>();
        var directionalIndex = new List<decimal>();

        for (int ind = 0; ind < smoothedTrueRange.Count; ind++)
        {
            posDirInd.Add((posDirectionalMov[ind] / smoothedTrueRange[ind]) * 100);
            negDirInd.Add((negDirectionalMov[ind] / smoothedTrueRange[ind]) * 100);
            var diffInd = Math.Abs(posDirInd[ind] - negDirInd[ind]);
            var sumInd = posDirInd[ind] + negDirInd[ind];
            directionalIndex.Add((diffInd / sumInd) * 100);
        }

        return (posDirInd, negDirInd, directionalIndex);
    }

    /// <summary>
    ///     Calculates the Average Directional Index (ADX) using the given list of Directional Movement Index (DX) values.
    /// </summary>
    /// <returns>A list of the calculated Average Directional Index (ADX) values.</returns>
    private List<decimal> AverageDirectionalIndex(List<decimal> directionalMovementIndex)
    {
        var avgDirectIndex = new List<decimal> { directionalMovementIndex.Take(14).Average() };
        for (int ind = 14; ind < directionalMovementIndex.Count; ind++)
            avgDirectIndex.Add((avgDirectIndex[ind - 14] * 13 + directionalMovementIndex[ind]) / 14);
        return avgDirectIndex;
    }
}