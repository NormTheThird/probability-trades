namespace ProbabilityTrades.UI.WPF.Services;

public class NavigationService<TViewModel> 
    where TViewModel : BaseViewModel
{
    private readonly NavigationStore _navigationStore;
    private readonly Func<TViewModel> _viewModel;

    public NavigationService(NavigationStore navigationStore, Func<TViewModel> viewModel)
    {
        _navigationStore = navigationStore ?? throw new ArgumentNullException(nameof(navigationStore));
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }

    public void Navigate()
    {
        _navigationStore.CurrentViewModel = _viewModel();
        _navigationStore.CurrentViewModel.LoadAsync();
    }
}

public class NavigationService<TParameter, TViewModel>
      where TViewModel : BaseViewModel
{ 
    private readonly NavigationStore _navigationStore;
    private readonly Func<TParameter, TViewModel> _viewModel;

    public NavigationService(NavigationStore navigationStore, Func<TParameter, TViewModel> viewModel)
    {
        _navigationStore = navigationStore ?? throw new ArgumentNullException(nameof(navigationStore));
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }

    public void Navigate(TParameter parameter)
    {
        _navigationStore.CurrentViewModel = _viewModel(parameter);
    }
}