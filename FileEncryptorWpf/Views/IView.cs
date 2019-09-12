namespace FileEncryptorWpf.Views
{
    /// <summary>
    /// Defines the <see cref="IWindow" /> interface. This interface should be implemented by every window which can change content to some <see cref="UserControl"/>.
    /// </summary>
    public interface IWindow
    {
        void ChangeCurrentControlTo(object x);
    }
}