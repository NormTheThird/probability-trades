namespace ProbabilityTrades.Domain.Services.ApplicationServices;

public class IndicatorAnalysisService : BaseApplicationService, IIndicatorAnalysisService
{
    private readonly ICurrencyHistoryService _currencyHistoryService;

    public IndicatorAnalysisService(IConfiguration configuration, ILogger<IndicatorAnalysisService> logger, ApplicationDbContext db, 
                                    ICurrencyHistoryService currencyHistoryService)
        : base(configuration, db)
    {
        _currencyHistoryService = currencyHistoryService ?? throw new ArgumentNullException(nameof(currencyHistoryService));
    }

    /// <summary>
    ///     Calculate the moving average position for the given current history record and moving average days
    /// </summary>
    /// <param name="baseDataModel"></param>
    /// <param name="shortMovingAverageDays"></param>
    /// <param name="longMovingAverageDays"></param>
    /// <returns>MovingAveragePositionModel</returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<MovingAveragePositionModel> CalculateMovingAveragePositionAsync(BaseDataModel baseDataModel, int shortMovingAverageDays, int longMovingAverageDays)
    {
        var movingAverageStatus = new MovingAveragePositionModel();
        var currentRecord = await _currencyHistoryService.GetCurrentHistoryRecordAsync(baseDataModel.BaseCurrency, baseDataModel.QuoteCurrency, baseDataModel.CandlestickPattern)
            // Todo: Trey: 2023.07.24 Should this return a movingAverageStatus with market action of unknown or throw an exception?
            // ?? return movingAverageStatus;
            ?? throw new KeyNotFoundException($"Unable to get current history record for {baseDataModel.BaseCurrency}-{baseDataModel.QuoteCurrency} {baseDataModel.CandlestickPattern}");

        var lastClosePrice = currentRecord.ClosingPrice;
        movingAverageStatus.ShortMovingAverage = await CalculateMovingAverageAsync(baseDataModel, shortMovingAverageDays);
        movingAverageStatus.LongMovingAverage = await CalculateMovingAverageAsync(baseDataModel, longMovingAverageDays);

        // Long Position: short (fast) moving average crosses above the long (slow) moving average
        if (movingAverageStatus.ShortMovingAverage > movingAverageStatus.LongMovingAverage)
            movingAverageStatus.MarketPosition = lastClosePrice > movingAverageStatus.LongMovingAverage ? MarketPosition.Long : MarketPosition.Cash;

        // Short Position: short (fast) moving average crosses below the long (slow) moving average
        if (movingAverageStatus.ShortMovingAverage < movingAverageStatus.LongMovingAverage)
            movingAverageStatus.MarketPosition = lastClosePrice < movingAverageStatus.LongMovingAverage ? MarketPosition.Short : MarketPosition.Cash;

        return movingAverageStatus;
    }

    /// <summary>
    ///     Calculate the moving average for the given history records and moving average days
    /// </summary>
    /// <param name="baseDataModel"></param>m>
    /// <param name="movingAverageDays"></param>
    /// <returns>decimal</returns>
    private async Task<decimal> CalculateMovingAverageAsync(BaseDataModel baseDataModel, int movingAverageDays)
    {
        var historyRecords = await _currencyHistoryService.GetHistoryRecordsByIntervalsBackAsync(baseDataModel.BaseCurrency, baseDataModel.QuoteCurrency, baseDataModel.CandlestickPattern, movingAverageDays);
        return historyRecords.Average(_ => _.ClosingPrice);
    }






    public async Task<CalculatePumpStatusBaseModel> CalculatePumpStatusAsync(CalculatePumpModel calculatePumpModel)
    {

        //var averageVolume = await _currencyHistoryService.GetDailyVolumeAverageForDaysBack(calculatePumpModel.BaseCurrency, calculatePumpModel.QuoteCurrency, calculatePumpModel.Period);
        //var volumeTarget = averageVolume * calculatePumpModel.VolumeMultiplier;

        //var lastDailyHistoryRecord = await _currencyHistoryService.GetCurrentHistoryRecordAsync(calculatePumpModel.BaseCurrency, calculatePumpModel.QuoteCurrency, calculatePumpModel.CandlestickPattern);
        //var historyRecords = await _currencyHistoryService.GetHistoryRecordsByIntervalsBackAsync(calculatePumpModel.BaseCurrency, calculatePumpModel.QuoteCurrency, calculatePumpModel.CandlestickPattern, calculatePumpModel.Period);
        //var atr = CalculateATR(historyRecords, calculatePumpModel.Period);
        //var priceTarget = lastDailyHistoryRecord.ClosingPrice + (calculatePumpModel.ATRMultiplier * atr);

        //var isPumping = calculatePumpModel.CurrentCandleVolume > volumeTarget && calculatePumpModel.CurrentCandlePrice > priceTarget;

        //return new CalculatePumpStatusBaseModel
        //{
        //    IsPumping = isPumping,
        //    ATR = atr,
        //    AverageVolume = averageVolume,
        //    VolumeTarget = volumeTarget,
        //    CurrentCandleVolume = calculatePumpModel.CurrentCandleVolume,
        //    PriceTarget = priceTarget,
        //    CurrentCandlePrice = calculatePumpModel.CurrentCandlePrice,
        //    Period = calculatePumpModel.Period,
        //};

        throw new NotImplementedException();
    }

    // Created from ChatGPT
    public static decimal CalculateATR(List<CurrencyHistoryModel> historyRecords, int period)
    {
        var highs = historyRecords.Select(_ => _.HighestPrice).ToList();
        var lows = historyRecords.Select(_ => _.LowestPrice).ToList();
        var closes = historyRecords.Select(_ => _.ClosingPrice).ToList();

        if (highs.Count != lows.Count || highs.Count != closes.Count)
            throw new ArgumentException("Input lists (highs, lows, closes) must have the same count.");

        decimal atr = 0.0m;
        for (var i = 0; i < period; i++)
        {
            var trueRange = 0.0m;
            if (i == 0)
            {
                trueRange = highs[i] - lows[i];
            }
            else
            {
                var highMinusLow = highs[i] - lows[i];
                var highMinusPrevClose = Math.Abs(highs[i] - closes[i - 1]);
                var lowMinusPrevClose = Math.Abs(lows[i] - closes[i - 1]);
                trueRange = Math.Max(highMinusLow, Math.Max(highMinusPrevClose, lowMinusPrevClose));
            }
            atr += trueRange;
        }

        atr /= period;

        for (var i = period; i < highs.Count; i++)
        {
            var trueRange = 0.0m;
            var highMinusLow = highs[i] - lows[i];
            var highMinusPrevClose = Math.Abs(highs[i] - closes[i - 1]);
            var lowMinusPrevClose = Math.Abs(lows[i] - closes[i - 1]);
            trueRange = Math.Max(highMinusLow, Math.Max(highMinusPrevClose, lowMinusPrevClose));
            atr = ((period - 1) * atr + trueRange) / period;
        }

        return atr;
    }
}