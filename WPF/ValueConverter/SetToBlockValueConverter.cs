namespace WPF.ValueConverter;

public class SetToBlockValueConverter : BaseValueConverter<SetToBlockValueConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            AStarSet.Start => $"/Img/HouseStart.png",
            AStarSet.End => $"/Img/HouseEnd.png",

            AStarSet.Obstacle => GetRandomObstacle(),
            AStarSet.Maze => $"/Img/Wall.png",

            AStarSet.RoadH => $"/Img/RoadH.png",
            AStarSet.RoadV => $"/Img/RoadV.png",

            AStarSet.RoadTL => $"/Img/LeftTop.png",
            AStarSet.RoadTR => $"/Img/RightTop.png",
            AStarSet.RoadBL => $"/Img/LeftBottom.png",
            AStarSet.RoadBR => $"/Img/RightBottom.png",

            AStarSet.RoadBLF => $"/Img/BottomLeftRight.png",
            AStarSet.RoadTBL => $"/Img/TopBottomLeft.png",
            AStarSet.RoadTBR => $"/Img/TopBottomRight.png",
            AStarSet.RoadTLR => $"/Img/TopLeftRight.png",

            AStarSet.RoadTBLR => $"/Img/TopBottomLeftRight.png",

            AStarSet.RoadT => $"/Img/RoadTop.png",
            AStarSet.RoadB => $"/Img/RoadBottom.png",
            AStarSet.RoadL => $"/Img/RoadLeft.png",
            AStarSet.RoadR => $"/Img/RoadRight.png",

            AStarSet.Path => $"/Img/RoadV.png",
            AStarSet.Undefined => $"/Img/GroundGreen.png",

            AStarSet.RiverV => $"/Img/RiverV.png",
            AStarSet.RiverH => $"/Img/RiverH.png",

            AStarSet.RiverTL => $"/Img/TopLeftRiver.png",
            AStarSet.RiverTR => $"/Img/TopRightRiver.png",
            AStarSet.RiverBL => $"/Img/BottomLeftRiver.png",
            AStarSet.RiverBR => $"/Img/BottomRightRiver.png",

            AStarSet.EmptyGround => $"/Img/GroundEmpty.png",
            AStarSet.EmptyStart => $"/Img/EmptyStart.png",
            AStarSet.EmptyEnd => $"/Img/EmptyEnd.png",
            AStarSet.EmptyObstacle => $"/Img/EmptyObstacle.png",
            _ => "",
        };
    }
    private static string GetRandomObstacle()
    {
        Dictionary<string, float> condition = new();
        for (int i = 1; i <= 15; i++)
        {
            condition.Add($"/Img/Tree{i}.png", 0.5f);
        }
        for (int i = 1; i <= 15; i++)
        {
            condition.Add($"/Img/House{i}.png", 0.8f);
        }

        return condition.RandomElementByWeight(e => e.Value).Key;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}