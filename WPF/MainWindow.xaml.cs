namespace WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new ApplicationViewModel();

        //Uri uri = new("file://" + @"C:\Users\redwo\source\repos\AStar Algorithm Visualization\WPF\Unity\index.html", UriKind.Absolute);
        //webView.Source = new Uri("http://localhost:9090/");

        //ViewModelApplication.Snackbar = MainSnackbar;
        MainSnackbar.MessageQueue.Enqueue("Welcome to my simple project to visualize A* pathfinding algorithm");
    }
}
