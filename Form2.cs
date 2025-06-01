using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Crop_To_Search
{
    public partial class Form2 : Form
    {
        private Timer delayTimer;

        public Form2()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form2_Load);
            this.Click += Form2_Click;
            foreach (Control c in this.Controls)
            {
                c.Click += Form2_Click;
            }
        }

        private void Form2_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(Path.GetTempPath(), "screenshot.png");
            File.Delete(filePath);
            Application.Exit();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = false;
            this.Opacity = 0;
            delayTimer = new Timer();
            delayTimer.Interval = 1000;
            delayTimer.Tick += DelayTimer_Tick;
            delayTimer.Start();
        }

        private void DelayTimer_Tick(object sender, EventArgs e)
        {
            delayTimer.Stop();
            this.BackColor = Color.Black;
            this.Opacity = 0.5;
        }
    }
}

