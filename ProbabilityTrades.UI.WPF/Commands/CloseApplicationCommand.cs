namespace ProbabilityTrades.UI.WPF.Commands;

public class CloseApplicationCommand : BaseCommand
{
    public override void Execute(object parameter)
    {
        App.Current.Shutdown();
    }
}