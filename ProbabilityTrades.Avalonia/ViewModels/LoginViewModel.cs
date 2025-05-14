namespace ProbabilityTrades.Avalonia.ViewModels;

public partial class LoginViewModel: BaseViewModel
{
    private readonly ISecurityService _securityService;

    public LoginViewModel(ISecurityService securityService)
    {
          _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
    }

    [RelayCommand] 
    private void Login()
    {
        try
        {
            // Perform login logic here

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Get the MainWindow
                var mainWindow = Ioc.Default.GetRequiredService<MainWindow>();

                // Hide the LoginWindow
                var loginWindow = desktop.MainWindow as LoginWindow;
                if (loginWindow != null)
                {
                    loginWindow.Hide();  // Properly close the LoginWindow
                }

                // Handle MainWindow closed event to exit application
                mainWindow.Closed += (s, e) => desktop.Shutdown();

                // Set and show the MainWindow
                desktop.MainWindow = mainWindow;
                mainWindow.Show();
            }
        }
        catch (System.Exception ex)
        {
            throw ex;
        }
    }
}