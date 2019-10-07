using System.Collections.Generic;
using System.Windows;
using FileEncryptorWpf.ViewModels;

namespace FileEncryptorWpf.Views
{
    /// <summary>
    /// Interaction logic for GeneralWindow.xaml
    /// </summary>
    public partial class GeneralWindow : Window
    {
        public GeneralWindow()
        {
            this.InitializeComponent();
            this.DataContext = new GeneralViewModel();
        }
    }
}