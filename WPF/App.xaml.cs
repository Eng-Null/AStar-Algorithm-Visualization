namespace WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Setup the main application
        // await ApplicationSetupAsync();

        Current.MainWindow = new MainWindow();
        Current.MainWindow.Show();
    }

    //private async Task ApplicationSetupAsync()
    //{
    //    // Setup the Dna Framework
    //    //Framework.Construct<DefaultFrameworkConstruction>()
    //    //    .AddFileLogger()
    //    //    .AddViewModels()
    //    //    .AddClientServices()
    //    //    .AddSQLClientServices()
    //    //    .Build();

    //    await Task.Delay(1);
    //}
}