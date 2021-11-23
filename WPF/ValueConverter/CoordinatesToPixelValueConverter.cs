namespace WPF;

public class CoordinatesToPixelValueConverter : BaseValueConverter<CoordinatesToPixelValueConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return int.Parse(value.ToString()) * StaticValues.TileSize;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
