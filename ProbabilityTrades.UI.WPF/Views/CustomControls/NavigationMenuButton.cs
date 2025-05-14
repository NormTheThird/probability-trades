namespace ProbabilityTrades.UI.WPF.Views.CustomControls;

public class NavigationMenuButton : Button
{
    public readonly static DependencyProperty IsSelectedProperty =
        DependencyProperty.Register("IsSelected", typeof(bool), typeof(NavigationMenuButton), new PropertyMetadata(false));

    public bool IsSelected
    {
        get { return (bool)GetValue(IsSelectedProperty); }
        set { SetValue(IsSelectedProperty, value); }
    }

    public readonly static DependencyProperty ImageProperty =
        DependencyProperty.Register("Image", typeof(string), typeof(NavigationMenuButton), new PropertyMetadata(string.Empty));

    public string Image
    {
        get { return (string)GetValue(ImageProperty); }
        set { SetValue(ImageProperty, value); }
    }
}