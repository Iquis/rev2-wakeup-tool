using System;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using GGXrdWakeupDPUtil.Commands;
using GGXrdWakeupDPUtil.Library;
using GGXrdWakeupDPUtil.Library.Replay;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace GGXrdWakeupDPUtil.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ReversalTool _reversalTool = new ReversalTool();
        private readonly UpdateManager _updateManager = new UpdateManager();

        private readonly StringBuilder _logStringBuilder = new StringBuilder();
        public string LogText => _logStringBuilder.ToString();

        public string Title => $"GGXrd Rev 2 Reversal Tool v{ConfigurationManager.AppSettings.Get("CurrentVersion")}";

        public bool AutoUpdate
        {
            get => ConfigurationManager.AppSettings.Get("AutoUpdate") == "1";
            set
            {
                ConfigManager.Set("AutoUpdate", value ? "1" : "0");


                this.OnPropertyChanged();
            }
        }



        private string _dummyName;
        public string DummyName
        {
            get => _dummyName;
            set
            {
                _dummyName = value;
                this.OnPropertyChanged();
            }
        }

        #region Wakeup Reversal
        private int _wakeupReversalSlotNumber = 1;
        public int WakeupReversalSlotNumber
        {
            get => _wakeupReversalSlotNumber;
            set
            {
                _wakeupReversalSlotNumber = value;
                this.OnPropertyChanged();
            }
        }

        private bool _isWakeupReversalStarted;
        public bool IsWakeupReversalStarted
        {
            get => _isWakeupReversalStarted;
            set
            {
                _isWakeupReversalStarted = value;
                this.OnPropertyChanged();
            }
        }

        private string _wakeupReversalInput;
        public string WakeupReversalInput
        {
            get => _wakeupReversalInput;
            set
            {
                _wakeupReversalInput = value;
                SlotInput slotInput = new SlotInput(this.WakeupReversalInput);
                this.IsWakeupInputValid = string.IsNullOrEmpty(this.WakeupReversalInput) || slotInput.IsValid;
                this.IsWakeupReversalInputValid = string.IsNullOrEmpty(this.WakeupReversalInput) || slotInput.IsReversalValid;

                if (!slotInput.IsValid)
                {
                    this.WakeupReversalErrorMessage = "Invalid Input";
                }
                else if (!slotInput.IsReversalValid)
                {
                    this.WakeupReversalErrorMessage = "Reversal frame not found";
                }


                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(WakeupReversalErrorVisibility));
            }
        }

        private bool _isWakeupInputValid;
        public bool IsWakeupInputValid
        {
            get => _isWakeupInputValid;
            set
            {
                _isWakeupInputValid = value;
                this.OnPropertyChanged();
            }
        }

        private bool _isWakeupReversalInputValid;

        public bool IsWakeupReversalInputValid
        {
            get => _isWakeupReversalInputValid;
            set
            {
                _isWakeupReversalInputValid = value;
                this.OnPropertyChanged();
            }

        }

        public Visibility WakeupReversalErrorVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(this.WakeupReversalInput) || IsWakeupReversalInputValid)
                {
                    return Visibility.Hidden;
                }

                return Visibility.Visible;
            }
        }

        private string _wakeupReversalErrorMessage;

        public string WakeupReversalErrorMessage
        {
            get => _wakeupReversalErrorMessage;
            set
            {
                _wakeupReversalErrorMessage = value;
                this.OnPropertyChanged();
            }
        }

        private int _wakeupReversalPercentage = 100;
        public int WakeupReversalPercentage
        {
            get => _wakeupReversalPercentage;
            set
            {
                _wakeupReversalPercentage = Math.Max(Math.Min(100, value), 0);
                this.OnPropertyChanged();

            }
        }

        private bool _playReversalAfterWallsplat = true;

        public bool PlayReversalAfterWallsplat
        {
            get => _playReversalAfterWallsplat;
            set
            {
                _playReversalAfterWallsplat = value;
                this.OnPropertyChanged();
            }


        }
        #endregion

        #region BlockStun Reversal
        private int _blockstunReversalSlotNumber = 1;
        public int BlockstunReversalSlotNumber
        {
            get => _blockstunReversalSlotNumber;
            set
            {
                _blockstunReversalSlotNumber = value;
                this.OnPropertyChanged();
            }
        }

        private bool _isBlockstunReversalStarted;
        public bool IsBlockstunReversalStarted
        {
            get => _isBlockstunReversalStarted;
            set
            {
                _isBlockstunReversalStarted = value;
                this.OnPropertyChanged();
            }
        }

        private string _blockstunReversalInput;
        public string BlockstunReversalInput
        {
            get => _blockstunReversalInput;
            set
            {
                _blockstunReversalInput = value;
                SlotInput slotInput = new SlotInput(this.BlockstunReversalInput);
                this.IsBlockstunInputValid = string.IsNullOrEmpty(this.BlockstunReversalInput) || slotInput.IsValid;
                this.IsBlockstunReversalInputValid = string.IsNullOrEmpty(this.BlockstunReversalInput) || slotInput.IsReversalValid;

                if (!slotInput.IsValid)
                {
                    this.BlockstunReversalErrorMessage = "Invalid Input";
                }
                else if (!slotInput.IsReversalValid)
                {
                    this.BlockstunReversalErrorMessage = "Reversal frame not found";
                }

                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(BlockstunReversalErrorVisibility));
            }
        }

        private bool _isBlockstunInputValid;
        public bool IsBlockstunInputValid
        {
            get => _isBlockstunInputValid;
            set
            {
                _isBlockstunInputValid = value;
                this.OnPropertyChanged();
            }
        }

        private bool _isBlockstunReversalInputValid;

        public bool IsBlockstunReversalInputValid
        {
            get => _isBlockstunReversalInputValid;
            set
            {
                _isBlockstunReversalInputValid = value;
                this.OnPropertyChanged();
            }
        }

        public Visibility BlockstunReversalErrorVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(this.BlockstunReversalInput) || IsBlockstunReversalInputValid)
                {
                    return Visibility.Hidden;
                }

                return Visibility.Visible;
            }
        }

        private string _blockstunReversalErrorMessage;

        public string BlockstunReversalErrorMessage
        {
            get => _blockstunReversalErrorMessage;
            set
            {
                _blockstunReversalErrorMessage = value;
                this.OnPropertyChanged();
            }
        }

        private int _blockstunReversalPercentage = 100;
        public int BlockstunReversalPercentage
        {
            get => _blockstunReversalPercentage;
            set
            {
                _blockstunReversalPercentage = Math.Max(Math.Min(100, value), 0);
                this.OnPropertyChanged();

            }
        }

        private int _blockstunReversalDelay = 0;
        public int BlockstunReversalDelay
        {
            get => _blockstunReversalDelay;
            set
            {
                _blockstunReversalDelay = Math.Max(Math.Min(100, value), 0);
                this.OnPropertyChanged();

            }
        }
        #endregion

        #region Burst

        private int _minimumBurstComboValue = 1;
        public int MinimumBurstComboValue
        {
            get => _minimumBurstComboValue;
            set
            {
                _minimumBurstComboValue = value;
                if (_minimumBurstComboValue > MaximumBurstComboValue)
                {
                    _minimumBurstComboValue = MaximumBurstComboValue;
                }
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(BurstInfo));
            }
        }

        private int _maximumBurstComboValue = 10;
        public int MaximumBurstComboValue
        {
            get => _maximumBurstComboValue;
            set
            {
                _maximumBurstComboValue = value;

                if (_maximumBurstComboValue < MinimumBurstComboValue)
                {
                    _maximumBurstComboValue = MinimumBurstComboValue;
                }
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(BurstInfo));
            }
        }

        private int _burstPercentage = 50;
        public int BurstPercentage
        {
            get => _burstPercentage;
            set
            {
                _burstPercentage = value;
                if (_burstPercentage < 0)
                {
                    _burstPercentage = 0;
                }

                if (_burstPercentage > 100)
                {
                    _burstPercentage = 100;
                }
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(BurstPercentageText));
                this.OnPropertyChanged(nameof(BurstInfo));
            }
        }

        public string BurstPercentageText => $"Burst Percentage {BurstPercentage}%";

        private int _randomBurstSlotNumber = 1;
        public int RandomBurstSlotNumber
        {
            get => _randomBurstSlotNumber;
            set
            {
                _randomBurstSlotNumber = value;
                this.OnPropertyChanged();
            }
        }

        private bool _isRandomBurstStarted = false;
        public bool IsRandomBurstStarted
        {
            get => _isRandomBurstStarted;
            set
            {
                _isRandomBurstStarted = value;
                this.OnPropertyChanged();
            }
        }



        public string BurstInfo
        {
            get
            {

                if (this.BurstPercentage == 100)
                {
                    return $"The dummy will burst randomly between {this.MinimumBurstComboValue} and {this.MaximumBurstComboValue} hit combo";
                }
                else
                {
                    string text =
                        this.MinimumBurstComboValue == this.MaximumBurstComboValue ?
                            $"- The dummy will burst randomly at {MinimumBurstComboValue} hit combo ({BurstPercentage}% chance)" :
                            $"- The dummy will burst randomly between {MinimumBurstComboValue} and {MaximumBurstComboValue} hit combo ({BurstPercentage}% chance)";

                    text += Environment.NewLine;
                    text += $"- The dummy won't burst at all ({100 - BurstPercentage}% chance)";

                    return text;

                }
            }

        }
        #endregion

        #region Import

        private int _importSlotNumber = 1;
        public int ImportSlotNumber
        {
            get => _importSlotNumber;
            set
            {
                _importSlotNumber = value;
                this.OnPropertyChanged();
            }
        }

        #endregion

        #region Export

        private int _exportSlotNumber = 1;
        public int ExportSlotNumber
        {
            get => _exportSlotNumber;
            set
            {
                _exportSlotNumber = value;
                this.OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        #region Window
        #region WindowLoadedCommand
        private RelayCommand _windowLoadedCommand;
        public RelayCommand WindowLoadedCommand => this._windowLoadedCommand ?? (this._windowLoadedCommand = new RelayCommand(this.InitializeWindow));
        #endregion

        #region WindowClosedCommand

        private RelayCommand _windowClosedCommand;
        public RelayCommand WindowClosedCommand => this._windowClosedCommand ?? (this._windowClosedCommand = new RelayCommand(this.DisposeWindow));
        #endregion
        #endregion

        #region StartWakeupReversalCommand
        private RelayCommand _startWakeupReversalCommand;
        public RelayCommand StartWakeupReversalCommand => this._startWakeupReversalCommand ?? (this._startWakeupReversalCommand = new RelayCommand(this.StartWakeupReversal, this.CanStartWakeupReversal));
        private void StartWakeupReversal()
        {
            SlotInput slotInput = new SlotInput(this.WakeupReversalInput);
            this._reversalTool.SetInputInSlot(this.WakeupReversalSlotNumber, slotInput);
            this._reversalTool.StartWakeupReversalLoop(slotInput, this.WakeupReversalPercentage, this.PlayReversalAfterWallsplat);
            this.IsWakeupReversalStarted = true;
        }
        private bool CanStartWakeupReversal()
        {
            return !this.IsWakeupReversalStarted && this.IsWakeupReversalInputValid && !string.IsNullOrEmpty(this.WakeupReversalInput);
        }
        #endregion

        #region StopReversalCommand
        private RelayCommand _stopWakeupReversalCommand;

        public RelayCommand StopWakeupReversalCommand => this._stopWakeupReversalCommand ?? (this._stopWakeupReversalCommand = new RelayCommand(this.StopWakeupReversal, this.CanStopWakeupReversal));

        private void StopWakeupReversal()
        {
            this._reversalTool.StopReversalLoop();
            this.IsWakeupReversalStarted = false;
        }

        private bool CanStopWakeupReversal()
        {
            return this.IsWakeupReversalStarted;
        }

        #endregion

        #region StartBlockstunReversalCommand
        private RelayCommand _startBlockstunReversalCommand;
        public RelayCommand StartBlockstunReversalCommand => this._startBlockstunReversalCommand ?? (this._startBlockstunReversalCommand = new RelayCommand(this.StartBlockstunReversal, this.CanStartBlockstunReversal));
        private void StartBlockstunReversal()
        {
            SlotInput slotInput = new SlotInput(this.BlockstunReversalInput);
            this._reversalTool.SetInputInSlot(this.BlockstunReversalSlotNumber, slotInput);
            this._reversalTool.StartBlockReversalLoop(slotInput, this.BlockstunReversalPercentage, this.BlockstunReversalDelay);
            this.IsBlockstunReversalStarted = true;
        }
        private bool CanStartBlockstunReversal()
        {
            return !this.IsBlockstunReversalStarted && this.IsBlockstunReversalInputValid && !string.IsNullOrEmpty(this.BlockstunReversalInput);
        }
        #endregion

        #region StopReversalCommand
        private RelayCommand _stopBlockstunReversalCommand;

        public RelayCommand StopBlockstunReversalCommand => this._stopBlockstunReversalCommand ?? (this._stopBlockstunReversalCommand = new RelayCommand(this.StopBlockstunReversal, this.CanStopBlockstunReversal));

        private void StopBlockstunReversal()
        {
            this._reversalTool.StopBlockReversalLoop();
            this.IsBlockstunReversalStarted = false;
        }

        private bool CanStopBlockstunReversal()
        {
            return this.IsBlockstunReversalStarted;
        }

        #endregion

        #region StartRandomBurstCommand

        private RelayCommand _startRandomBurstCommand;
        public RelayCommand StartRandomBurstCommand => _startRandomBurstCommand ?? (_startRandomBurstCommand = new RelayCommand(StartRandomBurst, CanStartRandomBurst));

        private bool CanStartRandomBurst()
        {
            return !this.IsRandomBurstStarted;
        }

        private void StartRandomBurst()
        {
            this._reversalTool.StartRandomBurstLoop(this.MinimumBurstComboValue, this.MaximumBurstComboValue, this.RandomBurstSlotNumber, this.BurstPercentage);
            this.IsRandomBurstStarted = true;
        }

        #endregion

        #region StopRandomBurstCommand

        private RelayCommand _stopRandomBurstCommand;

        public RelayCommand StopRandomBurstCommand => _stopRandomBurstCommand ?? (_stopRandomBurstCommand = new RelayCommand(StopRandomBurst, CanStopRandomBurst));

        private bool CanStopRandomBurst()
        {
            return this.IsRandomBurstStarted;
        }

        private void StopRandomBurst()
        {
            this._reversalTool.StopRandomBurstLoop();
            this.IsRandomBurstStarted = false;
        }

        #endregion

        #region CheckUpdatesCommand

        private RelayCommand _checkUpdatesCommand;

        public RelayCommand CheckUpdatesCommand => _checkUpdatesCommand ?? (_checkUpdatesCommand = new RelayCommand(CheckUpdates, CanCheckUpdates));

        private bool CanCheckUpdates()
        {
            return !IsWakeupReversalStarted && !IsBlockstunReversalStarted && !IsRandomBurstStarted;
        }
        private void CheckUpdates()
        {
            this.UpdateProcess(true);
        }



        #endregion

        #region ImportCommand

        private RelayCommand _importCommand;
        public RelayCommand ImportCommand => _importCommand ?? (_importCommand = new RelayCommand(ImportCommandExecute, ImportCommandCanExecute));

        private void ImportCommandExecute()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Reversal Tool Replay Slot file (*.ggrs)|*.ggrs"
            };
            var dialogResult = ofd.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                var bytes = this._reversalTool.ReadInputFile(ofd.FileName);
                SlotInput slotInput = new SlotInput(bytes);

                bool success = slotInput.IsValid && this._reversalTool.SetInputInSlot(this.ImportSlotNumber, slotInput);


                if (success)
                {
                    LogManager.Instance.WriteLine($"Inputs imported from file {ofd.FileName} to slot number {this.ImportSlotNumber}");
                    MessageBox.Show("Import succeed");
                    this._reversalTool.BringWindowToFront();
                }
                else
                {
                    LogManager.Instance.WriteLine($"Failed to import Inputs from file {ofd.FileName} to slot number {this.ImportSlotNumber}");
                    MessageBox.Show("Import failed");
                }
            }
        }

        private bool ImportCommandCanExecute()
        {
            return
                !this.IsWakeupReversalStarted &&
                !this.IsBlockstunReversalStarted &&
                !this.IsRandomBurstStarted;
        }
        #endregion

        #region ExportCommand
        private RelayCommand _exportCommand;

        public RelayCommand ExportCommand => _exportCommand ?? (_exportCommand = new RelayCommand(ExportCommandExecute, ExportCommandCanExecute));

        private void ExportCommandExecute()
        {
            SaveFileDialog svd = new SaveFileDialog()
            {
                Filter = "Reversal Tool Replay Slot file (*.ggrs)|*.ggrs"
            };

            var dialogResult = svd.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                byte[] input = this._reversalTool.ReadInputInSlot(this.ExportSlotNumber);

                var success = this._reversalTool.WriteInputFile(svd.FileName, input);


                if (success)
                {
                    LogManager.Instance.WriteLine($"Inputs exported from slot number {this.ExportSlotNumber} to file {svd.FileName}");
                    MessageBox.Show("Export succeed");
                }
                else
                {
                    LogManager.Instance.WriteLine($"Failed to export inputs from slot number {this.ExportSlotNumber} to file {svd.FileName}");
                    MessageBox.Show("Export failed");
                }
            }
        }

        private bool ExportCommandCanExecute()
        {
            return
                !this.IsWakeupReversalStarted &&
                !this.IsBlockstunReversalStarted &&
                !this.IsRandomBurstStarted;
        }
        #endregion

        #region Translate

        #region Wakeup
        private RelayCommand _wakeupTranslateFromFileCommand;
        public RelayCommand WakeupTranslateFromFileCommand => _wakeupTranslateFromFileCommand ?? (_wakeupTranslateFromFileCommand = new RelayCommand(WakeupTranslateFromFileCommandExecute, WakeupTranslateFromFileCommandCanExecute));

        private void WakeupTranslateFromFileCommandExecute()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Reversal Tool Replay Slot file (*.ggrs)|*.ggrs"
            };
            var dialogResult = ofd.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string input = this._reversalTool.TranslateFromFile(ofd.FileName);

                if (string.IsNullOrEmpty(input))
                {
                    LogManager.Instance.WriteLine($"Failed to import inputs from file {ofd.FileName} to Wakeup Reversal");
                }
                else
                {
                    this.WakeupReversalInput = input;

                    LogManager.Instance.WriteLine($"Inputs imported from file {ofd.FileName} to Wakeup Reversal");
                }

            }
        }

        private bool WakeupTranslateFromFileCommandCanExecute()
        {
            return !IsWakeupReversalStarted;
        }


        private RelayCommand _wakeupTranslateIntoFileCommand;
        public RelayCommand WakeupTranslateIntoFileCommand => _wakeupTranslateIntoFileCommand ?? (_wakeupTranslateIntoFileCommand = new RelayCommand(WakeupTranslateIntoFileCommandExecute, WakeupTranslateIntoFileCommandCanExecute));

        private void WakeupTranslateIntoFileCommandExecute()
        {
            SaveFileDialog svd = new SaveFileDialog()
            {
                Filter = "Reversal Tool Replay Slot file (*.ggrs)|*.ggrs"
            };

            var dialogResult = svd.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                var success = this._reversalTool.TranslateIntoFile(svd.FileName, this.WakeupReversalInput);

                if (success)
                {
                    LogManager.Instance.WriteLine($"Inputs exported from Wakeup Reversal to file {svd.FileName}");
                }
                else
                {
                    LogManager.Instance.WriteLine($"Failed to export inputs from Wakeup Reversal to file {svd.FileName}");
                }
            }
        }

        private bool WakeupTranslateIntoFileCommandCanExecute()
        {
            return !string.IsNullOrEmpty(this.WakeupReversalInput) && IsWakeupInputValid;
        }
        #endregion

        #region Blockstun
        private RelayCommand _blockstunTranslateFromFileCommand;
        public RelayCommand BlockstunTranslateFromFileCommand => _blockstunTranslateFromFileCommand ?? (_blockstunTranslateFromFileCommand = new RelayCommand(BlockstunTranslateFromFileCommandExecute, BlockstunTranslateFromFileCommandCanExecute));

        private void BlockstunTranslateFromFileCommandExecute()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Reversal Tool Replay Slot file (*.ggrs)|*.ggrs"
            };

            var dialogResult = ofd.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string input = this._reversalTool.TranslateFromFile(ofd.FileName);

                if (string.IsNullOrEmpty(input))
                {
                    LogManager.Instance.WriteLine($"Failed to import inputs from file {ofd.FileName} to Blockstun Reversal");
                }
                else
                {
                    this.BlockstunReversalInput = input;

                    LogManager.Instance.WriteLine($"Inputs imported from file {ofd.FileName} to Blockstun Reversal");
                }
            }
        }

        private bool BlockstunTranslateFromFileCommandCanExecute()
        {
            return !IsBlockstunReversalStarted;
        }


        private RelayCommand _blockstunTranslateIntoFileCommand;
        public RelayCommand BlockstunTranslateIntoFileCommand => _blockstunTranslateIntoFileCommand ?? (_blockstunTranslateIntoFileCommand = new RelayCommand(BlockstunTranslateIntoFileCommandExecute, BlockstunTranslateIntoFileCommandCanExecute));

        private void BlockstunTranslateIntoFileCommandExecute()
        {
            SaveFileDialog svd = new SaveFileDialog()
            {
                Filter = "Reversal Tool Replay Slot file (*.ggrs)|*.ggrs"
            };

            var dialogResult = svd.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                var success = this._reversalTool.TranslateIntoFile(svd.FileName, this.BlockstunReversalInput);

                if (success)
                {
                    LogManager.Instance.WriteLine($"Inputs exported from Blockstun Reversal to file {svd.FileName}");
                }
                else
                {
                    LogManager.Instance.WriteLine($"Failed to export inputs from Blockstun Reversal to file {svd.FileName}");
                }
            }
        }

        private bool BlockstunTranslateIntoFileCommandCanExecute()
        {
            return !string.IsNullOrEmpty(this.BlockstunReversalInput) && IsBlockstunInputValid;
        }
        #endregion



        #endregion

        #endregion


        #region MyRegion

        private void InitializeWindow()
        {
#if !DEBUG
            if (AutoUpdate)
            {
                UpdateProcess();
            } 
#endif

            _reversalTool.DummyChanged += ReversalTool_DummyChanged;
            try
            {
                _reversalTool.AttachToProcess();
            }
            catch (Exception exception)
            {
                string message =
                    "Guilty Gear not found open!  Remember, be in training mode paused when you open this program.  This program will now close.";
                LogManager.Instance.WriteLine(message);
                MessageBox.Show($"{message}{Environment.NewLine}{exception.Message}");
                Application.Current.Shutdown();
            }

            LogManager.Instance.LineReceived += LogManager_LineReceived;
        }



        private void LogManager_LineReceived(object sender, string e)
        {
            _logStringBuilder.AppendLine(e);
            this.OnPropertyChanged(nameof(LogText));
        }

        private void ReversalTool_DummyChanged(NameWakeupData dummy)
        {
            this.DummyName = dummy.CharName;
        }

        private void DisposeWindow()
        {
            this._reversalTool.StopReversalLoop();
            this._reversalTool.StopBlockReversalLoop();
            this._reversalTool.StopRandomBurstLoop();
            this._reversalTool.StopDummyLoop();
        }

        private void UpdateProcess(bool confirm = false)
        {
            string currentVersion = ConfigurationManager.AppSettings.Get("CurrentVersion");

            LogManager.Instance.WriteLine($"Current Version is {currentVersion}");
            try
            {
                this._updateManager.CleanOldFiles();
                var latestVersion = this._updateManager.CheckUpdates();

                if (latestVersion != null)
                {
                    if (!confirm || MessageBox.Show("A new version is available\r\nDo you want do download it?", "New version available", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        LogManager.Instance.WriteLine($"Found new version : v{latestVersion.Version}");
                        bool downloadSuccess = this._updateManager.DownloadUpdate(latestVersion);

                        if (downloadSuccess)
                        {
                            bool installSuccess = this._updateManager.InstallUpdate();

                            if (installSuccess)
                            {
                                this._updateManager.SaveVersion(latestVersion.Version);
                                this._updateManager.RestartApplication();
                            }
                        }
                    }

                }
                else
                {
                    LogManager.Instance.WriteLine("No updates");

                    if (confirm)
                    {
                        MessageBox.Show("Your version is up to date");
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteException(ex);
            }
        }
        #endregion

        #region ReplayTrigger

        public bool IsAsmReplayTypeChecked => ConfigurationManager.AppSettings.Get("ReplayTriggerType") == ReplayTriggerTypes.AsmInjection.ToString();
        public bool IsKeyStrokeReplayTypeChecked => ConfigurationManager.AppSettings.Get("ReplayTriggerType") == ReplayTriggerTypes.Keystroke.ToString();



        private RelayCommand<string> _changeReplayTypeCommand;
        public RelayCommand<string> ChangeReplayTypeCommand => _changeReplayTypeCommand ?? (_changeReplayTypeCommand = new RelayCommand<string>(ChangeReplayTypeCommandExecute, ChangeReplayTypeCommandCanExecute));
        private void ChangeReplayTypeCommandExecute(string parameter)
        {
            ReplayTriggerTypes replayTriggerType =
                (ReplayTriggerTypes) Enum.Parse(typeof(ReplayTriggerTypes), parameter);

            this._reversalTool.ChangeReplayTrigger(replayTriggerType);

            this.OnPropertyChanged(nameof(IsAsmReplayTypeChecked));
            this.OnPropertyChanged(nameof(IsKeyStrokeReplayTypeChecked));
        }
        private bool ChangeReplayTypeCommandCanExecute(string parameter)
        {
            return this._reversalTool.ReplayTriggerType.ToString() != parameter;
        }

        

        #endregion
    }
}
