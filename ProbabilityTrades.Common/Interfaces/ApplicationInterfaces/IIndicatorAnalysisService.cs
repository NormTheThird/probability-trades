namespace ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

public interface IIndicatorAnalysisService
{
    Task<MovingAveragePositionModel> CalculateMovingAveragePositionAsync(BaseDataModel baseDataModel, int shortMovingAverageDays, int longMovingAverageDays);
    Task<CalculatePumpStatusBaseModel> CalculatePumpStatusAsync(CalculatePumpModel calculatePumpModel);
}