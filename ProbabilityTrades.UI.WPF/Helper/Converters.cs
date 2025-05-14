namespace ProbabilityTrades.UI.WPF.Helper;

public class IsLessThanConverter : IValueConverter
{
    public readonly static IValueConverter Instance = new IsLessThanConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double doubleValue = System.Convert.ToDouble(value);
        double compareToValue = System.Convert.ToDouble(parameter);

        return doubleValue < compareToValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class IsGreaterThanConverter : IValueConverter
{
    public readonly static IValueConverter Instance = new IsGreaterThanConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double doubleValue = System.Convert.ToDouble(value);
        double compareToValue = System.Convert.ToDouble(parameter);

        return doubleValue > compareToValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class InverseBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class DecimalToCommaSeparatedStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal decimalValue)
            return decimalValue.ToString("#,0.##########", CultureInfo.InvariantCulture);

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}