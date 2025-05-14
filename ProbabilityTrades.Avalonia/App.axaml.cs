using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProbabilityTrades.Data.SqlServer.DataAccess;

namespace ProbabilityTrades.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var locator = new ViewLocator();
        DataTemplates.Add(locator);

        var services = new ServiceCollection();

        // Setup configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        ConfigureDatabase(services, configuration);
        ConfigureServices(services);
        ConfigureViewModels(services);
        ConfigureViews(services);
        //services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

        // Typed-clients
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0#typed-clients
        //services.AddHttpClient<ILoginService, LoginService>(httpClient => httpClient.BaseAddress = new Uri("https://dummyjson.com/"));

        var provider = services.BuildServiceProvider();

        Ioc.Default.ConfigureServices(provider);

        var vm = Ioc.Default.GetRequiredService<MainViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Create and configure LoginWindow
            var loginViewModel = Ioc.Default.GetRequiredService<LoginViewModel>();
            var loginWindow = new LoginWindow
            {
                DataContext = loginViewModel
            };

            // Create and configure MainWindow
            var mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();
            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            // Handle LoginWindow closed event to exit application
            //loginWindow.Closed += (s, e) => desktop.Shutdown();

            // Set the LoginWindow as the initial window
            desktop.MainWindow = loginWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
    private void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
        // Configure DbContext with connection string from configuration
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ISecurityService, SecurityService>();
    }

    [Singleton(typeof(LoginViewModel))]
    [Singleton(typeof(MainViewModel))]
    [Transient(typeof(HomeViewModel))]
    [Transient(typeof(SettingsViewModel))]
    //[SuppressMessage("CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.InvalidServiceRegistrationAnalyzer", "TKEXDI0004:Duplicate service type registration")]
    internal static partial void ConfigureViewModels(IServiceCollection services);

    [Singleton(typeof(LoginWindow))]
    [Singleton(typeof(MainWindow))]
    [Transient(typeof(HomeView))]
    [Transient(typeof(SettingsView))]
    internal static partial void ConfigureViews(IServiceCollection services);
}
