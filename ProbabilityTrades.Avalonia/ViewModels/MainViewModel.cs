namespace ProbabilityTrades.Avalonia.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    public MainViewModel()
    {
        NavigationItems = new ObservableCollection<NavigationItemRecord>(navigationItemRecords);
        // SelectedNavigationItem = navigationItems.First(vm => vm.ModelType == typeof(HomeViewModel));
    }

    partial void OnSelectedNavigationItemChanged(NavigationItemRecord? value)
    {
        try
        {
            if (value is null) return;

            var vm = Design.IsDesignMode
                ? Activator.CreateInstance(value.ModelType)
                : Ioc.Default.GetService(value.ModelType);

            if (vm is not BaseViewModel vmb) return;

            CurrentPage = vmb;
        }
        catch (Exception)
        {
            throw;
        }

    }

    [ObservableProperty] private bool isPaneOpen = true;
    [ObservableProperty] private bool isLoginViewActive = true;
    [ObservableProperty] private ObservableCollection<NavigationItemRecord> navigationItems;
    [ObservableProperty] private NavigationItemRecord? selectedNavigationItem;
    [ObservableProperty] private BaseViewModel currentPage = new HomeViewModel();

    [RelayCommand] private void TriggerPane() => IsPaneOpen ^= true;


    private readonly List<NavigationItemRecord> navigationItemRecords =
    [
        new NavigationItemRecord(typeof(HomeViewModel), MaterialIconKind.Home, "Home"),
        new NavigationItemRecord(typeof(SettingsViewModel), MaterialIconKind.Settings, "Settings"),
    ];
}