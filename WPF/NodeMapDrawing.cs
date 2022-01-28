using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPF;

public class NodeMapDrawing
{
    private static byte[] BitmapSourceToArray(BitmapSource bitmapSource)
    {
        // Stride = (width) x (bytes per pixel)
        int stride = bitmapSource.PixelWidth * (bitmapSource.Format.BitsPerPixel / 8);
        byte[] pixels = new byte[bitmapSource.PixelHeight * stride];

        bitmapSource.CopyPixels(pixels, stride, 0);

        return pixels;
    }

    public static WriteableBitmap DrawNodeMap(int X, int Y, Node[,] nodes)
    {
        WriteableBitmap wb = new(X * 64, Y * 64, 96, 96, PixelFormats.Bgra32, null);

        for (int i = 0; i < X; i++)
        {
            for (int j = 0; j < Y; j++)
            {
                BitmapImage? bitmapImage = new(new Uri(Convert(nodes[i, j].Style)));

                Int32Rect rect = new(i * 64, j * 64, bitmapImage.PixelWidth, bitmapImage.PixelHeight);
                byte[] Data = BitmapSourceToArray(bitmapImage);
                int stride = (((bitmapImage.PixelWidth * 32) + 31) & ~31) / 8;

                wb.WritePixels(rect, Data, stride, 0);
            }
        }

        return wb;
    }

    private static string Convert(AStarSet value)
    {
        return value switch
        {
            AStarSet.Start => $"pack://application:,,,/Img/HouseStart.png",
            AStarSet.End => $"pack://application:,,,/Img/HouseEnd.png",

            AStarSet.Obstacle => GetRandomObstacle(),
            AStarSet.Maze => $"pack://application:,,,/Img/Wall.png",

            AStarSet.RoadH => $"pack://application:,,,/Img/RoadH.png",
            AStarSet.RoadV => $"pack://application:,,,/Img/RoadV.png",

            AStarSet.RoadTL => $"pack://application:,,,/Img/LeftTop.png",
            AStarSet.RoadTR => $"pack://application:,,,/Img/RightTop.png",
            AStarSet.RoadBL => $"pack://application:,,,/Img/LeftBottom.png",
            AStarSet.RoadBR => $"pack://application:,,,/Img/RightBottom.png",

            AStarSet.RoadBLF => $"pack://application:,,,/Img/BottomLeftRight.png",
            AStarSet.RoadTBL => $"pack://application:,,,/Img/TopBottomLeft.png",
            AStarSet.RoadTBR => $"pack://application:,,,/Img/TopBottomRight.png",
            AStarSet.RoadTLR => $"pack://application:,,,/Img/TopLeftRight.png",

            AStarSet.RoadTBLR => $"pack://application:,,,/Img/TopBottomLeftRight.png",

            AStarSet.RoadT => $"pack://application:,,,/Img/RoadTop.png",
            AStarSet.RoadB => $"pack://application:,,,/Img/RoadBottom.png",
            AStarSet.RoadL => $"pack://application:,,,/Img/RoadLeft.png",
            AStarSet.RoadR => $"pack://application:,,,/Img/RoadRight.png",

            AStarSet.Path => $"pack://application:,,,/Img/RoadV.png",
            AStarSet.Undefined => $"pack://application:,,,/Img/GroundGreen.png",

            AStarSet.RiverV => $"pack://application:,,,/Img/RiverV.png",
            AStarSet.RiverH => $"pack://application:,,,/Img/RiverH.png",

            AStarSet.RiverTL => $"pack://application:,,,/Img/TopLeftRiver.png",
            AStarSet.RiverTR => $"pack://application:,,,/Img/TopRightRiver.png",
            AStarSet.RiverBL => $"pack://application:,,,/Img/BottomLeftRiver.png",
            AStarSet.RiverBR => $"pack://application:,,,/Img/BottomRightRiver.png",

            AStarSet.EmptyGround => $"pack://application:,,,/Img/GroundEmpty.png",
            AStarSet.EmptyStart => $"pack://application:,,,/Img/EmptyStart.png",
            AStarSet.EmptyEnd => $"pack://application:,,,/Img/EmptyEnd.png",
            AStarSet.EmptyObstacle => $"pack://application:,,,/Img/EmptyObstacle.png",
            _ => "",
        };
    }

    private static string GetRandomObstacle()
    {
        Dictionary<string, float> condition = new();
        for (int i = 1; i <= 15; i++)
        {
            condition.Add($"pack://application:,,,/Img/Tree{i}.png", 0.5f);
        }
        for (int i = 1; i <= 15; i++)
        {
            condition.Add($"pack://application:,,,/Img/House{i}.png", 0.8f);
        }

        return condition.RandomElementByWeight(e => e.Value).Key;
    }
}