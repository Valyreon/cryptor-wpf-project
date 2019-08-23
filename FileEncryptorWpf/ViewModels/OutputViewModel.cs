using System.Windows.Input;
using FileEncryptorWpf.Views;

namespace FileEncryptorWpf.ViewModels
{
    /// <summary>
    /// Defines the <see cref="OutputViewModel" /> class.
    /// </summary>
    public class OutputViewModel : ViewModelBase
    {
        private readonly IView thisWindow;
        private readonly object backViewModel;
        private string outputText;
        private bool isBackEnabled;
        private int currentOperationProgress;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputViewModel"/> class.
        /// </summary>
        /// <param name="thisWindow">Window in which LoginControl is shown.</param>
        /// <param name="backViewModel">UserControl to return to after completion.</param>
        public OutputViewModel(IView thisWindow, object backViewModel)
        {
            this.thisWindow = thisWindow;
            this.backViewModel = backViewModel;
            this.IsBackEnabled = false;
            this.CurrentOperationProgress = 0;
        }

        public int CurrentOperationProgress
        {
            get
            {
                return this.currentOperationProgress;
            }

            set
            {
                this.currentOperationProgress = value;
                this.RaisePropertyChangedEvent("CurrentOperationProgress");
            }
        }

        public string OutputText
        {
            get
            {
                return this.outputText;
            }

            set
            {
                this.outputText = value;
                this.RaisePropertyChangedEvent("OutputText");
            }
        }

        public bool IsBackEnabled
        {
            get
            {
                return this.isBackEnabled;
            }

            set
            {
                this.isBackEnabled = value;
                this.RaisePropertyChangedEvent("IsBackEnabled");
            }
        }

        public ICommand BackCommand { get => new DelegateCommand(this.Close); }

        public void WriteLine(string line)
        {
            this.OutputText += line + "\n";
            this.RaisePropertyChangedEvent("OutputText");
        }

        private void Close()
        {
            this.thisWindow.ChangeCurrentControlTo(this.backViewModel);
        }
    }
}