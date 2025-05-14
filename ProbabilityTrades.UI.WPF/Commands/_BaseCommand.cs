namespace ProbabilityTrades.UI.WPF.Commands;

public abstract class BaseCommand : ICommand
{
    public event EventHandler CanExecuteChanged;

    public virtual bool CanExecute(object parameter)
    {
        return true;
    }

    public abstract void Execute(object parameter);

    protected void OnCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

public abstract class BaseCommandAsync : ICommand
{
    private bool _isExecuting;
    public bool IsExecuting
    {
        get => _isExecuting;
        set
        {
            _isExecuting = value;
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }

    public event EventHandler CanExecuteChanged;

    public virtual bool CanExecute(object parameter)
    {
        return !IsExecuting;
    }

    public async void Execute(object parameter)
    {
        IsExecuting = true;
        await ExecuteAsync(parameter);
        IsExecuting = false;
    }

    protected abstract Task ExecuteAsync(object parameter);
}