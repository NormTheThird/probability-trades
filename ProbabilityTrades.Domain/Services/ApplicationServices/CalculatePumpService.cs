namespace ProbabilityTrades.Domain.Services.ApplicationServices;

public class CalculatePumpService : BaseApplicationService, ICalculatePumpService
{
    public CalculatePumpService(IConfiguration configuration, ApplicationDbContext db) : base(configuration, db) { }

    public async Task<List<CalculatePumpConfigurationModel>> GetCalculatePumpConfigurationsAsync(DataSource dataSource, bool excludeAlreadyPumping)
    {
        // The reason dataSource.ToString() works is because an enum value cannot be directly concatenated with a string in C#.
        // When you pass dataSource directly without calling .ToString(), it is treated as an object and not a string, which leads to a SQL query error.
        var configurations = await _db.Database.SqlQuery<GetCalculatePumpConfigurations_Result>($"EXEC GetCalculatePumpConfigurations {dataSource.ToString()}, {excludeAlreadyPumping}").ToListAsync();
        return configurations
            .Select(_ => new CalculatePumpConfigurationModel
            {
                Id = _.Id,
                DataSource = dataSource,
                BaseCurrency = _.BaseCurrency,
                QuoteCurrency = _.QuoteCurrency,
                CandlestickPattern = (CandlestickPattern)Enum.Parse(typeof(CandlestickPattern), _.CandlestickPattern),
                Period = _.Period,
                ATRMultiplier = _.ATRMultiplier,
                VolumeMultiplier = _.VolumeMultiplier,
                IsActive = _.IsActive,
                IsSendDiscordNotification = _.IsSendDiscordNotification,
                IsCurrentlyPumping = _.IsCurrentlyPumping,
                LastChangedBy = _.LastChangedBy,
                DateLastChanged = _.DateLastChanged,
                DateCreated = _.DateCreated
            })
            .ToList();
    }

    public async Task<List<CalculatePumpOrderModel>> GetOpenPumpOrdersAsync(DataSource dataSource, Guid userId)
    {
        return await _db.CalculatePumpOrders.AsNoTracking()
                                            .Where(_ => _.DataSource.Equals(dataSource.ToString())
                                                     && _.UserId.Equals(userId)
                                                     && _.ClosedTimeUTC == null)
                                            .Select(_ => new CalculatePumpOrderModel
                                            {
                                                Id = _.Id,
                                                DataSource = dataSource,
                                                BaseCurrency = _.BaseCurrency,
                                                QuoteCurrency = _.QuoteCurrency,
                                                OpenedOrderId = _.OpenedOrderId,
                                                OpenedTimeUTC = _.OpenedTimeUTC,
                                                OpenedAmount = _.OpenedAmount,
                                                OpenedMarketPrice = _.OpenedMarketPrice,
                                                StopOrderId = _.StopOrderId,
                                                StopPrice = _.StopPrice,
                                                OrderQuantity = _.OrderQuantity,
                                                ClosedOrderId = _.ClosedOrderId,
                                                ClosedTimeUTC = _.ClosedTimeUTC,
                                                ClosedAmount = _.ClosedAmount,
                                                ClosedMarketPrice = _.ClosedMarketPrice
                                            })
                                            .ToListAsync();
    }

    public async Task<Guid> CreateCalculatePumpStatusAsync(CalculatePumpStatusModel calculatePumpStatusModel)
    {
        // TODO: Trey: 2023.04.13 Do we even care how many time we enter into this table? 
        //var calculatePumpStatus = await _db.CalculatePumpStatuses.FirstOrDefaultAsync(_ => _.DataSource.Equals(calculatePumpStatusModel.DataSource.ToString())
        //                                                                                && _.BaseCurrency.Equals(calculatePumpStatusModel.BaseCurrency)
        //                                                                                && _.QuoteCurrency.Equals(calculatePumpStatusModel.QuoteCurrency)
        //                                                                                && _.CandlestickPattern.Equals(calculatePumpStatusModel.CandlestickPattern.ToString())
        //                                                                                && _.ChartTimeEpoch.Equals(calculatePumpStatusModel.ChartTimeEpoch));
        //if (calculatePumpStatus is not null)
        //    return calculatePumpStatus.Id;

        var calculatePumpStatus = new CalculatePumpStatus
        {
            Id = Guid.NewGuid(),
            DataSource = calculatePumpStatusModel.DataSource.ToString(),
            BaseCurrency = calculatePumpStatusModel.BaseCurrency,
            QuoteCurrency = calculatePumpStatusModel.QuoteCurrency,
            CandlestickPattern = calculatePumpStatusModel.CandlestickPattern.ToString(),
            IsPumping = calculatePumpStatusModel.IsPumping,
            Period = calculatePumpStatusModel.Period,
            ATR = calculatePumpStatusModel.ATR,
            AverageVolume = calculatePumpStatusModel.AverageVolume,
            VolumeTarget = calculatePumpStatusModel.VolumeTarget,
            CurrentCandleVolume = calculatePumpStatusModel.CurrentCandleVolume,
            PriceTarget = calculatePumpStatusModel.PriceTarget,
            CurrentCandlePrice = calculatePumpStatusModel.CurrentCandlePrice,
            DateCreated = DateTime.Now.InCst()
        };

        _db.CalculatePumpStatuses.Add(calculatePumpStatus);
        await _db.SaveChangesAsync();

        return calculatePumpStatus.Id;
    }

    public async Task<bool> CheckIfAssetIsCurrentlyPumpingTodayAsync(DataSource dataSource, string baseCurrency, string quoteCurrency, CandlestickPattern candlestickPattern)
    {
        var nowInCst = DateTime.Now.InCst().Date;
        return await _db.CalculatePumpStatuses.AsNoTracking()
                                              .AnyAsync(_ => _.DataSource.Equals(dataSource.ToString())
                                                          && _.BaseCurrency.Equals(baseCurrency)
                                                          && _.QuoteCurrency.Equals(quoteCurrency)
                                                          && _.CandlestickPattern.Equals(candlestickPattern.ToString())
                                                          && _.DateCreated.Date.Equals(nowInCst)
                                                          && _.IsPumping);
    }

    public async Task<Guid> CreateCalculateOrderAsync(CalculatePumpOrderBaseModel calculatePumpOrderBaseModel, string lastChangedBy)
    {
        var calculatePumpOrder = await _db.CalculatePumpOrders.FirstOrDefaultAsync(_ => _.DataSource.Equals(calculatePumpOrderBaseModel.DataSource.ToString())
                                                                                     && _.OpenedOrderId.Equals(calculatePumpOrderBaseModel.OpenedOrderId));
        if (calculatePumpOrder is not null)
            return calculatePumpOrder.Id;

        var nowInCst = DateTime.Now.InCst();
        calculatePumpOrder = new CalculatePumpOrder
        {
            Id = Guid.NewGuid(),
            UserId = calculatePumpOrderBaseModel.UserId,
            DataSource = calculatePumpOrderBaseModel.DataSource.ToString(),
            BaseCurrency = calculatePumpOrderBaseModel.BaseCurrency,
            QuoteCurrency = calculatePumpOrderBaseModel.QuoteCurrency,
            OpenedOrderId = calculatePumpOrderBaseModel.OpenedOrderId,
            OpenedTimeUTC = calculatePumpOrderBaseModel.OpenedTimeUTC,
            OpenedAmount = calculatePumpOrderBaseModel.OpenedAmount,
            OpenedMarketPrice = calculatePumpOrderBaseModel.OpenedMarketPrice,
            StopOrderId = calculatePumpOrderBaseModel.StopOrderId,
            StopPrice = calculatePumpOrderBaseModel.StopPrice,
            OrderQuantity = calculatePumpOrderBaseModel.OrderQuantity,
            ClosedOrderId = null,
            ClosedTimeUTC = null,
            ClosedAmount = null,
            ClosedMarketPrice = null,
            LastChangedBy = lastChangedBy,
            DateLastChanged = nowInCst,
            ExecutedStop = false,
            DateCreated = nowInCst
        };

        _db.CalculatePumpOrders.Add(calculatePumpOrder);
        await _db.SaveChangesAsync();

        return calculatePumpOrder.Id;
    }

    public async Task UpdateCalculateOrderAsync(CalculatePumpOrderModel calculatePumpOrderModel, string lastChangedBy)
    {
        var calculatePumpOrder = await _db.CalculatePumpOrders.FirstOrDefaultAsync(_ => _.Id.Equals(calculatePumpOrderModel.Id));
        if (calculatePumpOrder is null)
            throw new KeyNotFoundException($"Unable to get calculate pump order for id {calculatePumpOrderModel.Id}");

        calculatePumpOrder.ClosedOrderId = calculatePumpOrderModel.ClosedOrderId;
        calculatePumpOrder.ClosedTimeUTC = calculatePumpOrderModel.ClosedTimeUTC;
        calculatePumpOrder.ClosedAmount = calculatePumpOrderModel.ClosedAmount;
        calculatePumpOrder.ClosedMarketPrice = calculatePumpOrderModel.ClosedMarketPrice;
        calculatePumpOrder.ExecutedStop = calculatePumpOrderModel.ExecutedStop;
        calculatePumpOrder.LastChangedBy = lastChangedBy;
        calculatePumpOrder.DateLastChanged = DateTime.Now.InCst();

        await _db.SaveChangesAsync();
    }

    public async Task<decimal> GetCalculatePumpPercentageTest(CalculatePumpDataModel calculatePumpDataModel)
    {
        //TestVWAP(calculatePumpDataModel);

        return 100.00m;
    }


    private void TestVWAP(List<CalculatePumpDataModel> calculatePumpDataModel)
    {
        var currentRealTimeVWAP = 0.0m;
        var currentHistoricalVWAP = 0.0m;
        var vwap = new VolumeWeightedAveragePrice();
        foreach (var calculatePumpData in calculatePumpDataModel)
        {
            var utcDateTime = calculatePumpData.ChartTimeEpoch.EpochToUniversalDateTime();
            var isMidnight = utcDateTime.TimeOfDay.Equals(TimeSpan.Zero);
            if (isMidnight)
            {
                //Console.WriteLine("Clear");
                vwap.ClearHistoricalData();
            }

            currentRealTimeVWAP = vwap.CalculateRealTimeVWAP(calculatePumpData.HighPrice, calculatePumpData.LowPrice, calculatePumpData.ClosePrice, calculatePumpData.Volume);
            currentRealTimeVWAP = Math.Round(currentRealTimeVWAP, 4);
            //Console.WriteLine($"{utcDateTime} - RealTime: {currentRealTimeVWAP}");

            currentHistoricalVWAP = vwap.CalculateHistoricalVWAP(calculatePumpData.HighPrice, calculatePumpData.LowPrice, calculatePumpData.ClosePrice, calculatePumpData.Volume);
            currentHistoricalVWAP = Math.Round(currentHistoricalVWAP, 4);
            //Console.WriteLine($"{utcDateTime} - Historical: {currentHistoricalVWAP}");
        }

        var lastCloseDate = calculatePumpDataModel.OrderByDescending(_ => _?.ChartTimeEpoch).Select(_ => _.ChartTimeEpoch).FirstOrDefault().EpochToUniversalDateTime();
        var lastClosePrice = Math.Round(calculatePumpDataModel.OrderByDescending(_ => _.ChartTimeEpoch).Select(_ => _.ClosePrice).FirstOrDefault(), 4);
        var trending = currentRealTimeVWAP >= lastClosePrice ? "Up" : "Down";

        Console.WriteLine($"[VWAP 15min] Trending: {trending} | Last Close Date: {lastCloseDate.InCst()} | Last Close Price: {lastClosePrice} | VWAPReal: {currentRealTimeVWAP} | VWAPHist: {currentHistoricalVWAP}\n");
    }





    public decimal TestCalculateADX(List<decimal> high, List<decimal> low, List<decimal> close)
    {
        return AverageDirectionalIndex.CalculateADX(high, low, close, 14);
    }

    public (decimal DIPlus, decimal DIMinus, decimal ADX) TestCalculateADX2(List<decimal> high, List<decimal> low, List<decimal> close)
    {
        var adx2 = new AverageDirectionalIndex2(high, low, close);
        var retval = adx2.RunAverageDirection();
        return (retval.PosDirectionalIndex.LastOrDefault(), retval.NegDirectionalIndex.LastOrDefault(), retval.AvgDirectionIndex.LastOrDefault());
    }

    public (decimal ADX, decimal DIPlus, decimal DIMinus) TestCalculateADXandDX(List<decimal> high, List<decimal> low, List<decimal> close)
    {
        return AverageDirectionalIndex.CalculateADXandDX(high, low, close, 14);
    }

    public decimal CalculateDMI(List<decimal> high, List<decimal> low, List<decimal> close)
    {
        int len = high.Count;
        decimal[] dmiPlus = new decimal[len];
        decimal[] dmiMinus = new decimal[len];
        decimal[] tr = new decimal[len];
        decimal[] diPlus = new decimal[len];
        decimal[] diMinus = new decimal[len];
        decimal[] adx = new decimal[len];

        for (int i = 1; i < len; i++)
        {
            decimal upMove = high[i] - high[i - 1];
            decimal downMove = low[i - 1] - low[i];

            decimal plusDM = upMove > downMove && upMove > 0 ? upMove : 0;
            decimal minusDM = downMove > upMove && downMove > 0 ? downMove : 0;

            tr[i] = Math.Max(Math.Max(high[i] - low[i], Math.Abs(high[i] - close[i - 1])), Math.Abs(low[i] - close[i - 1]));
            diPlus[i] = plusDM > 0 && plusDM > minusDM ? 100 * (plusDM / tr[i]) : 0;
            diMinus[i] = minusDM > 0 && minusDM > plusDM ? 100 * (minusDM / tr[i]) : 0;
        }

        var period = 14;
        for (int i = period; i < len; i++)
        {
            decimal sumDX = 0;

            for (int j = i - period + 1; j <= i; j++)
            {
                sumDX += diPlus[j] - diMinus[j];
            }

            dmiPlus[i] = 100 * (sumDX > 0 ? diPlus[i] / (sumDX / period) : 0);
            dmiMinus[i] = 100 * (sumDX < 0 ? diMinus[i] / (Math.Abs(sumDX) / period) : 0);
            adx[i] = i >= period * 2 - 1 ? 100 * ((dmiPlus[i - period + 1] + dmiMinus[i - period + 1] + (i - period < period ? 0 : adx[i - 1])) / 3) : 0;
        }

        return adx[len - 1];
    }

    public decimal CalculateADX(List<decimal> high, List<decimal> low, List<decimal> close)
    {
        int diLength = 30;
        int adxSmoothing = 14;

        // Calculate the average true range
        decimal atr = CalculateAverageTrueRange(low, high, close, diLength);

        // Calculate the directional movement
        List<decimal> positiveDMList = new List<decimal>();
        List<decimal> negativeDMList = new List<decimal>();
        for (int i = 1; i < close.Count; i++)
        {
            decimal positiveDM = high[i] - high[i - 1];
            decimal negativeDM = low[i - 1] - low[i];
            if (positiveDM > negativeDM && positiveDM > 0)
            {
                positiveDMList.Add(positiveDM);
                negativeDMList.Add(0);
            }
            else if (negativeDM > positiveDM && negativeDM > 0)
            {
                positiveDMList.Add(0);
                negativeDMList.Add(negativeDM);
            }
            else
            {
                positiveDMList.Add(0);
                negativeDMList.Add(0);
            }
        }

        // Calculate the smoothed directional movement
        List<decimal> smoothedPositiveDMList = new List<decimal>();
        List<decimal> smoothedNegativeDMList = new List<decimal>();
        smoothedPositiveDMList.Add(positiveDMList.GetRange(0, adxSmoothing).Average());
        smoothedNegativeDMList.Add(negativeDMList.GetRange(0, adxSmoothing).Average());
        for (int i = adxSmoothing; i < close.Count - 1; i++)
        {
            decimal smoothedPositiveDM = ((adxSmoothing - 1) * smoothedPositiveDMList[i - adxSmoothing] + positiveDMList[i]) / adxSmoothing;
            decimal smoothedNegativeDM = ((adxSmoothing - 1) * smoothedNegativeDMList[i - adxSmoothing] + negativeDMList[i]) / adxSmoothing;
            smoothedPositiveDMList.Add(smoothedPositiveDM);
            smoothedNegativeDMList.Add(smoothedNegativeDM);
        }

        // Calculate the Directional Index (DI)
        List<decimal> positiveDIList = new List<decimal>();
        List<decimal> negativeDIList = new List<decimal>();
        for (int i = adxSmoothing; i < close.Count; i++)
        {
            decimal positiveDI = 100 * (smoothedPositiveDMList[i - adxSmoothing] / atr);
            decimal negativeDI = 100 * (smoothedNegativeDMList[i - adxSmoothing] / atr);
            positiveDIList.Add(positiveDI);
            negativeDIList.Add(negativeDI);
        }

        // Calculate the ADX
        List<decimal> dxList = new List<decimal>();
        for (int i = adxSmoothing; i < positiveDIList.Count; i++)
        {
            decimal dx = 100 * (Math.Abs(positiveDIList[i] - negativeDIList[i]) / (positiveDIList[i] + negativeDIList[i]));
            dxList.Add(dx);
        }

        // Calculate the smoothed ADX
        decimal adx = dxList.GetRange(0, adxSmoothing).Average();
        for (int i = adxSmoothing; i < dxList.Count; i++)
        {
            adx = ((adxSmoothing - 1) * adx + dxList[i]) / adxSmoothing;
        }

        return adx;
    }

    private static List<decimal> SmoothList(List<decimal> inputList, int smoothing)
    {
        List<decimal> smoothedList = new List<decimal>();

        for (int i = smoothing - 1; i < inputList.Count; i++)
        {
            decimal sum = 0;
            for (int j = i - smoothing + 1; j <= i; j++)
            {
                sum += inputList[j];
            }
            decimal smoothedValue = sum / smoothing;
            smoothedList.Add(smoothedValue);
        }

        return smoothedList;
    }

    public List<decimal> DirectionalMovementIndex(List<decimal> high, List<decimal> low, List<decimal> close, int length = 14, int adxSmoothing = 14)
    {
        if (high.Count != low.Count || high.Count != close.Count)
        {
            throw new ArgumentException("The length of the high, low, and close lists must be the same.");
        }

        var plusDM = new List<decimal>();
        var minusDM = new List<decimal>();
        var trur = new List<decimal>();
        var plus = new List<decimal>();
        var minus = new List<decimal>();
        var sum = new List<decimal>();
        var adx = new List<decimal>();

        for (int i = 0; i < close.Count; i++)
        {
            decimal up = i == 0 ? 0m : high[i] - high[i - 1];
            decimal down = i == 0 ? 0m : -(low[i] - low[i - 1]);
            decimal plusDMi = up > down && up > 0 ? up : 0;
            decimal minusDMi = down > up && down > 0 ? down : 0;
            plusDM.Add(plusDMi);
            minusDM.Add(minusDMi);

            decimal trurI = i == 0 ? high[i] - low[i] : Math.Max(high[i] - low[i], Math.Max(Math.Abs(high[i] - close[i - 1]), Math.Abs(low[i] - close[i - 1])));
            trur.Add(trurI);

            int len = Math.Min(i + 1, length);
            decimal plusI = 100 * SMA(plusDM, len) / SMA(trur, len);
            decimal minusI = 100 * SMA(minusDM, len) / SMA(trur, len);
            plus.Add(plusI);
            minus.Add(minusI);

            decimal sumI = plusI + minusI;
            sum.Add(sumI);

            decimal adxI = i < adxSmoothing - 1 ? 0 : adx.Last();
            decimal adxCurrent = 100 * Math.Abs(plusI - minusI) / (sumI == 0 ? 1 : sumI);
            adx.Add((adxI * (adxSmoothing - 1) + adxCurrent) / adxSmoothing);
        }

        return adx;
    }

    private static decimal SMA(List<decimal> values, int length)
    {
        if (values.Count < length)
        {
            length = values.Count;
        }
        return values.Skip(values.Count - length).Average();
    }


    private static decimal CalculateAverageTrueRange(List<decimal> high, List<decimal> low, List<decimal> close, int period)
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

}