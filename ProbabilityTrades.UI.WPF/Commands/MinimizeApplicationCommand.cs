namespace ProbabilityTrades.UI.WPF.Commands;

public class MinimizeApplicationCommand : BaseCommand
{
    public override void Execute(object parameter)
    {
        var mainWindow = parameter as MainWindow;

        if (mainWindow.WindowState == WindowState.Normal)
        {
            mainWindow.WindowState = WindowState.Minimized;
        }
    }
}