using MaterialDesignThemes.Wpf;
using System.Diagnostics;
using System.Threading;
using System.Windows.Media.Imaging;
using static WPF.AstarAlgorithm;
using static WPF.DataTransaction;
using static WPF.MazeManagement;
using static WPF.StaticValues;
using static WPF.StreetManagement;
using static WPF.WeatherCondition;
using static WPF.NodeMapDrawing;

namespace WPF.ViewModel;
//https://en.wikipedia.org/wiki/A*_search_algorithm
//https://en.wikipedia.org/wiki/Maze_generation_algorithm

public class AStarAlgorithmViewModel : BaseViewModel
{
    #region Algorithem Constructors

    public Node[,] NodeMap { get; set; }
    public WriteableBitmap NodeMapImage { get; set; }
    public List<Node> OpenSet { get; set; } = new();
    public List<Node> CloseSet { get; set; } = new();
    public List<Node> Path { get; set; } = new();
    public Node StartNode { get; set; } = new();
    public Node GoalNode { get; set; } = new();
    public Node Current { get; set; } = new();
    public bool DistanceType { get; set; }
    public bool IsDiagonalEnabled { get; set; }
    public bool IsConditionEnabled { get; set; }
    public string PathData { get; set; } = string.Empty;
    public int AlgorithemType { get; set; } = 1;
    public int EditNodeType { get; set; } = 0;

    #endregion Algorithem Constructors

    #region Visualization Constructors

    public DelegateCommand StartCommand { get; set; }
    private new readonly PauseTokenSource PauseTokenSource = new();
    public DelegateCommand PauseCommand { get; set; }
    public DelegateCommand CalculateCommand { get; set; }
    public DelegateCommand WallToRoadCommand { get; set; }
    public DelegateCommand SetConditionCommand { get; set; }
    public DelegateCommand RemoveConditionCommand { get; set; }
    public DelegateCommand AddMazeCommand { get; set; }
    public DelegateCommand RemoveMazeCommand { get; set; }
    public DelegateCommand SaveCommand { get; set; }
    public DelegateCommand LoadCommand { get; set; }
    public DelegateCommand<Node> EditNodeMapCommand { get; set; }

    //public DelegateCommand GetNodeAxis { get; set; }
    private bool CanMaze() => NodeMap[0, 0] != null;

    private bool CanCondition() => NodeMap[0, 0] != null;

    private bool StartIsRunning { get; set; }

    private bool CanStart() => (NodeMap[0, 0] != null && !StartIsRunning) || PauseTokenSource.IsPaused().Result;

    private bool CanPause() => NodeMap[0, 0] != null && !CanStart();

    public ObservableCollection<Node> Nodes { get; set; } = new();
    public ObservableCollection<PathScore> PathScore { get; set; } = new();

    public PathScore SelectedPathScore
    {
        get => selectedPathScore;
        set
        {
            selectedPathScore = value;
            NodeMap = value.NodeMap;
            OpenSet = value.OpenSet;
            CloseSet = value.CloseSet;
            PathData = value.Path;
        }
    }

    public int Delay { get; set; } = 5;
    private List<long> StepTimeSpan { get; set; }
    private Stopwatch Watch = new();
    private PathScore selectedPathScore = new();
    private readonly object _Nodeslock = new();
    private readonly object _PathScorelock = new();
    private bool _IsClearMap;

    #endregion Visualization Constructors

    public AStarAlgorithmViewModel()
    {
        StartCommand = new DelegateCommand(async () => await StartAsync(PauseTokenSource.Token, CancellationToken.None), CanStart);
        PauseCommand = new DelegateCommand(async () => await PauseAsync(CancellationToken.None), CanPause);

        CalculateCommand = new DelegateCommand(async () =>
        {
            await DialogHost.Show(new NewMapControl { }, "AStarDialog");
            switch (MapType)
            {
                case 0:
                    NodeMap = await CalculateNodesAsync(NodeMap);
                    await ClearSetNodeMapAsync();
                    _IsClearMap = true;
                    //await SetStartEndPoint();
                    //NodeMapImage = await DrawNodeMap(X,Y,NodeMap);                
                    PathData = string.Empty;
                    break;

                case 1:
                    NodeMap = await CalculateNodesAsync(NodeMap);
                    await AddMazeAsync(NodeMap);
                    await ClearSetNodeMapAsync();
                    _IsClearMap = true;
                    //await SetStartEndPoint();
                    //NodeMapImage = await DrawNodeMap(X, Y, NodeMap);
                    PathData = string.Empty;
                    break;

                case 2:
                    NodeMap = await CalculateNodesAsync(NodeMap);
                    await AddStreetAsync(NodeMap);
                    _IsClearMap = false;
                    //await SetStartEndPoint();
                    //NodeMapImage = await DrawNodeMap(X, Y, NodeMap);
                    PathData = string.Empty;
                    break;
            }
            await SetStartEndPoint();
            await GetNodesAsync();
        });

        WallToRoadCommand = new DelegateCommand(async () => await AddStreetAsync(NodeMap), CanMaze);

        SetConditionCommand = new DelegateCommand(async () => NodeMap = await AddWeatherConditionAsync(NodeMap), CanCondition);
        RemoveConditionCommand = new DelegateCommand(async () => await RemoveWeatherConditionAsync(NodeMap), CanCondition);

        AddMazeCommand = new DelegateCommand(async () => await AddMazeAsync(NodeMap), CanMaze);
        RemoveMazeCommand = new DelegateCommand(async () => await RemoveMazeAsync(NodeMap), CanMaze);

        SaveCommand = new DelegateCommand(async () => await SaveAsync(NodeMap), CanMaze);
        LoadCommand = new DelegateCommand(async () =>
        {
            var temp = await LoadAsync();
            if (temp is not null)
            {
                NodeMap = temp;
                if (NodeMap[0, 0].IsObstacle && NodeMap[0, 0].Style == AStarSet.RiverTL)
                {
                    _IsClearMap = false;
                }                 
                await GetNodesAsync();
                PathData = string.Empty;
            }
        });

        EditNodeMapCommand = new DelegateCommand<Node>(async (o) => await EditNodeMapAsync(o));
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
            NodeMap[StartPointX, StartPointY].Style = _IsClearMap ? AStarSet.EmptyStart : AStarSet.Start;
            NodeMap[StartPointX, StartPointY].IsObstacle = false;
            NodeMap[StartPointX, StartPointY].G = 0;
            NodeMap[StartPointX, StartPointY].F = 0;
            NodeMap[StartPointX, StartPointY].Condition = ExtraCondition.Road;

            NodeMap[EndPointX, EndPointY].Style = _IsClearMap ? AStarSet.EmptyEnd : AStarSet.End;
            NodeMap[EndPointX, EndPointY].IsObstacle = false;
            NodeMap[EndPointX, EndPointY].Condition = ExtraCondition.Road;

            StartNode = NodeMap[StartPointX, StartPointY];
            GoalNode = NodeMap[EndPointX, EndPointY];
        });
    }

    private async Task EditNodeMapAsync(Node o)
    {
        switch (EditNodeType)
        {
            case 0:
                if (o.IsObstacle)
                {
                    o.Style = AStarSet.EmptyGround;
                    o.IsObstacle = false;
                    break;
                }
                o.Style = AStarSet.EmptyObstacle;
                o.IsObstacle = true;
                break;

            case 1:

                NodeMap[StartPointX, StartPointY].Style = AStarSet.EmptyGround;
                NodeMap[StartPointX, StartPointY].IsObstacle = false;
                NodeMap[StartPointX, StartPointY].Condition = ExtraCondition.Clear;
                NodeMap[StartPointX, StartPointY].G = double.PositiveInfinity;
                NodeMap[StartPointX, StartPointY].F = double.PositiveInfinity;

                StartPointX = o.X;
                StartPointY = o.Y;
                await SetStartEndPoint();
                break;

            case 2:

                NodeMap[EndPointX, EndPointY].Style = AStarSet.EmptyGround;
                NodeMap[EndPointX, EndPointY].IsObstacle = false;
                NodeMap[EndPointX, EndPointY].Condition = ExtraCondition.Clear;

                EndPointX = o.X;
                EndPointY = o.Y;
                await SetStartEndPoint();
                break;
        }
    }

    private async Task StartAsync(PauseToken pause, CancellationToken token)
    {
        if (await PauseTokenSource.IsPaused())
        {
            await PauseTokenSource.ResumeAsync();
            return;
        }

        await RunCommandAsync(() => StartIsRunning, async () =>
        {
            await ClearNodeNeighbors();
            await GetNeighborsAsync(NodeMap, IsDiagonalEnabled);
            await ResetValuesAsync(NodeMap);
            await SetStartEndPoint();
            await Task.WhenAll(ComputeHeuristicCosts(1), ClearNodeMapAsync());

            PathData = "";
            OpenSet.Clear();
            CloseSet.Clear();
            StepTimeSpan = new();

            // The set of discovered nodes that may need to be (re-)expanded.
            // Initially, only the start node is known.
            // This is usually implemented as a min-heap or priority queue rather than a hash-set.
            // openSet:= {start}
            OpenSet.Add(StartNode);
            while (OpenSet.Count > 0)
            {
                token.ThrowIfCancellationRequested();
                await pause.PauseIfRequestedAsync();

                Watch = Stopwatch.StartNew();
                try
                {
                    // This operation can occur in O(1) time if openSet is a min-heap or a priority queue
                    // current:= the node in openSet having the lowest fScore[] value
                    switch (AlgorithemType)
                    {
                        // BFS
                        case 2:
                            await CheckForLowestHAsync();
                            break;
                        // A* and Dijkstra's But Dijkstra's F here is always infinity so it checks everything
                        default:
                            await CheckForLowestFAsync();
                            break;
                    }

                    // if current = goal
                    // return reconstruct_path(cameFrom, current)
                    if (Current == GoalNode)
                    {
                        await FindPathAsync();
                        await CheckOpenSetCloseSetPathAsync();

                        Watch.Stop();
                        StepTimeSpan.Add(Watch.ElapsedTicks);

                        await CalculatePathValueAsync();
                        return;
                    }
                    //TODO/NOTETOSELF: UPDATE NODEMAP instead of NODES and update NODES Value AT the END of each Loop
                    // openSet.Remove(current)
                    OpenSet.Remove(Current);
                    // for each neighbor of current
                    _ = Parallel.ForEach(Current.Neighbors, async neighbor =>
                       {
                           if (!CloseSet.Contains(neighbor) && !neighbor.IsObstacle)
                           {
                               // tentative_gScore is the distance from start to the neighbor through current and in this example the distance between current and neighbor is 1
                               // tentative_gScore:= gScore[current] + d(current, neighbor)
                               var tentativeGScore = Current.G + await MovementCost(Current, neighbor, IsDiagonalEnabled, DistanceType, IsConditionEnabled);

                               if (tentativeGScore < neighbor.G)
                               {
                                   // This path to neighbor is better than any previous one. Record it!
                                   neighbor.CameFrom = Current;
                                   neighbor.G = Math.Round(tentativeGScore, 2);
                                   switch (AlgorithemType)
                                   {
                                       case 0:
                                           neighbor.F = double.PositiveInfinity;
                                           break;

                                       case 1:
                                           //http://theory.stanford.edu/~amitp/GameProgramming/Variations.html
                                           //f(p) = g(p) + w * h(p)
                                           neighbor.F = Math.Round(tentativeGScore + neighbor.H, 2); //*(IsConditionEnabled ? (int)neighbor.Condition : 1)
                                           break;
                                   }
                                   if (!OpenSet.Contains(neighbor))
                                   {
                                       OpenSet.Add(neighbor);
                                   }
                               }
                           }
                           await CheckOpenSetCloseSetPathAsync();
                           await FindPathAsync();
                       });
                }
                finally
                {
                    Watch.Stop();
                    StepTimeSpan.Add(Watch.ElapsedTicks);

                    await Task.Delay(Delay);
                }
            }
        });
    }

    private async Task PauseAsync(CancellationToken token)
    {
        await PauseTokenSource.PauseAsync();
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

    private async Task CheckForLowestHAsync()
    {
        await Task.Run(() =>
        {
            Current = OpenSet.OrderBy(x => x.H).ToList().First();

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
            Path.Clear();
            PathData = "";
            Node? temp = Current;
            Path.Add(Current);

            //Drawing the path
            var middle = TileSize / 2;
            while (temp.CameFrom is not null)
            {
                PathData += $"M {temp.X * TileSize + middle},{temp.Y * TileSize + middle} {temp.CameFrom.X * TileSize + middle},{temp.CameFrom.Y * TileSize + middle} ";
                Path.Add(temp.CameFrom);
                temp = temp.CameFrom;
            }
        });
    }

    private async Task CalculatePathValueAsync()
    {
        await Task.Run(() =>
        {
            PathScore pathScore = new()
            {
                Length = Path.Count,
                Visited = CloseSet.Count,
                Time = TimeSpan.FromTicks((long)(StepTimeSpan.Average() * CloseSet.Count)),
                Path = PathData,
                OpenSet = OpenSet,
                CloseSet = CloseSet,
                NodeMap = NodeMap
            };

            //TODO: Calculate Score Value
            //var score = 0.0;

            //Parallel.ForEach(Path, node =>
            //{
            //    score += node.F;
            //});

            //pathScore.Score = score;

            switch (AlgorithemType)
            {
                case 0:
                    pathScore.Algorithm = "Dijkstra's Algorithm";
                    if (IsConditionEnabled)
                    {
                        pathScore.Algorithm += " - Conditions";
                    }
                    if (IsDiagonalEnabled)
                    {
                        pathScore.Algorithm += " - Diagonal";
                        pathScore.Algorithm += " - Euclidean Distance";
                        break;
                    }
                    if (DistanceType)
                    {
                        pathScore.Algorithm += " - Euclidean Distance";
                        break;
                    }
                    pathScore.Algorithm += " - Manhattan Distance";
                    break;

                case 1:
                    pathScore.Algorithm = "A* Algorithm";
                    if (IsConditionEnabled)
                    {
                        pathScore.Algorithm += " - Conditions";
                    }
                    if (IsDiagonalEnabled)
                    {
                        pathScore.Algorithm += " - Diagonal";
                        pathScore.Algorithm += " - Euclidean Distance";
                        break;
                    }
                    if (DistanceType)
                    {
                        pathScore.Algorithm += " - Euclidean Distance";
                        break;
                    }
                    pathScore.Algorithm += " - Manhattan Distance";
                    break;

                case 2:
                    pathScore.Algorithm = "Best-First Search";
                    if (IsConditionEnabled)
                    {
                        pathScore.Algorithm += " - Conditions";
                    }
                    if (IsDiagonalEnabled)
                    {
                        pathScore.Algorithm += " - Diagonal";
                        pathScore.Algorithm += " - Euclidean Distance";
                        break;
                    }
                    if (DistanceType)
                    {
                        pathScore.Algorithm += " - Euclidean Distance";
                        break;
                    }
                    pathScore.Algorithm += " - Manhattan Distance";
                    break;
            }
            PathScore.Add(pathScore);
        });
    }

    #endregion A* search algorithm Code Section

    #region Visualization Code Section

    private async Task CheckOpenSetCloseSetPathAsync()
    {
        await Task.Run(() =>
        {
            foreach (Node node in NodeMap)
            {
                if (CloseSet.Contains(node) && node != StartNode && node != GoalNode)
                {
                    node.Set = Set.Closed;
                    continue;
                }
                if (OpenSet.Contains(node) && node != StartNode && node != GoalNode)
                {
                    node.Set = Set.Open;
                    continue;
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
                    case Set.Closed:
                    case Set.Open:
                        node.Set = Set.Undefined;
                        break;
                }
            }
        });
    }

    private async Task ClearSetNodeMapAsync()
    {
        await Task.Run(() =>
        {
            foreach (var node in NodeMap)
            {
                if (node.Style is not AStarSet.Maze)
                {
                    node.Style = AStarSet.EmptyGround;
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
                node.CameFrom = null;
            }
        });
    }

    private async Task GetNodesAsync()
    {
        await Task.Run(() =>
        {
            Nodes.Clear();
            foreach (var node in NodeMap)
            {
                Nodes.Add(node);
            }
        });
    }

    #endregion Visualization Code Section
}