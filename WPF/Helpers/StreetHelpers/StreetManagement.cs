namespace WPF;

public static class StreetManagement
{
    private static readonly Random Random = new();
    private static Node[,] NodeMap { get; set; }
    private static int X;
    private static int Y;

    public static async Task<Node[,]> AddStreetAsync(int x, int y, Node[,] nodeMap)
    {
        NodeMap = nodeMap;
        X = x;
        Y = y;
        NodeMap = await AddStreetOuterWallsAsync();
        await AddStreetInnerWallsAsync(true, 1, Y - 2, 1, X - 2, new Point(X - 2, Y - 2));
        await WallToRoadAsync();
        return NodeMap;
    }

    private static async Task WallToRoadAsync()
    {
        await GetNeighborsAsync();
        await SetNodeSides();
        await Task.Run(() =>
        {
            foreach (var node in NodeMap)
            {
                if (node.IsObstacle)
                {
                    node.IsObstacle = false;
                    node.IsRoad = true;

                    node.Condition = ExtraCondition.Road;

                    #region Street Tiles

                    if (node.Left && node.Right && !node.Top && !node.Bottom)
                    {
                        node.Style = AStarSet.RoadH;
                    }
                    if (!node.Left && !node.Right && node.Top && node.Bottom)
                    {
                        node.Style = AStarSet.RoadV;
                    }

                    //1 Sides
                    if (!node.Left && !node.Right && node.Top && !node.Bottom)
                    {
                        node.Style = AStarSet.RoadT;
                    }
                    if (!node.Left && !node.Right && !node.Top && node.Bottom)
                    {
                        node.Style = AStarSet.RoadB;
                    }
                    if (node.Left && !node.Right && !node.Top && !node.Bottom)
                    {
                        node.Style = AStarSet.RoadL;
                    }
                    if (!node.Left && node.Right && !node.Top && !node.Bottom)
                    {
                        node.Style = AStarSet.RoadR;
                    }

                    //2 Sides
                    if (node.Left && !node.Right && node.Top && !node.Bottom)
                    {
                        node.Style = AStarSet.RoadTL;
                    }
                    if (!node.Left && node.Right && node.Top && !node.Bottom)
                    {
                        node.Style = AStarSet.RoadTR;
                    }
                    if (node.Left && !node.Right && !node.Top && node.Bottom)
                    {
                        node.Style = AStarSet.RoadBL;
                    }
                    if (!node.Left && node.Right && !node.Top && node.Bottom)
                    {
                        node.Style = AStarSet.RoadBR;
                    }

                    //3 Sides
                    if (node.Left && node.Right && !node.Top && node.Bottom)
                    {
                        node.Style = AStarSet.RoadBLF;
                    }
                    if (node.Left && !node.Right && node.Top && node.Bottom)
                    {
                        node.Style = AStarSet.RoadTBL;
                    }
                    if (!node.Left && node.Right && node.Top && node.Bottom)
                    {
                        node.Style = AStarSet.RoadTBR;
                    }
                    if (node.Left && node.Right && node.Top && !node.Bottom)
                    {
                        node.Style = AStarSet.RoadTLR;
                    }

                    //4 Sides
                    if (node.Left && node.Right && node.Top && node.Bottom)
                    {
                        node.Style = AStarSet.RoadTBLR;
                    }

                    #endregion Street Tiles

                    continue;
                }

                if (node.Style != AStarSet.Start && node.Style != AStarSet.End)
                {
                    if (Random.Next(100) < 80)
                    {
                        node.IsObstacle = true;
                        node.Style = AStarSet.Obstacle;
                    }
                }
            }

            //Set Border Back To Walls
            Parallel.For(0, X, i =>
            {
                if (i == 0 || i == X - 1)
                {
                    Parallel.For(0, Y, j =>
                    {
                        NodeMap[i, j].Style = AStarSet.RiverV;
                        NodeMap[i, j].IsObstacle = true;
                        NodeMap[i, j].Condition = ExtraCondition.Clear;
                    });
                    return;
                }

                NodeMap[i, 0].Style = AStarSet.RiverH;
                NodeMap[i, 0].IsObstacle = true;
                NodeMap[i, 0].Condition = ExtraCondition.Clear;
                NodeMap[i, Y - 1].Style = AStarSet.RiverH;
                NodeMap[i, Y - 1].IsObstacle = true;
                NodeMap[i, Y - 1].Condition = ExtraCondition.Clear;
            });

            NodeMap[0, 0].Style = AStarSet.RiverBR;
            NodeMap[X - 1, 0].Style = AStarSet.RiverBL;
            NodeMap[0, Y - 1].Style = AStarSet.RiverTR;
            NodeMap[X - 1, Y - 1].Style = AStarSet.RiverTL;
        });
    }

    private static async Task GetNeighborsAsync()
    {
        await Task.Run(() =>
        {
            Parallel.For(0, X, i =>
            {
                Parallel.For(0, Y, j =>
                {
                    if (i < X - 1)
                    {
                        NodeMap[i, j].Neighbors.Add(NodeMap[i + 1, j]);
                    }

                    if (i > 0)
                    {
                        NodeMap[i, j].Neighbors.Add(NodeMap[i - 1, j]);
                    }

                    if (j < Y - 1)
                    {
                        NodeMap[i, j].Neighbors.Add(NodeMap[i, j + 1]);
                    }

                    if (j > 0)
                    {
                        NodeMap[i, j].Neighbors.Add(NodeMap[i, j - 1]);
                    }
                });
            });
        });
    }

    private static async Task SetNodeSides()
    {
        await Task.Run(() =>
        {
            foreach (var node in NodeMap)
            {
                if (node.IsObstacle)
                {
                    foreach (var side in node.Neighbors)
                    {
                        if (side.X + 1 == node.X && side.Y == node.Y && side.IsObstacle)
                        {
                            node.Left = true;
                        }

                        if (side.X == node.X + 1 && side.Y == node.Y && side.IsObstacle)
                        {
                            node.Right = true;
                        }

                        if (side.X == node.X && side.Y == node.Y + 1 && side.IsObstacle)
                        {
                            node.Bottom = true;
                        }

                        if (side.X == node.X && side.Y + 1 == node.Y && side.IsObstacle)
                        {
                            node.Top = true;
                        }
                    }
                }
            }
        });
    }

    private static async Task<Node[,]> AddStreetOuterWallsAsync()
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
                }
                else
                {
                    NodeMap[i, 0].Style = AStarSet.Maze;
                    NodeMap[i, 0].IsObstacle = true;
                    NodeMap[i, 0].Condition = ExtraCondition.Clear;
                    NodeMap[i, Y - 1].Style = AStarSet.Maze;
                    NodeMap[i, Y - 1].IsObstacle = true;
                    NodeMap[i, Y - 1].Condition = ExtraCondition.Clear;
                }
            });
            return NodeMap;
        });
    }

    private static async Task AddStreetInnerWallsAsync(bool h, int minX, int maxX, int minY, int maxY, Point gate)
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
                                   AddStreetInnerWallsAsync(!h, minX, maxX, minY, (int)y - 1, gate),
                                   AddStreetInnerWallsAsync(!h, minX, maxX, (int)y + 1, maxY, gate));
                return;
            }
            if (maxY - minY < 2)
            {
                return;
            }
            var x = Math.Floor(await RandomNumberAsync(minX, maxX) / 2) * 2;
            await Task.WhenAll(AddVWallAsync(minY, maxY, (int)x),
                               AddStreetInnerWallsAsync(!h, minX, (int)x - 1, minY, maxY, gate),
                               AddStreetInnerWallsAsync(!h, (int)x + 1, maxX, minY, maxY, gate));
        });
    }

    private static async Task AddVWallAsync(int minY, int maxY, int x)
    {
        await Task.Run(() =>
        {
            for (int i = minY; i <= maxY; i++)
            {
                NodeMap[i, x].Style = AStarSet.Maze;
                NodeMap[i, x].IsObstacle = true;
                NodeMap[i, x].Condition = ExtraCondition.Clear;
            }
        });
    }

    private static async Task AddHWallAsync(int minX, int maxX, int y)
    {
        await Task.Run(() =>
        {
            for (int i = minX; i <= maxX; i++)
            {
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

    public static async Task<Node[,]> RemoveStreetAsync(Node[,] NodeMap)
    {
        return await Task.Run(() =>
        {
            foreach (var node in NodeMap)
            {
                if (node.IsObstacle || node.IsRoad)
                {
                    node.Style = AStarSet.Undefined;
                    node.IsObstacle = false;
                    node.IsRoad = false;
                }
            }
            return NodeMap;
        });
    }
}