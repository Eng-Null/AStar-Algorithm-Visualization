namespace WPF;

public class ConditionToOpacityValueConverter : BaseValueConverter<ConditionToOpacityValueConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            AStarSet.Obstacle => 0,
            _ => 100,
        };
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
