namespace ProbabilityTrades.UI.WPF.ViewModels;

public abstract class BaseViewModel : ObservableObject
{
    public readonly UserSingleton _userSingleton;

    public BaseViewModel(UserSingleton userSingleton)
    {
        _userSingleton = userSingleton ?? throw new ArgumentNullException(nameof(userSingleton));
    }

    public virtual async Task LoadAsync() { }
}