namespace ProbabilityTrades.UI.WPF.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly NavigationStore _navigationStore;
    private readonly ISecurityService _securityService;
    private readonly IUserExchangeService _userExchangeService;

    public LoginViewModel(UserSingleton userSingleton, NavigationStore navigationStore, ISecurityService securityService,
                          IUserExchangeService userExchangeService)
         : base(userSingleton)
    {
        _navigationStore = navigationStore ?? throw new ArgumentNullException(nameof(navigationStore));
        _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
        _userExchangeService = userExchangeService ?? throw new ArgumentNullException(nameof(userExchangeService));

        Username = "normthethird";
        Password = "password";
    }

    private string _username;
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    private string _password;
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
        }
    }

    private string _errorMessage;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }

    public ICommand LoginCommand => new RelayCommandAsync(LoginAsync);
    private async Task LoginAsync()
    {
        try
        {
            ErrorMessage = string.Empty;

            if (Username is null)
                throw new Exception("Username is required.");
            else if (Password is null)        
                throw new Exception("Password is required.");
            else
            {
                var authenticatedUser = await _securityService.AuthenticateUserAsync(Username, Password);
                if (authenticatedUser is null)
                    throw new Exception("Invalid username or password.");

                _userSingleton.LoggedInUser = authenticatedUser;
                _userSingleton.Exchanges = await GetUserExchanges(authenticatedUser.Id);

                var dashboardNavigationService = new NavigationService<DashboardViewModel>(_navigationStore, () => new DashboardViewModel(_userSingleton));
                dashboardNavigationService.Navigate();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    public ICommand ForgotPasswordCommand { get => throw new NotImplementedException(); }

    public ICommand ResetPasswordCommand { get => throw new NotImplementedException(); }


    private async Task<IEnumerable<ExchangeModel>> GetUserExchanges(Guid userId)
    {
        return await _userExchangeService.GetUserExchangesAsync(userId);
    }
}