﻿using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using FileEncryptorWpf.Models;
using FileEncryptorWpf.ViewModels.CustomValidationAttributes;
using FileEncryptorWpf.Views;
using PrivateKeyParsers;
using UserDatabaseManager;

namespace FileEncryptorWpf.ViewModels
{
    /// <summary>
    /// Defines the <see cref="LoginViewModel" /> class.
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        private readonly ViewModelHistory thisWindow;

        private string privateKeyPath;

        private string userDatabasePath;

        private string username;

        private bool areActionsEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        /// <param name="thisWindow">Window in which LoginControl is shown.</param>
        public LoginViewModel(ViewModelHistory thisWindow)
        {
            this.thisWindow = thisWindow;
            this.Username = "default";
            this.PrivateKeyFilePath = "OPENSSL\\private\\default.key";
            this.UserDatabasePath = "root\\Users.db";
            this.AreActionsEnabled = true;
            try
            {
                using (FileStream propertyFile = new FileStream("settings.cfg", FileMode.Open))
                {
                    PropertiesStreams.Properties props = new PropertiesStreams.Properties();
                    props.Load(propertyFile);
                    this.UserDatabasePath = props.GetProperty("userdb", string.Empty);
                }
            }
            catch (Exception)
            {
            }
        }

        [Required(ErrorMessage = "The path to User Database is required for proper functioning of the application.")]
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
        public bool AreActionsEnabled
        {
            get
            {
                return this.areActionsEnabled;
            }

            set
            {
                this.areActionsEnabled = value;
                this.RaisePropertyChangedEvent("AreActionsEnabled");
            }
        }

        public ICommand ChoosePrivateKeyCommand { get => new DelegateCommand(this.ChoosePrivateKey); }

        public ICommand ChooseUserDatabaseCommand { get => new DelegateCommand(this.ChooseUserDatabase); }

        public ICommand LoginCommand { get => new DelegateCommandWithParameter(this.TryToLogin); }

        public ICommand SaveSettingsCommand { get => new DelegateCommand(this.SaveSettings); }

        public ICommand RegisterCommand { get => new DelegateCommand(this.GoToRegister); }

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
            this.ClearErrors();
            this.ValidateProperty(this.UserDatabasePath, "UserDatabasePath");

            if (this.HasErrors)
            {
                return;
            }

            try
            {
                using (FileStream propertyFile = new FileStream("settings.cfg", FileMode.Create))
                {
                    PropertiesStreams.Properties props = new PropertiesStreams.Properties();
                    props.AddProperty("userdb", this.UserDatabasePath);
                    props.Store(propertyFile);

                    MessageBox.Show("Settings successfully written.", "Save Success");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Saving settings failed.", "Save Failure");
            }
        }

        private async void TryToLogin(object passBox)
        {
            try
            {
                this.Validate();
                if (!this.HasErrors)
                {
                    this.AreActionsEnabled = false;
                    UserDatabase data = null;
                    LoginManager loginManager = new LoginManager(this.PrivateKeyFilePath, this.UserDatabasePath);
                    var userInfo = await Task.Run(() => loginManager.Login(this.username, (passBox as PasswordBox).Password, out data));

                    this.thisWindow.GoToViewModel(new MainViewModel(userInfo, data, this.thisWindow));
                }
            }
            catch (InvalidKeyFileException)
            {
                MessageBox.Show("The chosen private key file is invalid.", "Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            } finally
            {
                this.AreActionsEnabled = true;
            }
        }

        private void GoToRegister()
        {
            UserDatabase data = new UserDatabase(this.UserDatabasePath);
            this.thisWindow.GoToViewModel(new RegisterViewModel(this.thisWindow, data));
        }
    }
}