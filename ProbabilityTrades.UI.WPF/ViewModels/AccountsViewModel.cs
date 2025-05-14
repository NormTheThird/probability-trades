namespace ProbabilityTrades.UI.WPF.ViewModels;

public class AccountsViewModel : BaseViewModel
{
    private readonly IExchangeApiService _krakenApiService;
    private readonly IUserExchangeService _userExchangeService;

    public AccountsViewModel(UserSingleton userSingleton, IExchangeApiService krakenApiService, IUserExchangeService userExchangeService)
        : base(userSingleton)
    {
        _krakenApiService = krakenApiService ?? throw new ArgumentNullException(nameof(krakenApiService));
        _userExchangeService = userExchangeService ?? throw new ArgumentNullException(nameof(userExchangeService));
    }

    public async override Task LoadAsync()
    {
        KrakenAccountDetails = new AccountDetailModel { StatusImageSource = $"../../Assets/icons8-toggle-off-64.png" };
        await KrakenUpdateAsync();
    }

    #region Properties

    private AccountDetailModel _krakenAccountDetails;
    public AccountDetailModel KrakenAccountDetails
    {
        get => _krakenAccountDetails;
        set
        {
            _krakenAccountDetails = value;
            OnPropertyChanged(nameof(KrakenAccountDetails));
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

    public ICommand KrakenUpdateCommand => new RelayCommandAsync(KrakenUpdateAsync);
    private async Task KrakenUpdateAsync()
    {
        try
        {
            KrakenErrorMessage = "";
            KrakenAccountDetails.IsLoading = true;
            KrakenAccountDetails.AccountBalanceDetails = (await GetKrakenAccountBalanceDetailsAsync()).OrderByDescending(_ => _.EstimatedValueUSD);
            KrakenAccountDetails.TotalValueUSD = KrakenAccountDetails.AccountBalanceDetails.Sum(x => x.EstimatedValueUSD);
            KrakenAccountDetails.IsConnected = true;
            KrakenAccountDetails.StatusImageSource = $"../../Assets/icons8-toggle-on-64.png";
        }
        catch (Exception ex)
        {
            KrakenAccountDetails.IsConnected = false;
            KrakenAccountDetails.StatusImageSource = $"../../Assets/icons8-toggle-off-64.png";
            KrakenErrorMessage = $"Error: Could not update Kraken \n{ex.GetFullMessage()}";

        }
        finally
        {
            KrakenAccountDetails.IsLoading = false;
            KrakenAccountDetails.LastUpdatedMessage = $"Last updated: {DateTime.Now}";
        }
    }

    #endregion

    private async Task<IEnumerable<AccountBalanceDetailModel>> GetKrakenAccountBalanceDetailsAsync()
    {
        var exchangeTokenModel = await GetKrakenExchangeTokenModelAsync();
        return await _krakenApiService.GetAccountBalanceDetailsAsync(exchangeTokenModel);
    }

    private async Task<ExchangeTokenModel> GetKrakenExchangeTokenModelAsync()
    {
        var apiKeys = await _userExchangeService.GetUserExchangesAsync(_userSingleton.LoggedInUser.Id);
        var krakenApiKeys = apiKeys.FirstOrDefault(_ => _.Name.Equals("Kraken"));

        return new ExchangeTokenModel
        {
            ApiKey = krakenApiKeys?.ApiKey ?? string.Empty,
            ApiSecret = krakenApiKeys?.ApiSecret ?? string.Empty
        };
    }
}