﻿using System;
using System.Windows;
using GGXrdWakeupDPUtil.Commands;
using GGXrdWakeupDPUtil.Library;

namespace GGXrdWakeupDPUtil.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ReversalTool _reversalTool = new ReversalTool();




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

        private bool _wakeupReversalStarted;
        public bool WakeupReversalStarted
        {
            get => _wakeupReversalStarted;
            set
            {
                _wakeupReversalStarted = value;
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

        private bool _blockstunReversalStarted;
        public bool BlockstunReversalStarted
        {
            get => _blockstunReversalStarted;
            set
            {
                _blockstunReversalStarted = value;
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
            this.WakeupReversalStarted = true;
        }
        private bool CanStartWakeupReversal()
        {
            return !this.WakeupReversalStarted && this.IsWakeupReversalInputValid && !string.IsNullOrEmpty(this.WakeupReversalInput);
        }
        #endregion

        #region StopReversalCommand
        private RelayCommand _stopWakeupReversalCommand;

        public RelayCommand StopWakeupReversalCommand => this._stopWakeupReversalCommand ?? (this._stopWakeupReversalCommand = new RelayCommand(this.StopWakeupReversal, this.CanStopWakeupReversal));

        private void StopWakeupReversal()
        {
            this._reversalTool.StopReversalLoop();
            this.WakeupReversalStarted = false;
        }

        private bool CanStopWakeupReversal()
        {
            return this.WakeupReversalStarted;
        }

        #endregion

        #region StartBlockstunReversalCommand
        private RelayCommand _startBlockstunReversalCommand;
        public RelayCommand StartBlockstunReversalCommand => this._startBlockstunReversalCommand ?? (this._startBlockstunReversalCommand = new RelayCommand(this.StartBlockstunReversal, this.CanStartBlockstunReversal));
        private void StartBlockstunReversal()
        {
            SlotInput slotInput = this._reversalTool.SetInputInSlot(this.BlockstunReversalSlotNumber, this.BlockstunReversalInput);
            this._reversalTool.StartBlockReversalLoop(slotInput);
            this.BlockstunReversalStarted = true;
        }
        private bool CanStartBlockstunReversal()
        {
            return !this.BlockstunReversalStarted && this.IsBlockstunReversalInputValid && !string.IsNullOrEmpty(this.BlockstunReversalInput);
        }
        #endregion

        #region StopReversalCommand
        private RelayCommand _stopBlockstunReversalCommand;

        public RelayCommand StopBlockstunReversalCommand => this._stopBlockstunReversalCommand ?? (this._stopBlockstunReversalCommand = new RelayCommand(this.StopBlockstunReversal, this.CanStopBlockstunReversal));

        private void StopBlockstunReversal()
        {
            this._reversalTool.StopBlockReversalLoop();
            this.BlockstunReversalStarted = false;
        }

        private bool CanStopBlockstunReversal()
        {
            return this.BlockstunReversalStarted;
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

        #endregion


        #region MyRegion

        private void InitializeWindow()
        {
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

        }

        private void DisposeWindow()
        {
            this._reversalTool.StopReversalLoop();
            this._reversalTool.StopBlockReversalLoop();
            this._reversalTool.StopRandomBurstLoop();
        }
        #endregion
    }
}
