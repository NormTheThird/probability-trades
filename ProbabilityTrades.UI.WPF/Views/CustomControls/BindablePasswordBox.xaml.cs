namespace ProbabilityTrades.UI.WPF.Views.CustomControls;

public partial class BindablePasswordBox : UserControl
{
    public readonly static DependencyProperty PasswordProperty = 
              DependencyProperty.Register("Password", typeof(string), typeof(BindablePasswordBox));

    public string Password
    {
        get => (string)GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }

    public BindablePasswordBox()
    {
        InitializeComponent();
        txtPassword.PasswordChanged += OnPasswordChanged;
    }

    private void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        Password = txtPassword.Password;
    }
}