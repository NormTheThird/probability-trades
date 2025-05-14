namespace ProbabilityTrades.UI.WPF.Commands;

public class MaximizeApplicationCommand :BaseCommand
{
    public override void Execute(object parameter)
    {
        var mainWindow = parameter as MainWindow;

        if (mainWindow.WindowState == WindowState.Normal)
        {
            mainWindow.WindowState = WindowState.Maximized;
        }
        else if (mainWindow.WindowState == WindowState.Maximized)
        {
            mainWindow.WindowState = WindowState.Normal;
        }
    }
}