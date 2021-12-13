using Newtonsoft.Json;

namespace WPF.Model;

public class Node : BaseViewModel
{
    public int X { get; set; }
    public int Y { get; set; }

    [JsonIgnore]
    public double F { get; set; } = double.PositiveInfinity;

    [JsonIgnore]
    public double G { get; set; } = double.PositiveInfinity;

    [JsonIgnore]
    public double H { get; set; }

    [JsonIgnore]
    public List<Node> Neighbors { get; set; } = new();

    [JsonIgnore]
    public Node? CameFrom { get; set; }

    public bool IsObstacle { get; set; }
    public bool IsRoad { get; set; }
    public bool Left { get; set; }
    public bool Right { get; set; }
    public bool Top { get; set; }
    public bool Bottom { get; set; }
    public AStarSet Style { get; set; } = AStarSet.Undefined;

    [JsonIgnore]
    public Set Set { get; set; } = Set.Undefined;

    public ExtraCondition Condition { get; set; } = ExtraCondition.Clear;

    [JsonIgnore]
    public string ToolTip => $"Node: ({X},{Y}) \n" +
        $"G: {G} [Distance from Start to the NEIGHBOR through CURRENT]\n" +
        $"H: {H} [h(n) Estimates the cost to reach GOAL from node n]\n" +
        $"F: {F} [fScore[n] := gScore[n] + h(n).]\n" +
        $"Condition: {Condition}\n" +
        $"CameFrom: ({CameFrom?.X},{CameFrom?.Y})";
}