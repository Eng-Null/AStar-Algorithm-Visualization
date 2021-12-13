namespace WPF
{
    public class StaticValues : BaseViewModel
    {
        private static int x = 11;
        private static int y = 11;

        public static int TileSize { get; set; } = 20;

        public static int X
        {
            get => x;
            set
            {
                x = value;
                EndPointX = value - 2;
            }
        }

        public static int Y
        {
            get => y;
            set
            {
                y = value;
                EndPointY = value - 2;
            }
        }

        public static int StartPointX { get; set; } = 1;
        public static int StartPointY { get; set; } = 1;
        public static int EndPointX { get; set; } = X - 2;
        public static int EndPointY { get; set; } = Y - 2;
        public static int MapType { get; set; }
    }
}