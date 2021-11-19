using System.Windows.Media;

namespace WPF;

public class IconToBrushValueConverter : BaseValueConverter<IconToBrushValueConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            Enum.ExtraCondition.Clear => new SolidColorBrush(Colors.LightGray),
            Enum.ExtraCondition.Sunny => new SolidColorBrush(Colors.LightYellow),
            Enum.ExtraCondition.Cloudy => new SolidColorBrush(Colors.Gray),
            Enum.ExtraCondition.Hail => new SolidColorBrush(Colors.WhiteSmoke),
            Enum.ExtraCondition.Rainy => new SolidColorBrush(Colors.LightBlue),
            Enum.ExtraCondition.HeavyRain => new SolidColorBrush(Colors.DarkBlue),
            Enum.ExtraCondition.Lightning => new SolidColorBrush(Colors.Yellow),
            Enum.ExtraCondition.LightningRainy => new SolidColorBrush(Colors.GreenYellow),
            Enum.ExtraCondition.Road => new SolidColorBrush(Colors.Black),


            _ => new SolidColorBrush(Colors.White),
        };
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
