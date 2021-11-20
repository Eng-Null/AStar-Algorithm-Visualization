﻿namespace WPF.ViewModel;
//https://en.wikipedia.org/wiki/A*_search_algorithm
//https://en.wikipedia.org/wiki/Maze_generation_algorithm

public class AStarAlgorithmViewModel : BaseViewModel
{
    #region Algorithem Constants

    public int X { get; set; } = 61;
    public int Y { get; set; } = 31;
    public Node[,] NodeMap { get; set; }
    public List<Node> OpenSet { get; set; } = new();
    public List<Node> CloseSet { get; set; } = new();
    public List<Node> Path { get; set; } = new();
    public Node StartNode { get; set; } = new();
    public Node GoalNode { get; set; } = new();
    public Node Current { get; set; } = new();
    public bool IsDiagonalEnabled { get; set; }

    #endregion Algorithem Constants

    #region Visualization Constants

    public DelegateCommand StartCommand { get; set; }

    private bool CanStart() => NodeMap[0, 0] != null;

    public DelegateCommand CalculateCommand { get; set; }
    public DelegateCommand WallToRoadCommand { get; set; }

    private bool CanRoad() => NodeMap[0, 0] != null && NodeMap[0, 0].IsWall == true;

    public DelegateCommand SetConditionCommand { get; set; }
    public DelegateCommand RemoveConditionCommand { get; set; }

    private bool CanCondition() => NodeMap[0, 0] != null;

    public DelegateCommand AddMazeCommand { get; set; }
    public DelegateCommand RemoveMazeCommand { get; set; }

    private bool CanMaze() => NodeMap[0, 0] != null;

    public ObservableCollection<Node> Nodes { get; set; } = new();
    public ObservableCollection<PathScore> PathScore { get; set; } = new();
    public Point StartPoint { get; set; } = new();
    public Point EndPoint { get; set; } = new();
    public int Delay { get; set; } = 0;
    public Random Random = new();

    //private readonly object _Nodeslock = new();
    private readonly object _PathScorelock = new();

    public bool IsWeatherEnabled { get; set; }
    public bool IsMazeEnabled { get; set; }

    #endregion Visualization Constants

    public AStarAlgorithmViewModel()
    {
        StartCommand = new DelegateCommand(async () => await StartAsync(), CanStart);
        CalculateCommand = new DelegateCommand(async () => await CalculateNodesAsync());
        WallToRoadCommand = new DelegateCommand(async () => await WallToRoadAsync(), CanRoad);
        SetConditionCommand = new DelegateCommand(async () => await AddWeatherConditionAsync(), CanCondition);
        RemoveConditionCommand = new DelegateCommand(async () => await RemoveWeatherConditionAsync(), CanCondition);
        AddMazeCommand = new DelegateCommand(async () => await AddMazeAsync(), CanMaze);
        RemoveMazeCommand = new DelegateCommand(async () => await RemoveMazeAsync(), CanMaze);
        NodeMap = new Node[X, Y];
        // BindingOperations.EnableCollectionSynchronization(Nodes, _Nodeslock);
        BindingOperations.EnableCollectionSynchronization(PathScore, _PathScorelock);
    }

    #region A* search algorithm Code Section

    private async Task CalculateNodesAsync()
    {
        await Task.Run(async () =>
        {
            NodeMap = new Node[X, Y];
            for (int i = 0; i < X; i++)
            {
                for (int j = 0; j < Y; j++)
                {
                    NodeMap[i, j] = new Node
                    {
                        X = i,
                        Y = j
                    };
                }
            }
            await SetStartEndPoint();

            OpenSet.Clear();
            CloseSet.Clear();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Nodes.Clear();
            });

            await GetNeighborsAsync();
            await Task.WhenAll(ComputeHeuristicCosts(1), ClearNodeMapAsync());

            await GetNodesAsync();
        });
    }

    private async Task SetStartEndPoint()
    {
        await Task.Run(() =>
        {
            NodeMap[1, 1].Set = Enum.AStarSet.Start;
            NodeMap[1, 1].IsWall = false;
            NodeMap[1, 1].Condition = Enum.ExtraCondition.Clear;

            NodeMap[X - 2, Y - 2].Set = Enum.AStarSet.End;
            NodeMap[X - 2, Y - 2].IsWall = false;
            NodeMap[X - 2, Y - 2].Condition = Enum.ExtraCondition.Clear;

            StartPoint = new(1, 1);
            EndPoint = new(X - 2, Y - 2);

            StartNode = NodeMap[(int)StartPoint.X, (int)StartPoint.Y];
            GoalNode = NodeMap[(int)EndPoint.X, (int)EndPoint.Y];
        });
    }

    private async Task StartAsync()
    {
        //await GetNeighborsAsync(IsDiagonalEnabled);
        //await AddWeatherConditionAsync();
        //await ComputeHeuristicCosts();
        //await ClearNodeMapAsync();
        await Task.Run(async () =>
        {
            await ClearNodeNeighbors();
            await GetNeighborsAsync();
            await SetStartEndPoint();
            await Task.WhenAll(ComputeHeuristicCosts(1), ClearNodeMapAsync());

            OpenSet.Clear();
            CloseSet.Clear();

            // The set of discovered nodes that may need to be (re-)expanded.
            // Initially, only the start node is known.
            // This is usually implemented as a min-heap or priority queue rather than a hash-set.
            // openSet:= { start}
            OpenSet.Add(StartNode);

            while (OpenSet.Count > 0)
            {
                // This operation can occur in O(1) time if openSet is a min-heap or a priority queue
                await CheckForLowestFAsync();

                if (Current == GoalNode)
                {
                    await FindPathAsync();
                    await CheckOpenSetCloseSetPathAsync();
                    await CalculatePathValueAsync();
                    return;
                }

                for (int i = 0; i < Current.Neighbors.Count; i++)
                {
                    var neighbor = Current.Neighbors[i];
                    if (!CloseSet.Contains(neighbor) && !neighbor.IsWall)
                    {
                        // tentative_gScore is the distance from start to the neighbor through current and in this example the distance between current and neighbor is 1
                        // tentative_gScore:= gScore[current] + d(current, neighbor)
                        var tentativeGScore = Current.G + await MovementCost(neighbor, Current);
                        if (!OpenSet.Contains(neighbor))
                        {
                            OpenSet.Add(neighbor);
                        }
                        else if (tentativeGScore >= neighbor.G)
                        {
                            continue;
                        }

                        neighbor.G = tentativeGScore;
                        neighbor.F = neighbor.G + neighbor.H;
                        neighbor.CameFrom = Current;

                        OpenSet.Add(neighbor);
                    }
                    await FindPathAsync();
                    await CheckOpenSetCloseSetPathAsync();
                    await Task.Delay(Delay);
                }
            }
        });
    }

    private async Task GetNeighborsAsync()
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < X; i++)
            {
                for (int j = 0; j < Y; j++)
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
                    if (IsDiagonalEnabled)
                    {
                        // diagonal Pathfinding
                        if (i > 0 && j > 0)
                        {
                            NodeMap[i, j].Neighbors.Add(NodeMap[i - 1, j - 1]);
                        }

                        if (i < X - 1 && j > 0)
                        {
                            NodeMap[i, j].Neighbors.Add(NodeMap[i + 1, j - 1]);
                        }

                        if (i > 0 && j < Y - 1)
                        {
                            NodeMap[i, j].Neighbors.Add(NodeMap[i - 1, j + 1]);
                        }

                        if (i < X - 1 && j < Y - 1)
                        {
                            NodeMap[i, j].Neighbors.Add(NodeMap[i + 1, j + 1]);
                        }
                    }
                }
            }
        });
    }

    private async Task CheckForLowestFAsync()
    {
        await Task.Run(() =>
        {
            Current = OpenSet.OrderBy(x => x.H).ToList().First();
            OpenSet.Remove(Current);
            if (!CloseSet.Contains(Current))
            {
                CloseSet.Add(Current);
            }
        });
    }

    private async Task ComputeHeuristicCosts(double D)
    {
        await Task.Run(async () =>
        {
            foreach (Node node in NodeMap)
            {   //TODO: Find the Correct Spot To add the Weight to the Nodes
                node.H = await ManhattanDistance(node, GoalNode) + (double)node.Condition;
            }

            async ValueTask<double> ManhattanDistance(Node node, Node goal)
            {
                if (IsDiagonalEnabled)
                {
                    int dx = Math.Abs(node.X - goal.X);
                    int dy = Math.Abs(node.Y - goal.Y);
                    return await new ValueTask<double>(D * Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
                }
                else
                {
                    int dx = Math.Abs(node.X - goal.X);
                    int dy = Math.Abs(node.Y - goal.Y);
                    return await new ValueTask<double>(dx + dy);
                }
            }
        });
    }

    private async ValueTask<double> MovementCost(Node firstNode, Node secondNode)
    {
        if (IsDiagonalEnabled)
        {
            int dx = Math.Abs(firstNode.X - secondNode.X);
            int dy = Math.Abs(firstNode.Y - secondNode.Y);
            return await new ValueTask<double>(Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
        }
        else
        {
            int dx = Math.Abs(firstNode.X - secondNode.X);
            int dy = Math.Abs(firstNode.Y - secondNode.Y);
            return await new ValueTask<double>(dx - dy);
        }
    }

    private async Task FindPathAsync()
    {
        await Task.Run(() =>
        {
            Path.Clear();
            Node? temp = Current;
            Path.Add(temp);
            while (temp.CameFrom is not null)
            {
                Path.Add(temp.CameFrom);
                temp = temp.CameFrom;
            }
        });
    }

    private async Task CalculatePathValueAsync()
    {
        await Task.Run(() =>
        {
            PathScore pathScore = new();
            var score = 0.0;
            foreach (var node in Path)
            {
                score += node.F;
            }
            pathScore.Length = Path.Count;
            pathScore.Score = score;
            pathScore.Visited = CloseSet.Count;
            PathScore.Add(pathScore);
        });
    }

    #endregion A* search algorithm Code Section

    #region WeatherConditions Code Section

    private async Task AddWeatherConditionAsync()
    {
        await Task.Run(async () =>
        {
            foreach (var node in NodeMap)
            {
                if (!node.IsWall && !node.IsRoad)
                {
                    node.Condition = await RandomEnumValue();
                }
            }
            await GetNodesAsync();
        });
    }

    private async Task RemoveWeatherConditionAsync()
    {
        await Task.Run(async () =>
        {
            foreach (var node in NodeMap)
            {
                if (!node.IsWall && !node.IsRoad)
                {
                    node.Condition = Enum.ExtraCondition.Clear;
                }
            }
            await GetNodesAsync();
        });
    }

    private static async Task<Enum.ExtraCondition> RandomEnumValue()
    {
        Dictionary<Enum.ExtraCondition, float> condition = new();
        condition.Add(Enum.ExtraCondition.Sunny, 0.8f);
        condition.Add(Enum.ExtraCondition.Cloudy, 0.3f);
        condition.Add(Enum.ExtraCondition.Hail, 0.2f);
        condition.Add(Enum.ExtraCondition.Rainy, 0.3f);
        condition.Add(Enum.ExtraCondition.HeavyRain, 0.2f);
        condition.Add(Enum.ExtraCondition.Lightning, 0.1f);
        condition.Add(Enum.ExtraCondition.LightningRainy, 0.1f);
        condition.RandomElementByWeight(e => e.Value);
        await Task.Delay(0);
        return condition.RandomElementByWeight(e => e.Value).Key;
    }

    #endregion WeatherConditions Code Section

    #region Maze Code Section

    private async Task AddMazeOuterWallsAsync()
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < X; ++i)
            {
                if (i == 0 || i == X - 1)
                {
                    for (int j = 0; j < Y; ++j)
                    {
                        NodeMap[i, j].Set = Enum.AStarSet.Wall;
                        NodeMap[i, j].IsWall = true;
                        NodeMap[i, j].Condition = Enum.ExtraCondition.Clear;
                    }
                }
                else
                {
                    NodeMap[i, 0].Set = Enum.AStarSet.Wall;
                    NodeMap[i, 0].IsWall = true;
                    NodeMap[i, 0].Condition = Enum.ExtraCondition.Clear;
                    NodeMap[i, Y - 1].Set = Enum.AStarSet.Wall;
                    NodeMap[i, Y - 1].IsWall = true;
                    NodeMap[i, Y - 1].Condition = Enum.ExtraCondition.Clear;
                }
            }
        });
    }

    private async Task AddMazeInnerWallsAsync(bool h, int minX, int maxX, int minY, int maxY, Point gate)
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
            }
            else
            {
                if (maxY - minY < 2)
                {
                    return;
                }
                var x = Math.Floor(await RandomNumberAsync(minX, maxX) / 2) * 2;
                await Task.WhenAll(AddVWallAsync(minY, maxY, (int)x),
                                   AddMazeInnerWallsAsync(!h, minX, (int)x - 1, minY, maxY, gate),
                                   AddMazeInnerWallsAsync(!h, (int)x + 1, maxX, minY, maxY, gate));
            }
        });
    }

    private async Task AddVWallAsync(int minY, int maxY, int x)
    {
        await Task.Run(async () =>
        {
            decimal hole = (Math.Floor(await RandomNumberAsync(minY, maxY) / 2) * 2) + 1;
            for (int i = minY; i <= maxY; ++i)
            {
                if (i == hole)
                {
                    NodeMap[i, x].Set = Enum.AStarSet.Undefined;
                    NodeMap[i, x].IsWall = false;
                }
                else
                {
                    NodeMap[i, x].Set = Enum.AStarSet.Wall;
                    NodeMap[i, x].IsWall = true;
                    NodeMap[i, x].Condition = Enum.ExtraCondition.Clear;
                }
            }
        });
    }

    private async Task AddHWallAsync(int minX, int maxX, int y)
    {
        await Task.Run(async () =>
        {
            decimal hole = (Math.Floor(await RandomNumberAsync(minX, maxX) / 2) * 2) + 1;
            for (int i = minX; i <= maxX; ++i)
            {
                if (i == hole)
                {
                    NodeMap[y, i].Set = Enum.AStarSet.Undefined;
                    NodeMap[y, i].IsWall = false;
                }
                else
                {
                    NodeMap[y, i].Set = Enum.AStarSet.Wall;
                    NodeMap[y, i].IsWall = true;
                    NodeMap[y, i].Condition = Enum.ExtraCondition.Clear;
                }
            }
        });
    }

    private static async ValueTask<decimal> RandomNumberAsync(int min, int max)
    {
        Random? rnd = new();
        return await new ValueTask<decimal>((decimal)Math.Floor((rnd.NextDouble() * (max - min + 1)) + min));
    }

    private async Task WallToRoadAsync()
    {
        await Task.Run(async () =>
        {
            foreach (var node in NodeMap)
            {
                if (node.IsWall)
                {
                    node.IsWall = false;
                    node.IsRoad = true;
                    node.Set = Enum.AStarSet.Road;
                    node.Condition = Enum.ExtraCondition.Road;
                }
            }
            for (int i = 0; i < X; ++i)
            {
                if (i == 0 || i == X - 1)
                {
                    for (int j = 0; j < Y; ++j)
                    {
                        NodeMap[i, j].Set = Enum.AStarSet.Wall;
                        NodeMap[i, j].IsWall = true;
                        NodeMap[i, j].Condition = Enum.ExtraCondition.Clear;
                    }
                }
                else
                {
                    NodeMap[i, 0].Set = Enum.AStarSet.Wall;
                    NodeMap[i, 0].IsWall = true;
                    NodeMap[i, 0].Condition = Enum.ExtraCondition.Clear;
                    NodeMap[i, Y - 1].Set = Enum.AStarSet.Wall;
                    NodeMap[i, Y - 1].IsWall = true;
                    NodeMap[i, Y - 1].Condition = Enum.ExtraCondition.Clear;
                }
            }
            await GetNodesAsync();
        });
    }

    private async Task AddMazeAsync()
    {
        await Task.WhenAll(AddMazeOuterWallsAsync(), AddMazeInnerWallsAsync(true, 1, Y - 2, 1, X - 2, new Point(X - 2, Y - 2)));
        await GetNodesAsync();
    }

    private async Task RemoveMazeAsync()
    {
        await Task.Run(async () =>
        {
            foreach (var node in NodeMap)
            {
                if (node.IsWall || node.IsRoad)
                {
                    node.Set = Enum.AStarSet.Undefined;
                    node.IsWall = false;
                    node.IsRoad = false;
                }
            }
            await GetNodesAsync();
        });
    }

    #endregion Maze Code Section

    #region Visualization Code Section

    private async Task CheckOpenSetCloseSetPathAsync()
    {
        await Task.Run(() =>
        {
            foreach (Node node in NodeMap)
            {
                if (OpenSet.Contains(node) && node != StartNode && node != GoalNode)
                {
                    node.Set = Enum.AStarSet.Open;
                }
                if (CloseSet.Contains(node) && node != StartNode && node != GoalNode)
                {
                    node.Set = Enum.AStarSet.Closed;
                }
                if (Path.Contains(node) && node != StartNode && node != GoalNode)
                {
                    node.Set = Enum.AStarSet.Path;
                }
            }
        });
    }

    private async Task ClearNodeMapAsync()
    {
        await Task.Run(() =>
        {
            foreach (var node in NodeMap)
            {
                switch (node.Set)
                {
                    case Enum.AStarSet.Closed:
                    case Enum.AStarSet.Open:
                    case Enum.AStarSet.Path:
                        node.Set = Enum.AStarSet.Undefined;
                        break;
                }
            }
        });
    }

    private async Task ClearNodeNeighbors()
    {
        await Task.Run(() =>
        {
            foreach (var node in NodeMap)
            {
                node.Neighbors.Clear();
            }
        });
    }

    private async Task GetNodesAsync()
    {
        await Task.Run(() =>
        {
            foreach (var node in NodeMap)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Nodes.Add(node);
                    OnPropertyChanged(nameof(Nodes));
                });
            }
        });
    }

    #endregion Visualization Code Section
}