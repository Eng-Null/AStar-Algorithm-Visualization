namespace WPF.Model;

public class PathScore : BaseViewModel
{
    public TimeSpan Time { get; set; }
    public int Length { get; set; }
    public double Score { get; set; }
    public int Visited { get; set; }
    public string Algorithm { get; set; }
    public string Path { get; set; }
    public List<Node> OpenSet { get; set; }
    public List<Node> CloseSet { get; set; }

}
