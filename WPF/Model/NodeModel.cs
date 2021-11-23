using Newtonsoft.Json;
namespace WPF.Model;

public class Node : BaseViewModel
{
    public int X { get; set; }
    public int Y { get; set; }
    public double F { get; set; }
    public double G { get; set; }
    public double H { get; set; }
    [JsonIgnore]
    public List<Node> Neighbors { get; set; } = new();
    public Node? CameFrom { get; set; }
    public bool IsObstacle { get; set; }
    public bool IsRoad { get; set; }
    public bool Left { get; set; }
    public bool Right { get; set; }
    public bool Top { get; set; }
    public bool Bottom { get; set; }
    public AStarSet Style { get; set; } = AStarSet.Undefined;
    public Set Set { get; set; } = Set.Undefined;
    public ExtraCondition Condition { get; set; } = ExtraCondition.Clear;

    public string ToolTip => $"Node: ({X},{Y}) \nG: {G}\nH: {H}\nF: {F}\nCondition: {Condition}\nCameFrom: ({CameFrom?.X},{CameFrom?.Y})";
    // $"Left:{Left}\nRight:{Right}\nTop:{Top}\nBottom{Bottom}\n({X},{Y})";

}
