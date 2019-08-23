using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using FileEncryptorWpf.Models;
using FileEncryptorWpf.ViewModels.CustomValidationAttributes;
using FileEncryptorWpf.Views;
using UserDatabaseManager;

namespace FileEncryptorWpf.ViewModels
{
    /// <summary>
    /// Defines the <see cref="RegisterViewModel" /> class.
    /// </summary>
    public class RegisterViewModel : ViewModelBase
    {
        private readonly IView thisWindow;
        private readonly object backControl;
        private readonly UserDatabase data;
        private string username;
        private string password;
        private string certificateFilePath;
        private bool isExternal;

        public RegisterViewModel(IView thisWindow, object backControl, UserDatabase data)
        {
            this.thisWindow = thisWindow;
            this.backControl = backControl;
            this.data = data;
        }

        public bool IsNotExternal { get => !this.isExternal; }

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

        public ICommand CancelCommand { get => new DelegateCommand(this.Cancel); }

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
                    this.CertificateFilePath = dlg.FileName;
                }
            }
        }

        private void Cancel()
        {
            this.thisWindow.ChangeCurrentControlTo(this.backControl);
        }

        private async void TryToRegister()
        {
            try
            {
                this.ClearErrors();
                if (this.IsNotExternal)
                {
                    this.ValidateProperty(this.Password, "Password");
                }

                this.OnErrorsChanged("Password");
                this.ValidateProperty(this.Username, "Username");
                this.ValidateProperty(this.CertificateFilePath, "CertificateFilePath");

                if (!this.HasErrors)
                {
                    RegisterManager registerManager = new RegisterManager(this.data);
                    await Task.Run(() => registerManager.Register(this.Username, this.CertificateFilePath, this.Password, this.IsExternal));
                    this.thisWindow.ChangeCurrentControlTo(this.backControl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
