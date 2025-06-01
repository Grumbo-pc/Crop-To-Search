using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Crop_To_Search
{
    public partial class Terms_And_Conditions : Form
    {
        public Terms_And_Conditions()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None; 
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Size = new Size(450, 320); 
            SetRoundedForm(10); 

            Panel scrollPanel = new Panel
            {
                AutoScroll = true,
                Width = 420,
                Height = 250,
                Left = 15,
                Top = 10,
                BorderStyle = BorderStyle.None 
            };

            Label termsLabel = new Label
            {
                Text = "Terms And Conditions\n\n" +
                "By using Crop-To-Search, you acknowledge and agree to the following:\n\n" +
                "- The app enables screenshot-based searches using third-party services.\n" +
                "- The app enables Song-based searches using third-party services.\n" +
                "- Use caution when handling sensitive data on your screen Ex: Credit Card Info\n" +
                "- The developer is not responsible for search results, external content, or errors.\n" +
                "- The app is open-source and available on GitHub: https://github.com/Grumbo-pc/Crop-To-Search\n"+
                "- The app does not store the information gathered from your system BUT third party APIs the app usess such as IMGBB (for image storage before google lens) or ACRcloud (for song recognition) MAY store the information for a limited amount of time\n"+
                "- The HTTP requests sent are handeled using the HTTPS protocol\n"+
                "- The developer provides the App as is, without warranties or guarantees of any kind.\n" +
                "- You assume full responsibility for the screenshots, images, and audio data submitted through the App.\n" +
                "- Third-Party Services Disclaimer\n- The App interacts with Google Lens and third-party APIs, which operate independently.\n- The developer does not control or store any data processed by these services and cannot guarantee their accuracy, security, or availability.\n- By using the App, you agree to abide by the terms and privacy policies of external services, including Google Lens.\n" +
                "- The developer is not liable for any privacy risks associated with third-party services.\n" +
                "- I (as the developer) am not afillieated with google or any other third party this app is mainly an open source experiment\n\n" +
                "If you do not agree to these terms, please exit the application.",
                AutoSize = true,
                MaximumSize = new Size(400, 0),
                Font = new Font("Segoe UI", 18, FontStyle.Regular)
            };

            scrollPanel.Controls.Add(termsLabel);

           
            Button acceptButton = new Button
            {
                Text = "Accept",
                Width = 120,
                Height = 40, 
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Location = new Point(100, 270) 
            };
            acceptButton.Click += (sender, e) =>
            {
                File.WriteAllText("accepted", "User has accepted the terms.");
                this.Close();
            };

            Button denyButton = new Button
            {
                Text = "Deny",
                Width = 120,
                Height = 40, 
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Location = new Point(230, 270) 
            };
            denyButton.Click += (sender, e) => Application.Exit();

            this.Controls.Add(scrollPanel);
            this.Controls.Add(acceptButton);
            this.Controls.Add(denyButton);
        }

        private void SetRoundedForm(int radius)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
            path.AddArc(this.Width - (radius * 2), 0, radius * 2, radius * 2, 270, 90);
            path.AddArc(this.Width - (radius * 2), this.Height - (radius * 2), radius * 2, radius * 2, 0, 90);
            path.AddArc(0, this.Height - (radius * 2), radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            this.Region = new Region(path);
        }
    }
}