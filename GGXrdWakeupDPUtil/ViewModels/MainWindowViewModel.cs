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

        private int _slotNumber = 1;
        public int SlotNumber
        {
            get => _slotNumber;
            set
            {
                _slotNumber = value;
                this.OnPropertyChanged();
            }
        }


        #region Reversal
        private bool _reversalStarted;
        public bool ReversalStarted
        {
            get => _reversalStarted;
            set
            {
                _reversalStarted = value;
                this.OnPropertyChanged();
            }
        }

        private string _reversalInput;
        public string ReversalInput
        {
            get => _reversalInput;
            set
            {
                _reversalInput = value;
                this.IsReversalInputValid = string.IsNullOrEmpty(this.ReversalInput) || this._reversalTool.CheckValidInput(this.ReversalInput);
                this.OnPropertyChanged();
                this.OnPropertyChanged("ReversalErrorVisibility");
            }
        }

        private bool _isReversalInputValid;
        public bool IsReversalInputValid
        {
            get => _isReversalInputValid;
            set
            {
                _isReversalInputValid = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("ReversalErrorVisibility");
            }
        }

        public Visibility ReversalErrorVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(this.ReversalInput) || IsReversalInputValid)
                {
                    return Visibility.Hidden;
                }

                return Visibility.Visible;
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

        #region StartReversalCommand
        private RelayCommand _startReversalCommand;
        public RelayCommand StartReversalCommand => this._startReversalCommand ?? (this._startReversalCommand = new RelayCommand(this.StartReversal, this.CanStartReversal));
        private void StartReversal()
        {
            SlotInput slotInput = this._reversalTool.SetInputInSlot(this.SlotNumber, this.ReversalInput);
            this._reversalTool.StartReversalLoop(slotInput);
            this.ReversalStarted = true;
        }
        private bool CanStartReversal()
        {
            return !this.ReversalStarted && this.IsReversalInputValid && !string.IsNullOrEmpty(this.ReversalInput);
        }
        #endregion

        #region StopReversalCommand
        private RelayCommand _stopReversalCommand;

        public RelayCommand StopReversalCommand => this._stopReversalCommand ?? (this._stopReversalCommand = new RelayCommand(this.StopReversal, this.CanStopReversal));

        private void StopReversal()
        {
            this._reversalTool.StopReversalLoop();
            this.ReversalStarted = false;
        }

        private bool CanStopReversal()
        {
            return this.ReversalStarted;
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
