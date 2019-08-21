using FileEncryptorWpf.Models;
using FileEncryptorWpf.ViewModels.CustomValidationAttributes;
using FileEncryptorWpf.Views;
using PrivateKeyParsers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace FileEncryptorWpf.ViewModels
{
    /// <summary>
    /// Defines the <see cref="LoginViewModel" /> class.
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        private readonly IView thisWindow;

        private string certificationsFolderPath;

        private string privateKeyPath;

        private string userDatabasePath;

        private string username;

        private string authorityCertPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/>.
        /// </summary>
        /// <param name="thisWindow">Window in which LoginControl is shown.</param>
        public LoginViewModel(IView thisWindow)
        {
            this.thisWindow = thisWindow;
            this.Username = "default";
            this.PrivateKeyFilePath = "OPENSSL\\private\\default.key";
            this.CertificationsFolderPath = "OPENSSL\\certs";
            this.UserDatabasePath = "root\\Users.db";
            try
            {
                using (FileStream propertyFile = new FileStream("settings.cfg", FileMode.Open))
                {
                    PropertiesStreams.Properties props = new PropertiesStreams.Properties();
                    props.Load(propertyFile);
                    this.CertificationsFolderPath = props.GetProperty("certs", string.Empty);
                    this.UserDatabasePath = props.GetProperty("userdb", string.Empty);
                    this.AuthorityCertPath = props.GetProperty("auth_path", string.Empty);
                }
            }
            catch (Exception)
            {
            }
        }

        [Required(ErrorMessage = "The path to certifications folder is required for proper functioning of the application.")]
        //[IsValidPath(ErrorMessage = "Specified certificates folder path is invalid.")]
        [DirectoryExists(ErrorMessage = "Specified certificates folder path does not exist.")]
        public string CertificationsFolderPath
        {
            get
            {
                return this.certificationsFolderPath;
            }

            set
            {
                this.certificationsFolderPath = value;
                this.RaisePropertyChangedEvent("CertificationsFolderPath");
            }
        }

        [Required(ErrorMessage = "The path to main CA is required for proper functioning of the application.")]
        //[IsValidPath(ErrorMessage = "Specified CA file path is invalid.")]
        [FileExists(ErrorMessage = "Specified CA file does not exist.")]
        public string AuthorityCertPath
        {
            get
            {
                return this.authorityCertPath;
            }

            set
            {
                this.authorityCertPath = value;
                this.RaisePropertyChangedEvent("AuthorityCertPath");
            }
        }

        [Required(ErrorMessage = "The path to User Database is required for proper functioning of the application.")]
        //[IsValidPath(ErrorMessage = "Specified User Database file path is invalid.")]
        [FileExists(ErrorMessage = "Specified User Database file does not exist.")]
        public string UserDatabasePath
        {
            get
            {
                return this.userDatabasePath;
            }

            set
            {
                this.userDatabasePath = value;
                this.RaisePropertyChangedEvent("UserDatabasePath");
            }
        }

        [Required(ErrorMessage = "Private key is required for login.")]
        //[IsValidPath(ErrorMessage = "Specified private key file path is invalid.")]
        [FileExists(ErrorMessage = "Specified private key file does not exist.")]
        public string PrivateKeyFilePath
        {
            get
            {
                return this.privateKeyPath;
            }

            set
            {
                this.privateKeyPath = value;
                this.RaisePropertyChangedEvent("PrivateKeyFilePath");
            }
        }

        [Required(ErrorMessage = "Username is required for login.")]
        //[StringLength(25, MinimumLength = 7, ErrorMessage = "Username must be between 7 and 25 characters long.")]
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

        public Dictionary<string, string> ValidationErrors = new Dictionary<string, string>();

        public ICommand ChooseCertificateFolderCommand { get => new DelegateCommand(this.ChooseCertificateFolder); }

        public ICommand ChoosePrivateKeyCommand { get => new DelegateCommand(this.ChoosePrivateKey); }

        public ICommand ChooseUserDatabaseCommand { get => new DelegateCommand(this.ChooseUserDatabase); }

        public ICommand ChooseAuthorityCommand { get => new DelegateCommand(this.ChooseAuthority); }

        public ICommand LoginCommand { get => new DelegateCommandWithParameter(this.TryToLogin); }

        public ICommand SaveSettingsCommand { get => new DelegateCommand(this.SaveSettings); }

        public ICommand RegisterCommand { get => new DelegateCommand(this.GoToRegister); }

        private void ChooseAuthority()
        {
            using (OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "PEM files (*.pem)|*.pem|DER files (*.der)|*.der|CRT files (*.crt)|*.crt|All files (*.*)|*.*",
                CheckFileExists = true,
                CheckPathExists = true,
            })
            {
                var result = dlg.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dlg.FileName))
                {
                    this.AuthorityCertPath = dlg.FileName;
                }
            }
        }

        private void ChooseCertificateFolder()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Choose folder that contains all the certificates for the app.";
                var result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.CertificationsFolderPath = fbd.SelectedPath;
                }
            }
        }

        private void ChoosePrivateKey()
        {
            using (OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "PEM file|*.pem|DER file|*.der|Key file|*.key|All files|*.*",
                CheckFileExists = true,
                CheckPathExists = true,
                FilterIndex = 3
            })
            {

                var result = dlg.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dlg.FileName))
                {
                    this.PrivateKeyFilePath = dlg.FileName;
                }
            }
        }

        private void ChooseUserDatabase()
        {
            using (OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Database file|*.db",
                CheckPathExists = true,
                CheckFileExists = true
            })
            {

                var result = dlg.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dlg.FileName))
                {
                    this.UserDatabasePath = dlg.FileName;
                }
            }
        }

        private void SaveSettings()
        {
            ClearErrors();
            ValidateProperty(this.CertificationsFolderPath, "CertificationsFolderPath");
            ValidateProperty(this.UserDatabasePath, "UserDatabasePath");
            ValidateProperty(this.AuthorityCertPath, "AuthorityCertPath");

            if (this.HasErrors)
                return;

            try
            {
                
                using (FileStream propertyFile = new FileStream("settings.cfg", FileMode.Create))
                {
                    PropertiesStreams.Properties props = new PropertiesStreams.Properties();
                    props.AddProperty("certs", this.CertificationsFolderPath);
                    props.AddProperty("userdb", this.UserDatabasePath);
                    props.AddProperty("auth_path", this.AuthorityCertPath);
                    props.Store(propertyFile);

                    MessageBox.Show("Settings successfully written.", "Save Success");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Saving settings failed.", "Save Failure");
            }
        }

        private void TryToLogin(object passBox)
        {
            try
            {
                this.Validate();
                if(!this.HasErrors)
                {
                    LoginManager loginManager = new LoginManager(certificationsFolderPath, privateKeyPath, userDatabasePath, authorityCertPath);
                    var userInfo = loginManager.Login(this.username, (passBox as PasswordBox).Password, out var data);

                    this.thisWindow.ChangeCurrentControlTo(new MainViewModel(userInfo, data, this.thisWindow));
                }
                else
                {

                }
                
            }
            catch (InvalidKeyFileException)
            {
                MessageBox.Show("The chosen private key file is invalid.", "Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void GoToRegister()
        {
            DataComponents data = new DataComponents(UserDatabasePath, CertificationsFolderPath, AuthorityCertPath);
            this.thisWindow.ChangeCurrentControlTo(new RegisterViewModel(this.thisWindow, this, data));
        }
    }
}