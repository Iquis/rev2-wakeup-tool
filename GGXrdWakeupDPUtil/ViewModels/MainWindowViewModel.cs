using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
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
                this.OnPropertyChanged("ReversalErrorVisibility");
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
                this.OnPropertyChanged("ReversalErrorVisibility");
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
                    $"Guilty Gear not found open!  Remember, be in training mode paused when you open this program.  This program will now close.";
                LogManager.Instance.WriteLine(message);
                MessageBox.Show($"{message}{Environment.NewLine}{exception.Message}");
                Application.Current.Shutdown();
                return;
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
