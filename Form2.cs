using System;
using System.Drawing;
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
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Make Form2 fill the screen
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = false; // Ensure Form1 is above Form2

            // Set initial opacity to 0 (fully transparent)
            this.Opacity = 0;

            // Initialize and start the timer
            delayTimer = new Timer();
            delayTimer.Interval = 1000; // 1 second delay
            delayTimer.Tick += DelayTimer_Tick;
            delayTimer.Start();
        }

        private void DelayTimer_Tick(object sender, EventArgs e)
        {
            // Stop the timer
            delayTimer.Stop();

            // Make Form2 semi-transparent black
            this.BackColor = Color.Black;
            this.Opacity = 0.5;
        }
    }
}
