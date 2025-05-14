namespace ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

public interface ICalculatePumpService
{
    Task<List<CalculatePumpConfigurationModel>> GetCalculatePumpConfigurationsAsync(DataSource dataSource, bool excludeAlreadyPumping);
    Task<List<CalculatePumpOrderModel>> GetOpenPumpOrdersAsync(DataSource dataSource, Guid userId);
    Task<Guid> CreateCalculatePumpStatusAsync(CalculatePumpStatusModel calculatePumpStatusModel);
    Task<bool> CheckIfAssetIsCurrentlyPumpingTodayAsync(DataSource dataSource, string baseCurrency, string quoteCurrency, CandlestickPattern candlestickPattern);
    Task<Guid> CreateCalculateOrderAsync(CalculatePumpOrderBaseModel calculatePumpOrderBaseModel, string lastChangedBy);
    Task UpdateCalculateOrderAsync(CalculatePumpOrderModel calculatePumpOrderModel, string lastChangedBy);
    Task<decimal> GetCalculatePumpPercentageTest(CalculatePumpDataModel calculatePumpDataModel);





    decimal TestCalculateADX(List<decimal> high, List<decimal> low, List<decimal> close);
    (decimal DIPlus, decimal DIMinus, decimal ADX) TestCalculateADX2(List<decimal> high, List<decimal> low, List<decimal> close);
    public (decimal ADX, decimal DIPlus, decimal DIMinus) TestCalculateADXandDX(List<decimal> high, List<decimal> low, List<decimal> close);
    decimal CalculateDMI(List<decimal> high, List<decimal> low, List<decimal> close);
    decimal CalculateADX(List<decimal> highs, List<decimal> lows, List<decimal> closes);
    List<decimal> DirectionalMovementIndex(List<decimal> high, List<decimal> low, List<decimal> close, int length = 14, int adxSmoothing = 14);
}