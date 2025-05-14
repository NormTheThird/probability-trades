namespace ProbabilityTrades.Common.Interfaces.ApplicationInterfaces;

public interface IUserExchangeService
{
    Task<List<ExchangeModel>> GetUserExchangesAsync(Guid userId);
    Task<ExchangeTokenModel> GetUserExchangeTokensAsync(Guid userId, string exchange);
    Task<Guid> CreateUserExchangeAsync(ExchangeModel userExchangeModel);
    Task UpdateUserExchangeAsync(ExchangeModel userExchangeModel);
}