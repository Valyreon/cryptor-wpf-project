using FileEncryptorWpf.ViewModels;

using System.Windows;

namespace FileEncryptorWpf.Views
{
    /// <summary>
    /// Interaction logic for GeneralWindow.xaml
    /// </summary>
    public partial class GeneralWindow : Window, IView
    {
        public GeneralWindow()
        {
            this.InitializeComponent();
            this.DataContext = new GeneralViewModel(this);
        }

        public void ChangeCurrentControlTo(object x)
        {
            ((GeneralViewModel)DataContext).CurrentControl = x;
        }

        public void SetHeight(int height)
        {
            this.Height = height;
        }
    }
}