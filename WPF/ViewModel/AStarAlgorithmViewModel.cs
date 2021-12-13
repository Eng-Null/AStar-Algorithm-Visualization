using MaterialDesignThemes.Wpf;
using System.Diagnostics;
using static WPF.AstarAlgorithm;
using static WPF.DataTransaction;
using static WPF.MazeManagement;
using static WPF.StaticValues;
using static WPF.StreetManagement;
using static WPF.WeatherCondition;

namespace WPF.ViewModel;
//https://en.wikipedia.org/wiki/A*_search_algorithm
//https://en.wikipedia.org/wiki/Maze_generation_algorithm

public class AStarAlgorithmViewModel : BaseViewModel
{
    #region Algorithem Constructors

    public Node[,] NodeMap { get; set; }
    public List<Node> OpenSet { get; set; } = new();
    public List<Node> CloseSet { get; set; } = new();
    public List<Node> Path { get; set; } = new();
    public Node StartNode { get; set; } = new();
    public Node GoalNode { get; set; } = new();
    public Node Current { get; set; } = new();
    public bool DistanceType { get; set; }
    public bool IsDiagonalEnabled { get; set; }
    public string PathData { get; set; } = string.Empty;

    #endregion Algorithem Constructors

    #region Visualization Constructors

    public DelegateCommand StartCommand { get; set; }
    public DelegateCommand CalculateCommand { get; set; }
    public DelegateCommand WallToRoadCommand { get; set; }
    public DelegateCommand SetConditionCommand { get; set; }
    public DelegateCommand RemoveConditionCommand { get; set; }
    public DelegateCommand AddMazeCommand { get; set; }
    public DelegateCommand RemoveMazeCommand { get; set; }
    public DelegateCommand SaveCommand { get; set; }
    public DelegateCommand LoadCommand { get; set; }

    //public DelegateCommand GetNodeAxis { get; set; }
    private bool CanMaze() => NodeMap[0, 0] != null;

    private bool CanCondition() => NodeMap[0, 0] != null;

    private bool CanStart() => NodeMap[0, 0] != null;

    public ObservableCollection<Node> Nodes { get; set; } = new();
    public ObservableCollection<PathScore> PathScore { get; set; } = new();

    public int Delay { get; set; } = 20;
    private TimeSpan TimeSpan { get; set; }
    private TimeSpan ExcludedTime { get; set; }
    private TimeSpan TotalDelay { get; set; }
    private Stopwatch Watch = new();
    private readonly object _Nodeslock = new();
    private readonly object _PathScorelock = new();

    #endregion Visualization Constructors

    public AStarAlgorithmViewModel()
    {
        StartCommand = new DelegateCommand(async () => await StartAsync(), CanStart);
        CalculateCommand = new DelegateCommand(async () =>
        {
            await DialogHost.Show(new NewMapControl { }, "AStarDialog");
            switch (MapType)
            {
                case 0:
                    NodeMap = await CalculateNodesAsync(NodeMap, X, Y);
                    break;

                case 1:
                    NodeMap = await CalculateNodesAsync(NodeMap, X, Y);
                    await AddMazeAsync(X, Y, NodeMap);
                    break;

                case 2:
                    NodeMap = await CalculateNodesAsync(NodeMap, X, Y);
                    await AddStreetAsync(X, Y, NodeMap);
                    break;
            }
            await SetStartEndPoint();
            await GetNodesAsync();
        });

        WallToRoadCommand = new DelegateCommand(async () => await AddStreetAsync(X, Y, NodeMap), CanMaze);

        SetConditionCommand = new DelegateCommand(async () => NodeMap = await AddWeatherConditionAsync(NodeMap), CanCondition);
        RemoveConditionCommand = new DelegateCommand(async () => await RemoveWeatherConditionAsync(NodeMap), CanCondition);

        AddMazeCommand = new DelegateCommand(async () => await AddMazeAsync(X, Y, NodeMap), CanMaze);
        RemoveMazeCommand = new DelegateCommand(async () => await RemoveMazeAsync(NodeMap), CanMaze);

        SaveCommand = new DelegateCommand(async () => await SaveAsync(NodeMap), CanMaze);
        LoadCommand = new DelegateCommand(async () =>
        {
            NodeMap = await LoadAsync();
            await GetNodesAsync();
        });
        //GetNodeAxis = new DelegateCommand(async () => await GetAxisAsync());
        NodeMap = new Node[X, Y];
        BindingOperations.EnableCollectionSynchronization(Nodes, _Nodeslock);
        BindingOperations.EnableCollectionSynchronization(PathScore, _PathScorelock);
    }

    #region A* search algorithm Code Section

    private async Task SetStartEndPoint()
    {
        await Task.Run(() =>
        {
            NodeMap[StartPointX, StartPointY].Style = AStarSet.Start;
            NodeMap[StartPointX, StartPointY].IsObstacle = false;
            NodeMap[StartPointX, StartPointY].Condition = ExtraCondition.Road;
            NodeMap[StartPointX, StartPointY].G = 0;

            NodeMap[EndPointX, EndPointY].Style = AStarSet.End;
            NodeMap[EndPointX, EndPointY].IsObstacle = false;
            NodeMap[EndPointX, EndPointY].Condition = ExtraCondition.Road;

            StartNode = NodeMap[StartPointX, StartPointY];
            GoalNode = NodeMap[EndPointX, EndPointY];
        });
    }

    private async Task StartAsync()
    {
        await Task.Run(async () =>
        {
            await ClearNodeNeighbors();
            await GetNeighborsAsync(NodeMap, X, Y, IsDiagonalEnabled);
            await ResetValuesAsync(NodeMap);
            await SetStartEndPoint();
            await Task.WhenAll(ComputeHeuristicCosts(1), ClearNodeMapAsync());

            PathData = "";
            OpenSet.Clear();
            CloseSet.Clear();
            ExcludedTime = TimeSpan.FromTicks(0);
            TotalDelay = TimeSpan.FromTicks(0);
            Watch = new();
            Watch.Start();
            // The set of discovered nodes that may need to be (re-)expanded.
            // Initially, only the start node is known.
            // This is usually implemented as a min-heap or priority queue rather than a hash-set.
            // openSet:= {start}
            OpenSet.Add(StartNode);
            while (OpenSet.Count > 0)
            {
                // This operation can occur in O(1) time if openSet is a min-heap or a priority queue
                // current:= the node in openSet having the lowest fScore[] value
                await CheckForLowestFAsync();

                // if current = goal
                // return reconstruct_path(cameFrom, current)
                if (Current == GoalNode)
                {
                    await FindPathAsync();
                    await CheckOpenSetCloseSetPathAsync();
                    Watch.Stop();
                    TimeSpan = Watch.Elapsed - ExcludedTime - TotalDelay;
                    await CalculatePathValueAsync();
                    return;
                }

                // openSet.Remove(current)
                OpenSet.Remove(Current);
                // for each neighbor of current
                foreach (Node neighbor in Current.Neighbors)
                {
                    if (!CloseSet.Contains(neighbor) && !neighbor.IsObstacle)
                    {
                        // tentative_gScore is the distance from start to the neighbor through current and in this example the distance between current and neighbor is 1
                        // tentative_gScore:= gScore[current] + d(current, neighbor)
                        var tentativeGScore = Current.G + await MovementCost(Current, neighbor, IsDiagonalEnabled, DistanceType);

                        if (tentativeGScore < neighbor.G)
                        {
                            // This path to neighbor is better than any previous one. Record it!
                            neighbor.CameFrom = Current;
                            neighbor.G = Math.Round(tentativeGScore, 2);
                            neighbor.F = Math.Round(tentativeGScore + neighbor.H, 2);
                            if (!OpenSet.Contains(neighbor))
                            {
                                OpenSet.Add(neighbor);
                            }
                        }
                    }
                    await CheckOpenSetCloseSetPathAsync();
                    await FindPathAsync();
                }
                TotalDelay += TimeSpan.FromMilliseconds(Delay);
                await Task.Delay(Delay);
            }
        });
    }

    private async Task CheckForLowestFAsync()
    {
        await Task.Run(() =>
        {
            Current = OpenSet.OrderBy(x => x.F).ToList().First();

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
            foreach (var node in NodeMap)
            {
                node.H = Math.Round(D * await ManhattanDistance(node, GoalNode), 2);
            }
        });
    }

    private async Task FindPathAsync()
    {
        await Task.Run(() =>
        {
            var watch = new Stopwatch();
            watch.Start();

            Path.Clear();
            PathData = "";
            Node? temp = Current;
            Path.Add(temp);
            var middle = TileSize / 2;

            while (temp.CameFrom is not null)
            {
                PathData += $"M {temp.X * TileSize + middle},{temp.Y * TileSize + middle} {temp.CameFrom.X * TileSize + middle},{temp.CameFrom.Y * TileSize + middle} ";
                Path.Add(temp.CameFrom);
                temp = temp.CameFrom;
            }

            watch.Stop();
            ExcludedTime += watch.Elapsed;
        });
    }

    private async Task CalculatePathValueAsync()
    {
        await Task.Run(() =>
        {
            PathScore pathScore = new();
            var score = 0.0;

            Parallel.ForEach(Path, node =>
            {
                score += node.F;
            });

            pathScore.Length = Path.Count;
            pathScore.Score = score;
            pathScore.Visited = CloseSet.Count;
            pathScore.Time = TimeSpan;
            PathScore.Add(pathScore);
        });
    }

    #endregion A* search algorithm Code Section

    #region Visualization Code Section

    private async Task CheckOpenSetCloseSetPathAsync()
    {
        await Task.Run(() =>
        {
            var watch = new Stopwatch();
            watch.Start();

            foreach (Node node in NodeMap)
            {
                if (OpenSet.Contains(node) && node != StartNode && node != GoalNode)
                {
                    node.Set = Set.Open;
                }
                if (CloseSet.Contains(node) && node != StartNode && node != GoalNode)
                {
                    node.Set = Set.Closed;
                }
            }

            watch.Stop();
            ExcludedTime += watch.Elapsed;
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
                    case Set.Closed:
                    case Set.Open:
                        node.Set = Set.Undefined;
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
            var temp = new ObservableCollection<Node>();
            foreach (var node in NodeMap)
            {
                temp.Add(node);
            }
            Nodes = temp;
        });
    }

    #endregion Visualization Code Section
}