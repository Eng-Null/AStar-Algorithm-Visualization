using MaterialDesignThemes.Wpf;

namespace WPF;

public class ConditionToIconValueConverter : BaseValueConverter<ConditionToIconValueConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            ExtraCondition.Clear => new PackIcon().Kind = PackIconKind.Dot,
            ExtraCondition.Road => new PackIcon().Kind = PackIconKind.Road,
            ExtraCondition.Sunny => new PackIcon().Kind = PackIconKind.WeatherSunny,
            ExtraCondition.Cloudy => new PackIcon().Kind = PackIconKind.WeatherCloudy,
            ExtraCondition.Hail => new PackIcon().Kind = PackIconKind.WeatherHail,
            ExtraCondition.Rainy => new PackIcon().Kind = PackIconKind.WeatherRainy,
            ExtraCondition.HeavyRain => new PackIcon().Kind = PackIconKind.WeatherHeavyRain,
            ExtraCondition.Lightning => new PackIcon().Kind = PackIconKind.WeatherLightning,
            ExtraCondition.LightningRainy => new PackIcon().Kind = PackIconKind.WeatherLightningRainy,

            _ => new PackIcon().Kind = PackIconKind.Dot,
        };
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
