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
        //ViewModelApplication.Snackbar = MainSnackbar;
        MainSnackbar.MessageQueue.Enqueue("Welcome to my simple project to visualize A* pathfinding algorithm");
    }
}
