namespace WPF;

using static WPF.StaticValues;

public static class AstarAlgorithm
{
    public static async Task<Node[,]> CalculateNodesAsync(Node[,] NodeMap)
    {
        return await Task.Run(() =>
        {
            NodeMap = new Node[X, Y];

            for (int i = 0; i < X; i++)
            {
                for (int j = 0; j < Y; j++)
                {
                    NodeMap[i,j] = new Node
                    {
                        X = i,
                        Y = j,
                    };
                }
            }
            return NodeMap;
        });
    }

    public static async Task ResetValuesAsync(Node[,] NodeMap)
    {
        await Task.Run(() =>
        {
            foreach (Node node in NodeMap)
            {
                node.G = double.PositiveInfinity;
                node.F = double.PositiveInfinity;
            }
            return NodeMap;
        });
    }

    public static async Task GetNeighborsAsync(Node[,] NodeMap, bool IsDiagonalEnabled)
    {
        await Task.Run(() =>
        {
            Parallel.For(0, X, i =>
            {
                Parallel.For(0, Y, j =>
                {
                    if (i < X - 1)
                    {
                        NodeMap[i,j].Neighbors.Add(NodeMap[i + 1,j]);
                    }

                    if (i > 0)
                    {
                        NodeMap[i,j].Neighbors.Add(NodeMap[i - 1,j]);
                    }

                    if (j < Y - 1)
                    {
                        NodeMap[i,j].Neighbors.Add(NodeMap[i,j + 1]);
                    }

                    if (j > 0)
                    {
                        NodeMap[i,j].Neighbors.Add(NodeMap[i,j - 1]);
                    }
                    if (IsDiagonalEnabled)
                    {
                        // diagonal Pathfinding
                        if (i > 0 && j > 0)
                        {
                            NodeMap[i,j].Neighbors.Add(NodeMap[i - 1,j - 1]);
                        }

                        if (i < X - 1 && j > 0)
                        {
                            NodeMap[i,j].Neighbors.Add(NodeMap[i + 1,j - 1]);
                        }

                        if (i > 0 && j < Y - 1)
                        {
                            NodeMap[i,j].Neighbors.Add(NodeMap[i - 1,j + 1]);
                        }

                        if (i < X - 1 && j < Y - 1)
                        {
                            NodeMap[i,j].Neighbors.Add(NodeMap[i + 1,j + 1]);
                        }
                    }
                });
            });
        });
    }

    public static async ValueTask<double> ManhattanDistance(Node firstNode, Node secondNode)
    {
        int dx = Math.Abs(firstNode.X - secondNode.X);
        int dy = Math.Abs(firstNode.Y - secondNode.Y);
        return await new ValueTask<double>(dx + dy);
    }

    public static async ValueTask<double> EuclideanDistance(Node firstNode, Node secondNode)
    {
        int dx = Math.Abs(firstNode.X - secondNode.X);
        int dy = Math.Abs(firstNode.Y - secondNode.Y);
        return await new ValueTask<double>(Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
    }

    public static async ValueTask<double> MovementCost(Node firstNode, Node secondNode, bool IsDiagonalEnabled, bool DistanceType, bool IsConditionEnabled)
    {
        return IsDiagonalEnabled ?
            await EuclideanDistance(firstNode, secondNode) + (IsConditionEnabled ? (int)secondNode.Condition : 0) :
            DistanceType ?
            await EuclideanDistance(firstNode, secondNode) + (IsConditionEnabled ? (int)secondNode.Condition : 0) :
            await ManhattanDistance(firstNode, secondNode) + (IsConditionEnabled ? (int)secondNode.Condition : 0);
    }
}