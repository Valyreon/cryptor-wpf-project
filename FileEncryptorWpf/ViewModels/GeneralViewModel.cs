using FileEncryptorWpf.Views;

namespace FileEncryptorWpf.ViewModels
{
    /// <summary>
    /// Defines the <see cref="GeneralViewModel" /> class.
    /// </summary>
    public class GeneralViewModel : ViewModelBase
    {
        private readonly IView thisWindow;

        private object currentControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralViewModel"/>.
        /// </summary>
        /// <param name="thisWindow">Window in which all the UserControls are to be shown in.</param>
        public GeneralViewModel(IView thisWindow)
        {
            this.thisWindow = thisWindow;
            this.CurrentControl = new LoginViewModel(this.thisWindow);
        }

        public object CurrentControl
        {
            get
            {
                return this.currentControl;
            }

            internal set
            {
                this.currentControl = value;
                this.RaisePropertyChangedEvent("CurrentControl");
            }
        }
    }
}