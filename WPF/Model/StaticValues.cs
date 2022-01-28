namespace WPF
{
    public class StaticValues : BaseViewModel
    {
        private static int y = 17;

        public static int TileSize { get; set; } = 32;

        public static int X { get; set; } = 43;
        public static int Y
        {
            get => y;
            set
            {
                y = value;
                EndPointX = X - 2;
                EndPointY = Y - 2;
            }
        }
        public static int StartPointX { get; set; } = 1;
        public static int StartPointY { get; set; } = 1;
        public static int EndPointX { get; set; } = X - 2;
        public static int EndPointY { get; set; } = Y - 2;
        public static int MapType { get; set; }
    }
}