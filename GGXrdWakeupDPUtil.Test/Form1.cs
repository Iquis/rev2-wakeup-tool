using System;
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
            _reversalTool.PlayReversal(this._stroke);
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
            button6.Enabled = false;
            button7.Enabled = true;
            button8.Enabled = false;
            button9.Enabled = false;
            var slotInput = _reversalTool.SetInputInSlot(1, textBox1.Text);

            _reversalTool.StartReversalLoop(slotInput);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button6.Enabled = true;
            button7.Enabled = false;
            button8.Enabled = true;
            button9.Enabled = false;
            _reversalTool.StopReversalLoop();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            button8.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button9.Enabled = true;
            numericUpDown1.Enabled = false;
            numericUpDown2.Enabled = false;

            _reversalTool.StartRandomBurstLoop((int)numericUpDown1.Value, (int)numericUpDown2.Value, 1, trackBar1.Value);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            button8.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button9.Enabled = false;
            numericUpDown1.Enabled = true;
            numericUpDown2.Enabled = true;

            _reversalTool.StopRandomBurstLoop();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label3.Text = $@"{trackBar1.Value}%";
        }
    }
}
