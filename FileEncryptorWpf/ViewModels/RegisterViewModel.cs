using FileEncryptorWpf.Models;
using FileEncryptorWpf.ViewModels.CustomValidationAttributes;
using FileEncryptorWpf.Views;
using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using UserDatabaseManager;

namespace FileEncryptorWpf.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly IView thisWindow;
        private readonly object backControl;
        private string username;
        private string password;
        private string certificateFilePath;
        private bool isExternal;
        private readonly UserDatabase data;

        public RegisterViewModel(IView thisWindow, object backControl, UserDatabase data)
        {
            this.thisWindow = thisWindow;
            this.backControl = backControl;
            this.data = data;
        }

        public bool IsNotExternal { get => !isExternal; }

        [Required]
        [StringLength(25, MinimumLength = 7, ErrorMessage = "Username must be between 7 and 25 characters long.")]
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

        [Required]
        [SecurePassword]
        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                this.password = value;
                this.RaisePropertyChangedEvent("Password");
            }
        }

        [Required]
        [FileExists(ErrorMessage = "Specified public certificate does not exist.")]
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

        public ICommand RegisterCommand { get => new DelegateCommand(this.TryToRegister); }

        public ICommand ChooseCertificateCommand { get => new DelegateCommandWithParameter(this.ChooseCertificate); }

        private void ChooseCertificate(object obj)
        {
            using (OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Key file | *.*"
            })
            {
                var result = dlg.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dlg.FileName))
                {
                    CertificateFilePath = dlg.FileName;
                }
            }
        }

        public ICommand CancelCommand { get => new DelegateCommand(this.Cancel); }

        private void Cancel()
        {
            this.thisWindow.ChangeCurrentControlTo(backControl);
        }

        private void TryToRegister()
        {
            try
            {
                this.ClearErrors();
                if(this.IsNotExternal)
                {
                    this.ValidateProperty(Password, "Password");
                }
                OnErrorsChanged("Password");
                this.ValidateProperty(Username, "Username");
                this.ValidateProperty(CertificateFilePath, "CertificateFilePath");

                if (!this.HasErrors)
                {
                    RegisterManager registerManager = new RegisterManager(data);
                    registerManager.Register(username, certificateFilePath, Password, isExternal);
                    this.thisWindow.ChangeCurrentControlTo(backControl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
