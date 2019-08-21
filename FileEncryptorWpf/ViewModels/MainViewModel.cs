using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using AlgorithmLibrary;
using CryptedStreamParsers;
using FileEncryptorWpf.Models;
using FileEncryptorWpf.Models.ComboBoxObjects;
using FileEncryptorWpf.ViewModels.CustomValidationAttributes;
using FileEncryptorWpf.Views;

namespace FileEncryptorWpf.ViewModels
{
    /// <summary>
    /// Defines the <see cref="MainViewModel"/> class.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IView thisWindow;
        private readonly List<string> operationModes = new List<string>();
        private readonly List<HashAlgorithmChoice> hashAlgorithms = new List<HashAlgorithmChoice>();
        private readonly List<EncryptionAlgorithmChoice> encryptionAlgorithms = new List<EncryptionAlgorithmChoice>();
        private HashAlgorithmChoice chosenHashAlgorithm;
        private EncryptionAlgorithmChoice chosenEncryptionAlgorithm;
        private string usernameLabel;
        private string chosenMode;
        private bool areAlgorithmParametersEnabled;
        private string inputFilePath;
        private string outputFilePath;
        private string otherPartyUsername;
        private bool isEncrypt;
        private DataComponents dataSource;
        private readonly UserInformation currentUserInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="currentUser">All the required information about the current logged in user.</param>
        /// <param name="data">Instance of <see cref="DataComponents"/> that allows access to user database and certificates.</param>
        /// <param name="thisWindow">Window in which MainControl is shown.</param>
        public MainViewModel(UserInformation currentUser, DataComponents data, IView thisWindow)
        {
            this.dataSource = data;
            this.currentUserInfo = currentUser;

            this.thisWindow = thisWindow;

            this.operationModes.Add("Encrypt");
            this.operationModes.Add("Decrypt");
            this.ChosenMode = this.operationModes[0];

            this.hashAlgorithms.Add(new HashAlgorithmChoice("SHA256", SHA256.Create()));
            this.hashAlgorithms.Add(new HashAlgorithmChoice("MD5", MD5.Create()));
            this.ChosenHashAlgorithm = this.hashAlgorithms[0];

            this.encryptionAlgorithms.Add(new EncryptionAlgorithmChoice("AES", new AesMachine()));
            this.encryptionAlgorithms.Add(new EncryptionAlgorithmChoice("TripleDES", new TDesMachine()));
            this.encryptionAlgorithms.Add(new EncryptionAlgorithmChoice("Twofish", new TwofishMachine()));
            this.ChosenEncryptionAlgorithm = this.encryptionAlgorithms[0];
        }

        public IList<string> OperationModes
        {
            get { return this.operationModes; }
        }

        public IList<HashAlgorithmChoice> HashAlgorithms
        {
            get { return this.hashAlgorithms; }
        }

        public HashAlgorithmChoice ChosenHashAlgorithm
        {
            get
            {
                return this.chosenHashAlgorithm;
            }

            set
            {
                this.chosenHashAlgorithm = value;
                this.RaisePropertyChangedEvent("ChosenHashAlgorithm");
            }
        }

        public IList<EncryptionAlgorithmChoice> EncryptionAlgorithms
        {
            get { return this.encryptionAlgorithms; }
        }

        public EncryptionAlgorithmChoice ChosenEncryptionAlgorithm
        {
            get
            {
                return this.chosenEncryptionAlgorithm;
            }

            set
            {
                this.chosenEncryptionAlgorithm = value;
                this.RaisePropertyChangedEvent("ChosenEncryptionAlgorithm");
            }
        }

        public string UsernameLabel
        {
            get
            {
                return this.usernameLabel;
            }

            set
            {
                this.usernameLabel = value;
                this.RaisePropertyChangedEvent("UsernameLabel");
            }
        }

        public string ChosenMode
        {
            get
            {
                return this.chosenMode;
            }

            set
            {
                this.chosenMode = value;

                if (this.chosenMode == "Decrypt")
                {
                    this.AreAlgorithmParametersEnabled = false;
                    this.isEncrypt = false;
                    this.UsernameLabel = "  Sender:";
                }
                else
                {
                    this.AreAlgorithmParametersEnabled = true;
                    this.isEncrypt = true;
                    this.UsernameLabel = "Receiver:";
                }

                this.RaisePropertyChangedEvent("ChosenMode");
            }
        }

        public bool AreAlgorithmParametersEnabled
        {
            get
            {
                return this.areAlgorithmParametersEnabled;
            }

            set
            {
                this.areAlgorithmParametersEnabled = value;
                this.RaisePropertyChangedEvent("AreAlgorithmParametersEnabled");
            }
        }

        public bool IsEncrypt
        {
            get
            {
                return this.isEncrypt;
            }

            set
            {
                this.isEncrypt = value;
            }
        }

        [Required]
        [FileExists(ErrorMessage = "Specified input file does not exist.")]
        public string InputFilePath
        {
            get
            {
                return this.inputFilePath;
            }

            set
            {
                this.inputFilePath = value;
                this.RaisePropertyChangedEvent("InputFilePath");
            }
        }

        [Required]
        [FileExists(invert: true, ErrorMessage = "This file already exists. Select another file.")]
        [DirectoryExists(invert: true, ErrorMessage = "This path is a directory. Select a file.")]
        public string OutputFilePath
        {
            get
            {
                return this.outputFilePath;
            }

            set
            {
                this.outputFilePath = value;
                this.RaisePropertyChangedEvent("OutputFilePath");
            }
        }

        [Required]
        public string OtherPartyUsername
        {
            get
            {
                return this.otherPartyUsername;
            }

            set
            {
                this.otherPartyUsername = value;
                this.RaisePropertyChangedEvent("OtherParty");
            }
        }

        public ICommand ApplyOperationCommand { get => new DelegateCommand(this.ApplyOperation); }

        public ICommand ChooseInputFileCommand { get => new DelegateCommand(this.ChooseInputFile); }

        public ICommand ChooseOutputFileCommand { get => new DelegateCommand(this.ChooseOutputFile); }

        private void ApplyOperation()
        {
            this.Validate();
            if (this.HasErrors)
                return;

            bool userExists = !(this.dataSource.UserDatabase.GetUser(OtherPartyUsername) is null);
            if (!userExists)
            {
                System.Windows.Forms.MessageBox.Show("That user does not exist in the database.", "Error");
                return;
            }

            OutputViewModel outputViewModel = new OutputViewModel(this.thisWindow, this);
            this.thisWindow.ChangeCurrentControlTo(outputViewModel);

            void DoWork(object sender, DoWorkEventArgs e)
            {
                try
                {
                    using (FileStream input = new FileStream(this.InputFilePath, FileMode.Open))
                    {
                        var reporter = new ProgressReporter(
                            (string x) => App.Current.Dispatcher.Invoke(() => outputViewModel.WriteLine(x)),
                            (int i) => (sender as BackgroundWorker).ReportProgress(i));

                        if (this.IsEncrypt)
                        {
                            using (FileStream output = new FileStream(this.OutputFilePath, FileMode.Create))
                            {
                                OriginalFile origin = new OriginalFile(input, Path.GetExtension(input.Name));
                                Encryptor encryptor = new Encryptor(dataSource, otherPartyUsername, currentUserInfo, new CryptCombo(chosenHashAlgorithm.HashMachine, chosenEncryptionAlgorithm.CryptMachine));
                                encryptor.EncryptFile(origin, output, reporter);
                            }
                        }
                        else
                        {
                            var cryptedFile = EncryptedFileChecker.Parse(input);
                            using (var output = new FileStream(this.OutputFilePath + cryptedFile.FormatExtension, FileMode.Create))
                            {
                                Decryptor decryptor = new Decryptor(dataSource, otherPartyUsername, currentUserInfo);
                                decryptor.DecryptFile(cryptedFile, output, reporter);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    outputViewModel.WriteLine("FATAL ERROR: " + ex.Message);
                    outputViewModel.CurrentOperationProgress = 100;
                }
                finally
                {
                    outputViewModel.IsBackEnabled = true;
                }
            }

            void ProgressChanged(object sender, ProgressChangedEventArgs e)
            {
                if (e.ProgressPercentage > outputViewModel.CurrentOperationProgress)
                {
                    outputViewModel.CurrentOperationProgress = e.ProgressPercentage;
                }
            }

            using (BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            })
            {
                worker.DoWork += DoWork;
                worker.ProgressChanged += ProgressChanged;
                worker.RunWorkerAsync();
            }
        }

        private void ChooseOutputFile()
        {
            using (SaveFileDialog dlg = new SaveFileDialog
            {
                FileName = string.Empty,
                AddExtension = true,
                InitialDirectory = string.IsNullOrWhiteSpace(InputFilePath) ? "" : Path.GetDirectoryName(InputFilePath)
            })
            {
                var result = dlg.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dlg.FileName))
                {
                    this.OutputFilePath = dlg.FileName;
                }
                else
                {
                    this.ValidateProperty(OutputFilePath, "OutputFilePath");
                }
            }
        }

        private void ChooseInputFile()
        {
            using (OpenFileDialog fileChooseDialog = new OpenFileDialog
            {
                ValidateNames = true,
                CheckFileExists = true,
                CheckPathExists = true
            })
            {

                fileChooseDialog.ShowDialog();
                try
                {
                    FileInfo selectedFile = new FileInfo(fileChooseDialog.FileName);
                    this.InputFilePath = selectedFile.FullName;
                    if (string.IsNullOrWhiteSpace(this.OutputFilePath))
                    {
                        this.OutputFilePath = Path.GetDirectoryName(this.InputFilePath) + @"\";
                    }
                }
                catch (Exception)
                {
                    this.ValidateProperty(InputFilePath, "InputFilePath");
                }
            }
        }
    }
}