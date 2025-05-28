using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace Crop_To_Search
{

    public partial class Form1 : Form
    {
        private WebView2 webView;
        private ProgressBar progressBar;
        

        public Form1()
        {
            InitializeComponent();
            InitializeProgressBar();
            InitializeWebView();
            SetStartupPosition();
            Load += new EventHandler(Form1_Load);
            this.FormBorderStyle = FormBorderStyle.None;
            ShowForm2();
        }

        private void SetStartupPosition()
        {
            this.Width = 1200; // Increased width
            this.Height = 800; // Increased height

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            int x = (screenWidth - this.Width) / 2;
            int y = (screenHeight - this.Height) / 2; // Center vertically

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(x, y);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
            progressBar.Show();
            string imageUrl = await CaptureAndUploadScreenshot();
            DisplayGoogleLensResults(imageUrl);
        }

        private void InitializeWebView()
        {
            webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            webView.Visible = false; // Hide initially
            this.Controls.Add(webView);
            webView.BringToFront();
        }

        private void InitializeProgressBar()
        {
            progressBar = new ProgressBar();
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.Dock = DockStyle.Bottom;
            progressBar.MarqueeAnimationSpeed = 1; // Adjust as needed
            this.Controls.Add(progressBar);
        }

        private async Task<string> CaptureAndUploadScreenshot()
        {
            string filePath = Path.Combine(Path.GetTempPath(), "screenshot.png");

            int width = Screen.PrimaryScreen.Bounds.Width;
            int height = Screen.PrimaryScreen.Bounds.Height;

            using (Bitmap bitmap = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, new Size(width, height));
                }
                bitmap.Save(filePath, ImageFormat.Png);
            }

            string apiKey = "c4ef29b400eb25b21651c444349d0b90";
            using (var client = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(apiKey), "key");
                    content.Add(new ByteArrayContent(File.ReadAllBytes(filePath)), "image", "screenshot.png");
                    var response = await client.PostAsync("https://api.imgbb.com/1/upload", content);
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    return result.data.url;
                }
            }
        }

        private void DisplayGoogleLensResults(string imageUrl)
        {
            string googleLensUrl = $"https://lens.google.com/uploadbyurl?url={imageUrl}";
            webView.Source = new Uri(googleLensUrl);
            webView.Visible = true; // Show WebView
            progressBar.Hide(); // Hide progress bar
            webView.BringToFront(); // Bring WebView to the front
            this.Show();
        }


        // Event handler for pictureBox2 Click
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Add your logic here if needed
        }

        // Event handler for Form1 Load
        private void Form1_Load_1(object sender, EventArgs e)
        {
            // Add your logic here if needed
            this.CreateRoundRegion();
        }

        // Method to create round corners for the form
        private void CreateRoundRegion()
        {
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            int cornerRadius = 10; // Adjust for desired corner radius
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            path.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            path.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);

            this.Region = new Region(path);
        }
        private void ShowForm2()
        {
            Form2 form2 = new Form2();
            form2.Show();
            this.BringToFront(); // Bring Form1 to the Top
        }
    }
}
