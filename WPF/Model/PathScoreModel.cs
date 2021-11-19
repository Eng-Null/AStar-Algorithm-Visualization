namespace WPF.Model;

public class PathScore : BaseViewModel
{
    public TimeOnly Time { get; set; }
    public int Length { get; set; }
    public double Score { get; set; }
    public int Visited { get; set; }

}
