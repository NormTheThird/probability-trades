namespace ProbabilityTrades.UI.WPF.Models;

public class AccountKeyModel : BaseModel
{
    private string _apiKey;
    public string ApiKey
    {
        get => _apiKey;
        set
        {
            _apiKey = value;
            OnPropertyChanged(nameof(ApiKey));
        }
    }

    private string _privateKey;
    public string PrivateKey
    {
        get => _privateKey;
        set
        {
            _privateKey = value;
            OnPropertyChanged(nameof(PrivateKey));
        }
    }
}

public class AccountDetailModel : BaseModel
{
    private decimal _TotalValueUSD;
    public decimal TotalValueUSD
    {
        get => _TotalValueUSD;
        set
        {
            _TotalValueUSD = value;
            OnPropertyChanged(nameof(TotalValueUSD));
        }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged(nameof(IsLoading));
        }
    }

    private bool _isConnected;
    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            _isConnected = value;
            OnPropertyChanged(nameof(IsConnected));
        }
    }

    private string _statusImageSource;
    public string StatusImageSource
    {
        get => _statusImageSource;
        set
        {
            _statusImageSource = value;
            OnPropertyChanged(nameof(StatusImageSource));
        }
    }

    private string _lastUpdatedMessage;
    public string LastUpdatedMessage
    {
        get => _lastUpdatedMessage;
        set
        {
            _lastUpdatedMessage = value;
            OnPropertyChanged(nameof(LastUpdatedMessage));
        }
    }

    private IEnumerable<AccountBalanceDetailModel> _accountBalanceDetails;
    public IEnumerable<AccountBalanceDetailModel> AccountBalanceDetails
    {
        get => _accountBalanceDetails;
        set
        {
            _accountBalanceDetails = value;
            OnPropertyChanged(nameof(AccountBalanceDetails));
        }
    }
}