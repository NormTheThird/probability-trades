namespace ProbabilityTrades.UI.WPF.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly NavigationStore _navigationStore;
    private readonly LoginViewModel _loginViewModel;
    private readonly DashboardViewModel _dashboardViewModel;
    private readonly AccountsViewModel _accountsViewModel;
    private readonly SettingsViewModel _settingsViewModel;

    public BaseViewModel CurrentViewModel => _navigationStore.CurrentViewModel;

    public MainViewModel(UserSingleton userSingleton, NavigationStore navigationStore, LoginViewModel loginViewModel, 
                         DashboardViewModel dashboardViewModel, AccountsViewModel accountsViewModel, SettingsViewModel settingsViewModel)
        : base(userSingleton)
    {
        _navigationStore = navigationStore ?? throw new ArgumentNullException(nameof(navigationStore));
        _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        _loginViewModel = loginViewModel ?? throw new ArgumentNullException(nameof(loginViewModel));
        _dashboardViewModel = dashboardViewModel ?? throw new ArgumentNullException(nameof(dashboardViewModel));
        _accountsViewModel = accountsViewModel ?? throw new ArgumentNullException(nameof(accountsViewModel));
        _settingsViewModel = settingsViewModel ?? throw new ArgumentNullException(nameof(settingsViewModel));

        _navigationStore.CurrentViewModel = _loginViewModel;

        ShowLoginViewCommand = new NavigateCommand<LoginViewModel>(new NavigationService<LoginViewModel>(navigationStore, () => _loginViewModel));
        ShowDashboardViewCommand = new NavigateCommand<DashboardViewModel>(new NavigationService<DashboardViewModel>(navigationStore, () => _dashboardViewModel));
        ShowAccountsViewCommand = new NavigateCommand<AccountsViewModel>(new NavigationService<AccountsViewModel>(navigationStore, () => _accountsViewModel));
        ShowSettingsViewCommand = new NavigateCommand<SettingsViewModel>(new NavigationService<SettingsViewModel>(navigationStore, () => _settingsViewModel));
        CloseApplicationCommand = new CloseApplicationCommand();
        MaximizeApplicationCommand = new MaximizeApplicationCommand();
        MinimizeApplicationCommand = new MinimizeApplicationCommand();
    }

    #region Properties

    private bool _isAccountsSelected;
    public bool IsAccountsSelected
    {
        get => _isAccountsSelected;
        set
        {
            _isAccountsSelected = value;
            OnPropertyChanged(nameof(IsAccountsSelected));
        }
    }

    private bool _isDashboardSelected;
    public bool IsDashboardSelected
    {
        get => _isDashboardSelected;
        set
        {
            _isDashboardSelected = value;
            OnPropertyChanged(nameof(IsDashboardSelected));
        }
    }

    private UserAuthenticationModel _loggedInUser;
    public UserAuthenticationModel LoggedInUser
    {
        get => _loggedInUser;
        set
        {
            _loggedInUser = value;
            OnPropertyChanged(nameof(LoggedInUser));
        }
    }

    private bool _isLoginViewActive;
    public bool IsLoginViewActive
    {
        get => _isLoginViewActive;
        set
        {
            _isLoginViewActive = value;
            OnPropertyChanged(nameof(IsLoginViewActive));
        }
    }

    private bool _isSettingsSelected;
    public bool IsSettingsSelected
    {
        get => _isSettingsSelected;
        set
        {
            _isSettingsSelected = value;
            OnPropertyChanged(nameof(IsSettingsSelected));
        }
    }

    #endregion

    #region Commands

    public ICommand ShowLoginViewCommand { get; }

    public ICommand ShowDashboardViewCommand { get; }

    public ICommand ShowAccountsViewCommand { get; }

    public ICommand ShowSettingsViewCommand { get; }

    public ICommand CloseApplicationCommand { get; }

    public ICommand MaximizeApplicationCommand { get; }

    public ICommand MinimizeApplicationCommand { get; }

    #endregion


    private void OnCurrentViewModelChanged()
    {
        IsAccountsSelected = CurrentViewModel is AccountsViewModel;
        IsDashboardSelected = CurrentViewModel is DashboardViewModel;
        IsLoginViewActive = CurrentViewModel is LoginViewModel;
        IsSettingsSelected = CurrentViewModel is SettingsViewModel;

        if (LoggedInUser is null)
            LoggedInUser = _userSingleton.LoggedInUser;

        OnPropertyChanged(nameof(CurrentViewModel));
    }
}