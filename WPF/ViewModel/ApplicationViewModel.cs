using WPF.MVVMHelpers;

namespace WPF.ViewModel;
public class ApplicationViewModel : ApplicationBaseViewModel<ApplicationPage>
{
    public string ApplicationTitle { get; set; }
    public ApplicationViewModel()
    {
        ApplicationTitle = "Home Page";
        CurrentPage = ApplicationPage.Home;
        mWindow = (MainWindow)Application.Current.MainWindow;
        WindowResizer();
    }
}
