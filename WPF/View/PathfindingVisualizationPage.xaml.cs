namespace WPF.View;

/// <summary>
/// Interaction logic for PathfindingVisualizationPage.xaml
/// </summary>
public partial class PathfindingVisualizationPage : UserControl
{
    public PathfindingVisualizationPage()
    {
        InitializeComponent();
        DataContext = new AStarAlgorithmViewModel();
    }
}
