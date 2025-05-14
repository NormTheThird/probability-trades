using Avalonia.Interactivity;

namespace ProbabilityTrades.Avalonia;

public partial class LoginWindow : Window
{
    // constructor with 1 parameter is needed to stop the DI to instantly create the window (when declared as singleton)
    // during the startup phase and crashing the whole android app
    // with "Specified method is not supported window" error
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}