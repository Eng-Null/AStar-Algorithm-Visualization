namespace WPF;

public static class MazeManagement
{
    private static Node[,] NodeMap { get; set; }
    private static int X;
    private static int Y;

    public static async Task<Node[,]> AddMazeAsync(int x, int y, Node[,] nodeMap)
    {
        NodeMap = nodeMap;
        X = x;
        Y = y;
        NodeMap = await AddMazeOuterWallsAsync();
        await AddMazeInnerWallsAsync(true, 1, Y - 2, 1, X - 2, new Point(X - 2, Y - 2));
        return NodeMap;
    }

    private static async Task<Node[,]> AddMazeOuterWallsAsync()
    {
        return await Task.Run(() =>
        {
            Parallel.For(0, X, i =>
            {
                if (i == 0 || i == X - 1)
                {
                    Parallel.For(0, Y, j =>
                    {
                        NodeMap[i, j].Style = AStarSet.Maze;
                        NodeMap[i, j].IsObstacle = true;
                        NodeMap[i, j].Condition = ExtraCondition.Clear;
                    });
                    return;
                }
                NodeMap[i, 0].Style = AStarSet.Maze;
                NodeMap[i, 0].IsObstacle = true;
                NodeMap[i, 0].Condition = ExtraCondition.Clear;
                NodeMap[i, Y - 1].Style = AStarSet.Maze;
                NodeMap[i, Y - 1].IsObstacle = true;
                NodeMap[i, Y - 1].Condition = ExtraCondition.Clear;
            });
            return NodeMap;
        });
    }

    private static async Task AddMazeInnerWallsAsync(bool h, int minX, int maxX, int minY, int maxY, Point gate)
    {
        await Task.Run(async () =>
        {
            if (h)
            {
                if (maxX - minX < 2)
                {
                    return;
                }
                var y = Math.Floor(await RandomNumberAsync(minY, maxY) / 2) * 2;
                await Task.WhenAll(AddHWallAsync(minX, maxX, (int)y),
                                   AddMazeInnerWallsAsync(!h, minX, maxX, minY, (int)y - 1, gate),
                                   AddMazeInnerWallsAsync(!h, minX, maxX, (int)y + 1, maxY, gate));
                return;
            }
            if (maxY - minY < 2)
            {
                return;
            }
            var x = Math.Floor(await RandomNumberAsync(minX, maxX) / 2) * 2;
            await Task.WhenAll(AddVWallAsync(minY, maxY, (int)x),
                               AddMazeInnerWallsAsync(!h, minX, (int)x - 1, minY, maxY, gate),
                               AddMazeInnerWallsAsync(!h, (int)x + 1, maxX, minY, maxY, gate));
        });
    }

    private static async Task AddVWallAsync(int minY, int maxY, int x)
    {
        await Task.Run(async () =>
        {
            decimal hole = (Math.Floor(await RandomNumberAsync(minY, maxY) / 2) * 2) + 1;

            for (int i = minY; i <= maxY; i++)
            {
                if (i == hole)
                {
                    NodeMap[i, x].Style = AStarSet.Undefined;
                    NodeMap[i, x].IsObstacle = false;
                    continue;
                }
                NodeMap[i, x].Style = AStarSet.Maze;
                NodeMap[i, x].IsObstacle = true;
                NodeMap[i, x].Condition = ExtraCondition.Clear;
            }
        });
    }

    private static async Task AddHWallAsync(int minX, int maxX, int y)
    {
        await Task.Run(async () =>
        {
            decimal hole = (Math.Floor(await RandomNumberAsync(minX, maxX) / 2) * 2) + 1;
            for (int i = minX; i <= maxX; i++)
            {
                if (i == hole)
                {
                    NodeMap[y, i].Style = AStarSet.Undefined;
                    NodeMap[y, i].IsObstacle = false;
                    continue;
                }
                NodeMap[y, i].Style = AStarSet.Maze;
                NodeMap[y, i].IsObstacle = true;
                NodeMap[y, i].Condition = ExtraCondition.Clear;
            }
        });
    }

    private static async ValueTask<decimal> RandomNumberAsync(int min, int max)
    {
        Random? rnd = new();
        return await new ValueTask<decimal>((decimal)Math.Floor((rnd.NextDouble() * (max - min + 1)) + min));
    }

    public static async Task<Node[,]> RemoveMazeAsync(Node[,] nodeMap)
    {
        return await Task.Run(() =>
        {
            foreach (var node in nodeMap)
            {
                if (node.IsObstacle || node.IsRoad)
                {
                    node.Style = Enum.AStarSet.Undefined;
                    node.IsObstacle = false;
                    node.IsRoad = false;
                }
            }
            return nodeMap;
        });
    }
}