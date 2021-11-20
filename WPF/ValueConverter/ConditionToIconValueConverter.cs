using MaterialDesignThemes.Wpf;

namespace WPF;

public class ConditionToIconValueConverter : BaseValueConverter<ConditionToIconValueConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            Enum.ExtraCondition.Clear => new PackIcon().Kind = PackIconKind.Dot,
            Enum.ExtraCondition.Road => new PackIcon().Kind = PackIconKind.Road,
            Enum.ExtraCondition.Sunny => new PackIcon().Kind = PackIconKind.WeatherSunny,
            Enum.ExtraCondition.Cloudy => new PackIcon().Kind = PackIconKind.WeatherCloudy,
            Enum.ExtraCondition.Hail => new PackIcon().Kind = PackIconKind.WeatherHail,
            Enum.ExtraCondition.Rainy => new PackIcon().Kind = PackIconKind.WeatherRainy,
            Enum.ExtraCondition.HeavyRain => new PackIcon().Kind = PackIconKind.WeatherHeavyRain,
            Enum.ExtraCondition.Lightning => new PackIcon().Kind = PackIconKind.WeatherLightning,
            Enum.ExtraCondition.LightningRainy => new PackIcon().Kind = PackIconKind.WeatherLightningRainy,

            _ => new PackIcon().Kind = PackIconKind.Dot,
        };
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
