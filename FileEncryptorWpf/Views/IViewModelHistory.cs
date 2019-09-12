namespace FileEncryptorWpf.Views
{
    /// <summary>
    /// Defines the <see cref="IViewModelHistory" /> interface. This interface should be implemented by every window which can change content to some <see cref="UserControl"/>.
    /// </summary>
    public interface IViewModelHistory
    {
        void GoToViewModel(object x);
        void GoToPreviousViewModel();
    }
}