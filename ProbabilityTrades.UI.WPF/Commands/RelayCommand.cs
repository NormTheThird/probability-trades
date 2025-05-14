namespace ProbabilityTrades.UI.WPF.Commands;

/// <summary>
///     RelayCommand is a class that implements ICommand. It is used to bind a command to a control.
///     
///     USAGE: "COMMAND" = new RelayCommand(async _ => await "METHOD"(), _ => true);
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Predicate<object> _canExecute;

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
        _execute(parameter);
    }
}

/// <summary>
///     RelayCommandAsync is a class that implements ICommand. It is used to bind a command to a control.
/// 
///     USAGE: public ICommand MyCommand => new RelayCommandAsync(MyMethodAsync);
/// </summary>
public class RelayCommandAsync : BaseCommandAsync
{
    private readonly Func<Task> _callback;

    public RelayCommandAsync(Func<Task> callback)
    {
        _callback = callback ?? throw new ArgumentNullException(nameof(callback));
    }

    protected override async Task ExecuteAsync(object parameter)
    {
        await _callback();
    }
}