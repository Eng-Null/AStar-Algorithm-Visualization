using System.Windows.Media;

namespace WPF;

public class IconToBrushValueConverter : BaseValueConverter<IconToBrushValueConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            ExtraCondition.Clear => new SolidColorBrush(Colors.LightGray),
            ExtraCondition.Sunny => new SolidColorBrush(Colors.LightYellow),
            ExtraCondition.Cloudy => new SolidColorBrush(Colors.Gray),
            ExtraCondition.Hail => new SolidColorBrush(Colors.WhiteSmoke),
            ExtraCondition.Rainy => new SolidColorBrush(Colors.LightBlue),
            ExtraCondition.HeavyRain => new SolidColorBrush(Colors.DarkBlue),
            ExtraCondition.Lightning => new SolidColorBrush(Colors.Yellow),
            ExtraCondition.LightningRainy => new SolidColorBrush(Colors.GreenYellow),
            ExtraCondition.Road => new SolidColorBrush(Colors.Black),
            _ => new SolidColorBrush(Colors.White),
        };
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
