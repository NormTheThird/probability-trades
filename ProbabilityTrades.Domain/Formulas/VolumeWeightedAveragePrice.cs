namespace ProbabilityTrades.Domain.Formulas;

/// <summary>
///     Volume Weighted Average Price (VWAP)
///     
///     Volume Weighted Average Price (VWAP) is a trading indicator used to calculate the average price a security has 
///     traded at throughout the day, taking into account both the price and volume of each trade. It is calculated by 
///     dividing the total value of trades by the total volume of trades over a specific time period.
///     
///     The formula for Volume Weighted Average Price (VWAP) is:
///     
///     VWAP = Σ(Pi x Vi) / ΣVi
///     
///     Where:
///     * VWAP is the Volume Weighted Average Price
///     * Σ is the sum of all trades
///     * Pi is the price of each trade
///     * Vi is the volume of each trade
///     
///     In this formula, each trade is multiplied by its corresponding volume and then summed together.The resulting sum 
///     is then divided by the total volume of all trades over the given time period to obtain the VWAP. This calculation 
///     takes into account both the price and volume of each trade, providing a more accurate measure of the average price \
///     of a security.
/// </summary>
public class VolumeWeightedAveragePrice
{
    private decimal cumulativePrice = 0m;
    private decimal cumulativeVolume = 0m;
    private List<decimal> prices = new List<decimal>();
    private List<decimal> volumes = new List<decimal>();

    /// <summary>
    /// Calculates the real-time VWAP based on the High, Low, Close, and Volume inputs.
    /// </summary>
    /// <param name="high">The high price of the current period.</param>
    /// <param name="low">The low price of the current period.</param>
    /// <param name="close">The close price of the current period.</param>
    /// <param name="volume">The volume of the current period.</param>
    /// <returns>The real-time VWAP for the current period.</returns>
    public decimal CalculateRealTimeVWAP(decimal high, decimal low, decimal close, decimal volume)
    {
        var typicalPrice = (high + low + close) / 3;
        cumulativePrice += typicalPrice * volume;
        cumulativeVolume += volume;
        return cumulativePrice / cumulativeVolume;
    }

    /// <summary>
    /// Calculates the historical VWAP based on the High, Low, Close, and Volume inputs for the current day.
    /// </summary>
    /// <param name="high">The high price of the current period.</param>
    /// <param name="low">The low price of the current period.</param>
    /// <param name="close">The close price of the current period.</param>
    /// <param name="volume">The volume of the current period.</param>
    /// <returns>The historical VWAP for the current day.</returns>
    public decimal CalculateHistoricalVWAP(decimal high, decimal low, decimal close, decimal volume)
    {
        var typicalPrice = (high + low + close) / 3;
        prices.Add(typicalPrice);
        volumes.Add(volume);

        var cumulativePrice = prices.Select((p, i) => p * volumes[i]).Sum();
        var cumulativeVolume = volumes.Sum();
        return cumulativePrice / cumulativeVolume;
    }

    /// <summary>
    /// Clears out the historical data at the end of the day and starts over.
    /// </summary>
    public void ClearHistoricalData()
    {
        prices.Clear();
        volumes.Clear();
    }
}