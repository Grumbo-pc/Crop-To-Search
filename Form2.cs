using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Crop_To_Search
{
    public partial class Form2 : Form
    {
        private Timer fadeInTimer;
        private double targetOpacity = 0.5;
        private double fadeStep = 0.05;

        public Form2()
        {
            InitializeComponent();
            this.Visible = false; 
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
            this.BackColor = Color.Black;

            fadeInTimer = new Timer();
            fadeInTimer.Interval = 20;
            fadeInTimer.Tick += FadeInTimer_Tick;

            
            this.Visible = true;
            fadeInTimer.Start();
        }

        private void FadeInTimer_Tick(object sender, EventArgs e)
        {
            if (this.Opacity < targetOpacity)
            {
                this.Opacity += fadeStep;
                if (this.Opacity > targetOpacity)
                    this.Opacity = targetOpacity;
            }
            else
            {
                fadeInTimer.Stop();
            }
        }
    }
}

