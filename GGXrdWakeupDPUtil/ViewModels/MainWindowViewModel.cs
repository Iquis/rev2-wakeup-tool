using System;
using System.Configuration;
using System.Text;
using System.Windows;
using GGXrdWakeupDPUtil.Commands;
using GGXrdWakeupDPUtil.Library;

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
                AddUpdateAppSettings("AutoUpdate", value ? "1" : "0");


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
                this.IsWakeupReversalInputValid = string.IsNullOrEmpty(this.WakeupReversalInput) || this._reversalTool.CheckValidInput(this.WakeupReversalInput);
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(WakeupReversalErrorVisibility));
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
                this.OnPropertyChanged(nameof(WakeupReversalErrorVisibility));
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
                this.IsBlockstunReversalInputValid = string.IsNullOrEmpty(this.BlockstunReversalInput) || this._reversalTool.CheckValidInput(this.BlockstunReversalInput);
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(BlockstunReversalErrorVisibility));
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
                this.OnPropertyChanged(nameof(BlockstunReversalErrorVisibility));
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
            SlotInput slotInput = this._reversalTool.SetInputInSlot(this.WakeupReversalSlotNumber, this.WakeupReversalInput);
            this._reversalTool.StartWakeupReversalLoop(slotInput);
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
            SlotInput slotInput = this._reversalTool.SetInputInSlot(this.BlockstunReversalSlotNumber, this.BlockstunReversalInput);
            this._reversalTool.StartBlockReversalLoop(slotInput);
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

        private void DisposeWindow()
        {
            this._reversalTool.StopReversalLoop();
            this._reversalTool.StopBlockReversalLoop();
            this._reversalTool.StopRandomBurstLoop();
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
        private void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException ex)
            {
                LogManager.Instance.WriteLine("Error writing app settings");
                LogManager.Instance.WriteException(ex);
            }
        }
        #endregion
    }
}
