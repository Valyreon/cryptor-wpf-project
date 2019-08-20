using FileEncryptorWpf.Models;
using FileEncryptorWpf.Views;
using System;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using UserDatabaseManager;

namespace FileEncryptorWpf.ViewModels
{
    public class RegisterViewModel: ViewModelBase
    {
        private readonly IView thisWindow;
        private readonly object backControl;
        private string username;
        private string certificateFilePath;
        private bool isExternal;
        private readonly DataComponents data;

        public RegisterViewModel(IView thisWindow, object backControl, DataComponents data)
        {
            this.thisWindow = thisWindow;
            this.backControl = backControl;
            this.data = data;
        }

        public bool IsNotExternal { get => !isExternal; }

        public string Username
        {
            get
            {
                return this.username;
            }

            set
            {
                this.username = value;
                this.RaisePropertyChangedEvent("Username");
            }
        }

        public string CertificateFilePath
        {
            get
            {
                return this.certificateFilePath;
            }

            set
            {
                this.certificateFilePath = value;
                this.RaisePropertyChangedEvent("CertificateFilePath");
            }
        }

        public bool IsExternal
        {
            get
            {
                return this.isExternal;
            }

            set
            {
                this.isExternal = value;
                this.RaisePropertyChangedEvent("IsExternal");
                this.RaisePropertyChangedEvent("IsNotExternal");
            }
        }

        public ICommand RegisterCommand { get => new DelegateCommandWithParameter(this.TryToRegister); }

        public ICommand ChooseCertificateCommand { get => new DelegateCommandWithParameter(this.ChooseCertificate); }

        private void ChooseCertificate(object obj)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Key file | *.*"
            };

            var result = dlg.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dlg.FileName))
            {
                CertificateFilePath = dlg.FileName;
            }
        }

        public ICommand CancelCommand { get => new DelegateCommand(this.Cancel); }

        private void Cancel()
        {
            this.thisWindow.ChangeCurrentControlTo(backControl);
        }

        private void TryToRegister(object passwordBox)
        {
            try
            {
                RegisterManager registerManager = new RegisterManager(data);
                registerManager.Register(username, certificateFilePath , (passwordBox as PasswordBox).Password, isExternal);
                this.Cancel();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
