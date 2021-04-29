using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using GGXrdWakeupDPUtil.Library;

namespace GGXrdWakeupDPUtil
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Window1
    {
        //TODO Use mvvm
        public Window1()
        {
            InitializeComponent();

            this.InputTextBox.TextChanged += inputTextBox_TextChanged;
            this.BlockReversalInputTextBox.TextChanged += blockReversalInputTextBox_TextChanged;
        }

        private readonly ReversalTool _reversalTool = new ReversalTool();
        private readonly UpdateManager _updateManager = new UpdateManager();
        private readonly bool _autoUpdate = ConfigurationManager.AppSettings.Get("AutoUpdate") == "1";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _reversalTool.DummyChanged += _reversalTool_DummyChanged;
            _reversalTool.ReversalLoopErrorOccured += _reversalTool_ReversalLoopErrorOccured;
            _reversalTool.RandomBurstlLoopErrorOccured += _reversalTool_RandomBurstlLoopErrorOccured;
            LogManager.Instance.LineReceived += LogManager_LineReceived;




#if !DEBUG
            if (_autoUpdate)
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
                    $"Guilty Gear not found open!  Remember, be in training mode paused when you open this program.  This program will now close.";
                LogManager.Instance.WriteLine(message);
                MessageBox.Show($"{message}{Environment.NewLine}{exception.Message}");
                Application.Current.Shutdown();
                return;
            }



            RefreshBurstInfo();


        }


        private void Window_Closed(object sender, EventArgs e)
        {
            _reversalTool?.Dispose();
        }

        #region Dummy

        private void _reversalTool_DummyChanged(NameWakeupData dummy)
        {
            SetDummyName(dummy.CharName);
        }

        #endregion

        #region Reversal

        private void _reversalTool_ReversalLoopErrorOccured(Exception ex)
        {
            StopReversal();
        }

        private void enableButton_Click(object sender, RoutedEventArgs e)
        {
            int slotNumber = 0;

            if (Slot1R.IsChecked != null && Slot1R.IsChecked.Value)
            {
                slotNumber = 1;
            }
            else if (Slot2R.IsChecked != null && Slot2R.IsChecked.Value)
            {
                slotNumber = 2;
            }
            else if (Slot3R.IsChecked != null && Slot3R.IsChecked.Value)
            {
                slotNumber = 3;
            }


            var slotInput = new SlotInput(InputTextBox.Text);
            _reversalTool.SetInputInSlot(slotNumber, slotInput);



            _reversalTool.StartWakeupReversalLoop(slotInput, 100,true);


            this.ReversalActivation(true);
        }

        private void disableButton_Click(object sender, RoutedEventArgs e)
        {
            StopReversal();
        }



        private void inputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TODO add watermark

            var text = this.InputTextBox.Text;
            SlotInput slotInput = new SlotInput(text);

            if (slotInput.IsReversalValid)
            {
                ErrorTextBlock.Visibility = Visibility.Hidden;
                EnableButton.IsEnabled = true;
            }
            else
            {
                ErrorTextBlock.Visibility = Visibility.Visible;

                if (!string.IsNullOrEmpty(text))
                {
                    ErrorTextBlock.Text = "Invalid Input";
                }

                EnableButton.IsEnabled = false;
            }
        }





        private void SetDummyName(string dummyName)
        {
            Dispatcher.Invoke(() =>
            {
                DummyTextBlock.Text = $"Current Dummy: {dummyName}";
            });
        }

        private void StopReversal()
        {
            Dispatcher.Invoke(() =>
            {
                this.ReversalActivation(false);
            });

            _reversalTool.StopReversalLoop();

        }


        #endregion

        #region Block Reversal
        private void blockReversalEnableButton_Click(object sender, RoutedEventArgs e)
        {
            int slotNumber = 0;

            if (BlockReversalSlot1R.IsChecked != null && BlockReversalSlot1R.IsChecked.Value)
            {
                slotNumber = 1;
            }
            else if (BlockReversalSlot2R.IsChecked != null && BlockReversalSlot2R.IsChecked.Value)
            {
                slotNumber = 2;
            }
            else if (BlockReversalSlot3R.IsChecked != null && BlockReversalSlot3R.IsChecked.Value)
            {
                slotNumber = 3;
            }

            var slotInput = new SlotInput(BlockReversalInputTextBox.Text);
            _reversalTool.SetInputInSlot(slotNumber, slotInput);



            _reversalTool.StartBlockReversalLoop(slotInput, 100, 0);

            this.BlockReversalActivation(true);
        }
        private void blockReversaldisableButton_Click(object sender, RoutedEventArgs e)
        {
            StopBlockReversal();
        }
        private void blockReversalInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TODO add watermark

            var text = this.BlockReversalInputTextBox.Text;

            SlotInput slotInput = new SlotInput(text);

            if (slotInput.IsReversalValid)
            {
                BlockReversalErrorTextBlock.Visibility = Visibility.Hidden;
                BlockReversalEnableButton.IsEnabled = true;
            }
            else
            {
                BlockReversalErrorTextBlock.Visibility = Visibility.Visible;

                if (!string.IsNullOrEmpty(text))
                {
                    BlockReversalErrorTextBlock.Text = "Invalid Input";
                }

                BlockReversalEnableButton.IsEnabled = false;
            }
        }


        private void StopBlockReversal()
        {
            Dispatcher.Invoke(() =>
            {
                this.BlockReversalActivation(false);
            });

            _reversalTool.StopBlockReversalLoop();

        }
        #endregion

        #region Burst
        private void enableBurstButton_Click(object sender, RoutedEventArgs e)
        {
            int slotNumber = 0;

            if (Slot1RBurst.IsChecked != null && Slot1RBurst.IsChecked.Value)
            {
                slotNumber = 1;
            }
            else if (Slot2RBurst.IsChecked != null && Slot2RBurst.IsChecked.Value)
            {
                slotNumber = 2;
            }
            else if (Slot3RBurst.IsChecked != null && Slot3RBurst.IsChecked.Value)
            {
                slotNumber = 3;
            }

            int min = NumericUpDownMinBurst.Value;
            int max = NumericUpDownMaxBurst.Value;
            int burstPercentage = (int)BurstSlider.Value;

            _reversalTool.StartRandomBurstLoop(min, max, slotNumber, burstPercentage);

            this.BurstActivation(true);

        }

        private void disableBurstButton_Click(object sender, RoutedEventArgs e)
        {
            StopBurst();
        }
        private void StopBurst()
        {
            Dispatcher.Invoke(() =>
            {
                this.BurstActivation(false);
            });

            _reversalTool.StopRandomBurstLoop(); ;

        }
        private void AppendLog(string message)
        {
            Dispatcher.Invoke(() =>
            {
                LogTextBox.AppendText($"{message}{Environment.NewLine}");
            });
        }
        private void NumericUpDownMinBurst_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (NumericUpDownMinBurst != null && NumericUpDownMaxBurst != null)
            {
                NumericUpDownMaxBurst.Minimum = NumericUpDownMinBurst.Value;
            }


            RefreshBurstInfo();
        }
        private void NumericUpDownMaxBurst_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (NumericUpDownMinBurst != null && NumericUpDownMaxBurst != null)
            {
                NumericUpDownMinBurst.Maximum = NumericUpDownMaxBurst.Value;
            }

            RefreshBurstInfo();
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RefreshBurstInfo();
        }


        private void RefreshBurstInfo()
        {

            if (NumericUpDownMinBurst != null && NumericUpDownMaxBurst != null && BurstSlider != null)
            {
                int min = NumericUpDownMinBurst.Value;
                int max = NumericUpDownMaxBurst.Value;
                int burstPercentage = (int)BurstSlider.Value;

                string text;

                if (burstPercentage == 100)
                {
                    text = $"The dummy will burst randomly between {min} and {max} hit combo";
                }
                else
                {
                    text = min == max ?
                        $"- The dummy will burst randomly at {min} hit combo ({burstPercentage}% chance)" :
                        $"- The dummy will burst randomly between {min} and {max} hit combo ({burstPercentage}% chance)";
                    text += Environment.NewLine;
                    text += $"- The dummy won't burst at all ({100 - burstPercentage}% chance)";
                }

                Dispatcher.Invoke(() =>
                {
                    BurstInfoTextBlock.Text = text;
                    BurstPercentageLabel.Content = $"{burstPercentage}%";
                });
            }
        }

        private void _reversalTool_RandomBurstlLoopErrorOccured(Exception ex)
        {
            StopBurst();
        }
        private void LogManager_LineReceived(object sender, string e)
        {
            AppendLog(e);
        }



        #endregion

        #region Menu
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdateProcess(confirm: true);
        }

        #endregion


        private void ReversalActivation(bool isEnabled)
        {
            EnableButton.IsEnabled = !isEnabled;
            DisableButton.IsEnabled = isEnabled;
            Slot1R.IsEnabled = !isEnabled;
            Slot2R.IsEnabled = !isEnabled;
            Slot3R.IsEnabled = !isEnabled;
            InputTextBox.IsEnabled = !isEnabled;

            BurstTabItem.IsEnabled = !isEnabled;
            BlockReversalTabItem.IsEnabled = !isEnabled;

        }
        private void BurstActivation(bool isEnabled)
        {
            EnableBurstButton.IsEnabled = !isEnabled;
            DisableBurstButton.IsEnabled = isEnabled;
            Slot1RBurst.IsEnabled = !isEnabled;
            Slot2RBurst.IsEnabled = !isEnabled;
            Slot3RBurst.IsEnabled = !isEnabled;

            ReversalTabItem.IsEnabled = !isEnabled;
            BlockReversalTabItem.IsEnabled = !isEnabled;
        }
        private void BlockReversalActivation(bool isEnabled)
        {
            BlockReversalEnableButton.IsEnabled = !isEnabled;
            BlockReversalDisableButton.IsEnabled = isEnabled;
            BlockReversalSlot1R.IsEnabled = !isEnabled;
            BlockReversalSlot2R.IsEnabled = !isEnabled;
            BlockReversalSlot3R.IsEnabled = !isEnabled;
            BlockReversalInputTextBox.IsEnabled = !isEnabled;
            BurstTabItem.IsEnabled = !isEnabled;
            ReversalTabItem.IsEnabled = !isEnabled;
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
                    if (!confirm || MessageBox.Show("A new version is available\r\bDo you want do download it?", "New version available", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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


    }
}
