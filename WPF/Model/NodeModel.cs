using WPF.Enum;
namespace WPF.Model;

public class Node : BaseViewModel
{
    public int X { get; set; }
    public int Y { get; set; }
    public double F { get; set; }
    public double G { get; set; }
    public double H { get; set; }
    public List<Node> Neighbors { get; set; } = new();
    public Node? CameFrom { get; set; }
    public bool IsWall { get; set; }
    public bool IsRoad { get; set; }
    public AStarSet Set { get; set; } = AStarSet.Undefined;
    public ExtraCondition Condition { get; set; } = ExtraCondition.Clear;

    public string ToolTip => $"Node: ({X},{Y}) \nG: {G}\nH: {H}\nF: {F}\nSet: {Set}\nCondition: {Condition}\nCameFrom: ({CameFrom?.X},{CameFrom?.Y})";

}


public class Coordinates : BaseViewModel
{
    public int X { get; set; }
    public int Y { get; set; }

}