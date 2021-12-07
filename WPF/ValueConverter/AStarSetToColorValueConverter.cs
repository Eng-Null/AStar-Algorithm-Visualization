namespace WPF;

public class AStarSetToColorValueConverter : BaseValueConverter<AStarSetToColorValueConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            Set.Closed => $"/Img/ClosedSet.png",
            Set.Open => $"/Img/OpenSet.png",
            Set.Undefined => null,
            _ => null,
        };
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}