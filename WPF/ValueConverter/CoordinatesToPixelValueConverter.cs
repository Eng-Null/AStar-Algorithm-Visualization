using System.Diagnostics.CodeAnalysis;

namespace WPF;

public class CoordinatesToPixelValueConverter : BaseValueConverter<CoordinatesToPixelValueConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int && value is not null)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            return int.Parse(value.ToString()) * 20;
        }
        else return 0;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
