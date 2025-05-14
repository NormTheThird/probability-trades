namespace ProbabilityTrades.Domain.Services.ApplicationServices;

public class UserExchangeService : BaseApplicationService, IUserExchangeService
{
    public UserExchangeService(IConfiguration configuration, ApplicationDbContext db) : base(configuration, db) { }

    public async Task<List<ExchangeModel>> GetUserExchangesAsync(Guid userId)
    {
        var userExchanges = await _db.UserExchanges.AsNoTracking()
                                                   .Where(_ => _.UserId.Equals(userId))
                                                   .Select(_ => new ExchangeModel
                                                   {
                                                       Id = _.Id,
                                                       UserId = userId,
                                                       Name = _.Name,
                                                       ApiKey = _.ApiKey,
                                                       ApiPassphrase = _.ApiPassphrase,
                                                       ApiSecret = _.ApiSecret,
                                                       DateCreated = _.DateCreated
                                                   })
                                                   .ToListAsync();

        foreach (var exchange in userExchanges)
        {

            exchange.ApiKey = DecryptString(exchange.ApiKey, exchange.UserId.ToString());
            exchange.ApiPassphrase = DecryptString(exchange.ApiPassphrase, exchange.UserId.ToString());
            exchange.ApiSecret = DecryptString(exchange.ApiSecret, exchange.UserId.ToString());
        }

        return userExchanges;
    }

    public async Task<ExchangeTokenModel> GetUserExchangeTokensAsync(Guid userId, string exchange)
    {
        var userExchange = await _db.UserExchanges.AsNoTracking().FirstOrDefaultAsync(_ => _.UserId.Equals(userId) && _.Name.Equals(exchange));
        if (userExchange == null)
            return null;

        return new ExchangeTokenModel
        {
            ApiKey = DecryptString(userExchange.ApiKey, userId.ToString()),
            ApiPassphrase = DecryptString(userExchange.ApiPassphrase, userId.ToString()),
            ApiSecret = DecryptString(userExchange.ApiSecret, userId.ToString()),
        };
    }

    public async Task<Guid> CreateUserExchangeAsync(ExchangeModel userExchangeModel)
    {

        var exchange = await _db.UserExchanges.FirstOrDefaultAsync(_ => _.Name.Equals(userExchangeModel.Name) && _.UserId.Equals(userExchangeModel.UserId));
        if (exchange != null)
            //TODO: TREY: 2022.08.18 Think about implementing this or somehting similar. throw new PrincipalExistsException() 
            throw new ApplicationException($"User exchange already exists for {userExchangeModel.Name}");

        var encryptedApiKey = EncryptString(userExchangeModel.ApiKey, userExchangeModel.UserId.ToString());
        var encryptedApiSecret = EncryptString(userExchangeModel.ApiSecret, userExchangeModel.UserId.ToString());
        var encryptedApiPassphrase = EncryptString(userExchangeModel.ApiPassphrase, userExchangeModel.UserId.ToString()) ?? "";
 

        exchange = new UserExchange
        {
            Id = Guid.NewGuid(),
            UserId = userExchangeModel.UserId,
            Name = userExchangeModel.Name,
            ApiKey = encryptedApiKey,
            ApiPassphrase = encryptedApiPassphrase,
            ApiSecret = encryptedApiSecret,
            DateCreated = DateTime.Today.InCst()
        };
        _db.UserExchanges.Add(exchange);
        await _db.SaveChangesAsync();

        return exchange.Id;
    }

    public async Task UpdateUserExchangeAsync(ExchangeModel userExchangeModel)
    {
        var exchange = await _db.UserExchanges.FirstOrDefaultAsync(_ => _.Id.Equals(userExchangeModel.Id));
        if (exchange == null)
            throw new KeyNotFoundException($"Unable to get exchange for id {userExchangeModel.Id}");

        exchange.ApiKey = EncryptString(userExchangeModel.ApiKey, userExchangeModel.UserId.ToString());
        exchange.ApiPassphrase = EncryptString(userExchangeModel.ApiPassphrase, userExchangeModel.UserId.ToString()) ?? "";
        exchange.ApiSecret = EncryptString(userExchangeModel.ApiSecret, userExchangeModel.UserId.ToString());
        await _db.SaveChangesAsync();
    }
}