using System.IO;
using System.Windows.Xps.Packaging;

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

        XpsDocument xpsDocument = new(Application.Current.StartupUri + @"Resources\PathfindingAlgorithms.xps", FileAccess.Read);
        myDocumentViewer.Document = xpsDocument.GetFixedDocumentSequence();
        
        webView2.Source = new Uri(@"https://github.com/Eng-RedWolf/AStar-Algorithm-Visualization/blob/3f5c4e484cee320a38ac2ec160b3c7b0606d0076/Path%20Planning%20Using%20A-star%20Algorithm.pdf");

        //ViewModelApplication.Snackbar = MainSnackbar;
        MainSnackbar.MessageQueue.Enqueue("Welcome to my simple project to visualize A* pathfinding algorithm");
    }
}