namespace ProbabilityTrades.UI.WPF.Commands;

public class NavigateCommand<TViewModel> : BaseCommand
    where TViewModel : BaseViewModel
{
    private readonly NavigationService<TViewModel> _navigationService;

    public NavigateCommand(NavigationService<TViewModel> navigationService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
    }

    public override void Execute(object parameter)
    {
        _navigationService.Navigate();
    }
}