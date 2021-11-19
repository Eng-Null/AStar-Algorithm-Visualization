using WPF.ViewModel;

namespace WPF.MVVMHelpers;

public class ViewModelLocator
{
    #region Public Properties

    /// <summary>
    /// Singleton instance of the locator
    /// </summary>
    public static ViewModelLocator Instance { get; private set; } = new ViewModelLocator();

    /// <summary>
    /// The application view model
    /// </summary>
   // public ApplicationViewModel ApplicationViewModel => ViewModelApplication;

    #endregion Public Properties
}
