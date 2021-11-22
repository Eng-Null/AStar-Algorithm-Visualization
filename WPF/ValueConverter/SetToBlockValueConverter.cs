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
            _ => null,
        };
    }

    //private object GetRandomMaze()
    //{
    //    var rnd = new Random();
    //    return rnd.Next(4) switch
    //    {
    //        0 => $"/Img/House1.png",
    //        1 => $"/Img/House2.png",
    //        2 => $"/Img/House3.png",
    //        3 => $"/Img/House4.png",
    //        4 => $"/Img/House5.png",
    //        _ => null,
    //    };
    //}

    private static string GetRandomObstacle()
    {
        var rnd = new Random();
        return rnd.Next(11) switch
        {
            0 => $"/Img/Tree1.png",
            1 => $"/Img/Tree2.png",
            2 => $"/Img/Tree3.png",
            3 => $"/Img/Tree4.png",
            4 => $"/Img/Tree5.png",
            5 => $"/Img/Tree6.png",
            6 => $"/Img/Tree7.png",

            7 => $"/Img/House1.png",
            8 => $"/Img/House2.png",
            9 => $"/Img/House3.png",
            10 => $"/Img/House4.png",
            11 => $"/Img/House5.png",
            _ => null,
        };
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}