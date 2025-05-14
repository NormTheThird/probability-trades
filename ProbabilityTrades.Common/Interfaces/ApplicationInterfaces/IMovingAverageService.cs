namespace ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

public interface IMovingAverageService
{
    Task<List<MovingAverageConfigurationModel>> GetMovingAverageConfigurationsAsync(DataSource dataSource);
    Task<MovingAverageConfigurationModel> GetMovingAverageConfigurationAsync(Guid movingAverageConfigurationId);
    Task<Guid> CreateMovingAverageConfigurationAsync(MovingAverageConfigurationModel maConfigurationModel, string lastChangedBy);
    Task UpdateMovingAverageConfigurationAsync(MovingAverageConfigurationModel maConfigurationModel, string lastChangedBy);
    Task DeleteMovingAverageConfigurationAsync(Guid maConfigurationId);
    Task<List<MovingAverageStatusModel>> GetMovingAverageStatusesAsync(BaseDataModel baseDataMode, int numberOfPositions);
    Task<List<MovingAveragePositionStatusModel>> GetMovingAveragePositionStatusesAsync(BaseDataModel baseDataModel, int numberOfPositions = 0);
    Task<List<MovingAverageStatusModel>> GetMovingAverageStatusPositionChangesByDateAsync(DataSource dataSource, DateTime dateTime);
    Task<IEnumerable<MovingAverageCurrentPositionModel>> GetCurrentMovingAveragePositionsAsync(DataSource dataSource);
    Task<Guid> CreateMovingAverageStatusAsync(MovingAverageStatusModel movingAverageStatusModel);
}