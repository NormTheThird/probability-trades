namespace ProbabilityTrades.UI.WPF.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly IUserExchangeService _userExchangeService;

    public SettingsViewModel(UserSingleton userSingleton, IUserExchangeService userExchangeService)
        : base(userSingleton)
    {
        _userExchangeService = userExchangeService ?? throw new ArgumentNullException(nameof(userExchangeService));
    }

    public async override Task LoadAsync()
    {
        KrakenAccountKeys = new AccountKeyModel();
        await GetKrakenApiKeysAsync();
    }

    #region Properties

    private AccountKeyModel _krakenAccountKeys;
    public AccountKeyModel KrakenAccountKeys
    {
        get => _krakenAccountKeys;
        set
        {
            _krakenAccountKeys = value;
            OnPropertyChanged(nameof(KrakenAccountKeys));
        }
    }

    private string _krakenErrorMessage;
    public string KrakenErrorMessage
    {
        get => _krakenErrorMessage;
        set
        {
            _krakenErrorMessage = value;
            OnPropertyChanged(nameof(KrakenErrorMessage));
        }
    }

    #endregion

    #region Commands

    public ICommand SaveKrakenApiKeysCommand => new RelayCommandAsync(SaveKrakenApiKeysAsync);
    private async Task SaveKrakenApiKeysAsync()
    {
        try
        {
            KrakenErrorMessage = "";
            var krakenExchange = _userSingleton.Exchanges.FirstOrDefault(_ => _.Name.Equals("Kraken"));
            if (krakenExchange is null)
            {
                krakenExchange = new ExchangeModel
                {
                    UserId = _userSingleton.LoggedInUser.Id,
                    Name = "Kraken",
                    ApiKey = KrakenAccountKeys.ApiKey,
                    ApiSecret = KrakenAccountKeys.PrivateKey,
                    DateCreated = DateTime.Today
                };

                krakenExchange.Id = await _userExchangeService.CreateUserExchangeAsync(krakenExchange);

                _userSingleton.Exchanges.ToList().Add(krakenExchange);
            }
            else
            {
                krakenExchange.ApiKey = KrakenAccountKeys.ApiKey;
                krakenExchange.ApiSecret = KrakenAccountKeys.PrivateKey;

                await _userExchangeService.UpdateUserExchangeAsync(krakenExchange);
            }
        }
        catch (Exception ex)
        {
            KrakenErrorMessage = $"Error: Could not save Kraken API keys \n{ex.GetFullMessage()}";
        }
    }

    #endregion

    private async Task GetKrakenApiKeysAsync()
    {
        try
        {
            KrakenErrorMessage = "";
            var apiKeys = await _userExchangeService.GetUserExchangesAsync(_userSingleton.LoggedInUser.Id);
            var krakenApiKeys = apiKeys.FirstOrDefault(_ => _.Name.Equals("Kraken"));
            // ApiKey: Tz8xKcL9FhSu/Ue1Ft1t4ZJBMg0qc1nMK1KQgsG+PN7wAtYW4XrWAP1N
            KrakenAccountKeys.ApiKey = krakenApiKeys?.ApiKey ?? string.Empty;
            // PrivateKey: F4lzPu1aDyi3IzSajtNSRr4sk1hJd+XLQuWW6nfq6F7v217sxihnnt68N4EJWvhcK9QSgb+uYECZQWE1nsccDA==
            KrakenAccountKeys.PrivateKey = krakenApiKeys?.ApiSecret ?? string.Empty;
        }
        catch (Exception ex)
        {
            KrakenErrorMessage = $"Error: Could not get Kraken API keys \n{ex.GetFullMessage()}";
        }
    }
}