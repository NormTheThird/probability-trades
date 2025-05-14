namespace ProbabilityTrades.UI.WPF;

public partial class App : Application
{
    public static IHost AppHost { get; private set; }
    private readonly NavigationStore _navigationStore;

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
            {
                configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
                configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((hostBuilderContext, services) =>
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer("ApplicationDatabaseSqlServer");
                });

                services.AddDbContext<CurrencyHistoryDbContext>(options =>
                {
                    options.UseSqlServer("CurrencyHistoryDatabaseSqlServer");
                });

                services.AddSingleton(provider => new MainWindow
                {
                    DataContext = provider.GetRequiredService<MainViewModel>()
                });

                services.AddSingleton<UserSingleton>();
                services.AddSingleton<NavigationStore>();
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<LoginViewModel>();
                services.AddSingleton<DashboardViewModel>();
                services.AddSingleton<AccountsViewModel>();
                services.AddSingleton<SettingsViewModel>();
                services.AddSingleton<IExchangeApiService, KrakenApiService>();
                services.AddSingleton<ISecurityService, SecurityService>();
                services.AddSingleton<IUserExchangeService, UserExchangeService>();
                services.AddSingleton<Func<Type, BaseViewModel>>(serviceProvider =>
                    viewModelType => (BaseViewModel)serviceProvider.GetRequiredService(viewModelType));
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost.StartAsync();

        MainWindow = AppHost.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();
        base.OnExit(e);
    }
}