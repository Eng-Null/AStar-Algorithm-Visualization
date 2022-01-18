namespace WPF
{
    public class StaticValues : BaseViewModel
    {
        public static int TileSize { get; set; } = 64;

        public static int X { get; set; } = 21;
        public static int Y { get; set; } = 9;

        public static int StartPointX { get; set; } = 1;
        public static int StartPointY { get; set; } = 1;
        public static int EndPointX { get; set; } = X - 2;
        public static int EndPointY { get; set; } = Y - 2;
        public static int MapType { get; set; }
    }
}