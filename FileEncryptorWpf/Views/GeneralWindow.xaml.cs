using System.Collections.Generic;
using System.Windows;
using FileEncryptorWpf.ViewModels;

namespace FileEncryptorWpf.Views
{
    /// <summary>
    /// Interaction logic for GeneralWindow.xaml
    /// </summary>
    public partial class GeneralWindow : Window, IViewModelHistory
    {
        private readonly Stack<object> viewModelHistory = new Stack<object>();

        public GeneralWindow()
        {
            this.InitializeComponent();
            this.DataContext = new GeneralViewModel(this);
        }

        public void GoToPreviousViewModel()
        {
            viewModelHistory.Pop();
            ((GeneralViewModel)DataContext).CurrentControl = viewModelHistory.Peek();
        }

        public void GoToViewModel(object x)
        {
            viewModelHistory.Push(x);
            ((GeneralViewModel)DataContext).CurrentControl = x;
        }
    }
}