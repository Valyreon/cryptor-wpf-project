using System.Collections.Generic;

namespace FileEncryptorWpf.Views
{
    /// <summary>
    /// Defines the <see cref="IWindow" /> interface. This interface should be implemented by every window which can change content to some <see cref="UserControl"/>.
    /// </summary>
    public abstract class IWindow
    {
        protected Stack<object> ViewModelHistory { get; }
        public abstract void GoToViewModel(object x);
        public void GoToPreviousViewModel()
        {
            GoToViewModel(ViewModelHistory.Pop());
        }
    }
}