using System.Windows;
using FileEncryptorWpf.ViewModels;

namespace FileEncryptorWpf.Views
{
    /// <summary>
    /// Interaction logic for GeneralWindow.xaml
    /// </summary>
    public partial class GeneralWindow : Window, IWindow
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