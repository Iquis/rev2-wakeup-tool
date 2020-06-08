using System;
using System.Configuration;
using System.Windows.Forms;
using GGXrdWakeupDPUtil.Library;

namespace GGXrdWakeupDPUtil.Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private ReversalTool _reversalTool;
        private Keyboard.DirectXKeyStrokes _stroke;
        private UpdateManager _updateManager = new UpdateManager();

        private void button1_Click(object sender, EventArgs e)
        {
            var dummy = _reversalTool.GetDummy();

            MessageBox.Show($@"Current Dummy : {dummy.CharName}");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _reversalTool = new ReversalTool();

            _reversalTool.AttachToProcess();

            this._stroke = _reversalTool.GetReplayKeyStroke();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _reversalTool.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _reversalTool.PlayReversal();
        }



        private void button2_Click(object sender, EventArgs e)
        {
            _reversalTool.SetInputInSlot(1, textBox1.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _reversalTool.SetInputInSlot(2, textBox1.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            _reversalTool.SetInputInSlot(3, textBox1.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ReversalButtonEnable();
            var slotInput = _reversalTool.SetInputInSlot(1, textBox1.Text);

            _reversalTool.StartWakeupReversalLoop(slotInput);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FunctionButtonsDisable();
            _reversalTool.StopReversalLoop();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            RandomBurstButtonEnable();

            _reversalTool.StartRandomBurstLoop((int)numericUpDown1.Value, (int)numericUpDown2.Value, 1, trackBar1.Value);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            FunctionButtonsDisable();

            _reversalTool.StopRandomBurstLoop();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label3.Text = $@"{trackBar1.Value}%";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            UpdateProcess(true);
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
                    if (!confirm || MessageBox.Show("A new version is available\r\bDo you want do download it?", "New version available", MessageBoxButtons.YesNo) == DialogResult.Yes)
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

        private void button11_Click(object sender, EventArgs e)
        {
            BlockReversalButtonEnable();
            var slotInput = _reversalTool.SetInputInSlot(1, textBox1.Text);
            this._reversalTool.StartBlockReversalLoop(slotInput);
        }
        private void button12_Click(object sender, EventArgs e)
        {
            FunctionButtonsDisable();
            this._reversalTool.StopBlockReversalLoop();
        }

        private void ReversalButtonEnable()
        {
            button6.Enabled = false;
            button7.Enabled = true;
            button8.Enabled = false;
            button9.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;
        }
        private void BlockReversalButtonEnable()
        {
            button6.Enabled = false;
            button7.Enabled = true;
            button8.Enabled = false;
            button9.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = true;
        }

        private void RandomBurstButtonEnable()
        {
            button8.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button9.Enabled = true;
            numericUpDown1.Enabled = false;
            numericUpDown2.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;
        }

        private void FunctionButtonsDisable()
        {
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            button9.Enabled = true;

            numericUpDown1.Enabled = true;
            numericUpDown2.Enabled = true;

            button11.Enabled = true;
            button12.Enabled = true;
        }
    }
}
