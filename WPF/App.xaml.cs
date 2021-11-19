namespace WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {

        base.OnStartup(e);

        // Setup the main application
        await ApplicationSetupAsync();

        try
        {
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();
        }
        catch (Exception ex)
        {
            //Logger.LogInformationSource($"{ex.Message} : {ex.StackTrace}");
        }
    }

    private async Task ApplicationSetupAsync()
    {
        // Setup the Dna Framework
        //Framework.Construct<DefaultFrameworkConstruction>()
        //    .AddFileLogger()
        //    .AddViewModels()
        //    .AddClientServices()
        //    .AddSQLClientServices()
        //    .Build();

        await Task.Delay(1);
    }

}
