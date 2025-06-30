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
        private System.Windows.Forms.Timer progressTimer;
        private int fakeProgress;
        private Timer slideDownTimer;
        private int targetTop;
        private Label offlineErrorLabel;

        public Form1()
        {
            InitializeComponent();
            progressBarTop = new CustomColorProgressBar();
            InitializeWebView();
            SetStartupPosition();
            Load += new EventHandler(Form1_Load);
            this.FormBorderStyle = FormBorderStyle.None;
            ShowForm2();

            if (ThemeHelper.IsWindowsInDarkMode())
            {
                this.BackColor = Color.Black;
                webView.BackColor = Color.Black;
                pictureBox1.Show();
                pictureBox3.Show();
                this.Icon = new Icon("crop-to-search-dark.ico");
            }
            else
            {
                this.Icon = new Icon("crop-to-search.ico");
            }
        }

        private void SetStartupPosition()
        {
            this.Width = 1200;
            this.Height = 800;

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            int x = (screenWidth - this.Width) / 2;
            int y = (screenHeight - this.Height) / 2;

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(x, y);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            
            this.Top = -this.Height;
            this.Left = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
            targetTop = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 4; 

            slideDownTimer = new Timer();
            slideDownTimer.Interval = 10; 
            slideDownTimer.Tick += SlideDownTimer_Tick;
            slideDownTimer.Start();
            this.CreateRoundRegion();
            this.Hide();
            string imageUrl = await CaptureAndUploadScreenshot();
            if (!string.IsNullOrEmpty(imageUrl))
                DisplayGoogleLensResults(imageUrl);
        }

        private void SlideDownTimer_Tick(object sender, EventArgs e)
        {
            if (this.Top < targetTop)
            {
                this.Top += 75; // Speed of animation
                if (this.Top > targetTop)
                    this.Top = targetTop;
            }
            else
            {
                slideDownTimer.Stop();
            }
        }

        private void InitializeWebView()
        {
            webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            webView.Visible = false;
            this.Controls.Add(webView);
            webView.BringToFront();

            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
        }

        private void WebView_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                webView.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
                webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted_Failure;
            }
        }

        private void CoreWebView2_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            if (ThemeHelper.IsWindowsInDarkMode())
            {
                webView.DefaultBackgroundColor = System.Drawing.Color.Black;
                webView.CoreWebView2.ExecuteScriptAsync(
                    "document.body.style.backgroundColor = '#000';"
                );
            }
            ShowProgressBar();
        }

        private void CoreWebView2_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            HideProgressBar();

            if (ThemeHelper.IsWindowsInDarkMode())
            {
                webView.CoreWebView2.ExecuteScriptAsync(
                    "document.body.style.backgroundColor = '';"
                );
            }
        }

        private void CoreWebView2_NavigationCompleted_Failure(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                ShowOfflineErrorLabel();
            }
        }

        private void ShowProgressBar()
        {
            if (ThemeHelper.IsWindowsInDarkMode())
            {
                progressBarTop.BackColor = ColorTranslator.FromHtml("#1f1f1f");
            }
            else
            {
                progressBarTop.BackColor = SystemColors.Control;
            }

            progressBarTop.Dock = DockStyle.Top;
            progressBarTop.Height = 8;
            progressBarTop.Visible = false;
            this.Controls.Add(progressBarTop);
            progressBarTop.BringToFront();
            progressBarTop.Visible = true;
            progressBarTop.Value = 0;
            fakeProgress = 0;

            if (progressTimer == null)
            {
                progressTimer = new System.Windows.Forms.Timer();
                progressTimer.Interval = 30;
                progressTimer.Tick += ProgressTimer_Tick;
            }
            progressTimer.Start();
        }

        private void HideProgressBar()
        {
            if (progressTimer != null)
                progressTimer.Stop();
            progressBarTop.Value = progressBarTop.Maximum;
            var t = new System.Windows.Forms.Timer();
            t.Interval = 300;
            t.Tick += (s, e) =>
            {
                progressBarTop.Visible = false;
                progressBarTop.Value = 0;
                t.Stop();
                t.Dispose();
            };
            t.Start();
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (fakeProgress < 90)
            {
                fakeProgress += 2;
                progressBarTop.Value = Math.Min(fakeProgress, 90);
            }
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
            int maxRetries = 3;

            using (var client = new HttpClient())
            {
                for (int i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        using (var content = new MultipartFormDataContent())
                        {
                            content.Add(new StringContent(apiKey), "key");
                            content.Add(new ByteArrayContent(File.ReadAllBytes(filePath)), "image", "screenshot.png");

                            var response = await client.PostAsync("https://api.imgbb.com/1/upload", content);
                            response.EnsureSuccessStatusCode();

                            var json = await response.Content.ReadAsStringAsync();
                            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                            return result.data.url;
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        if (i == maxRetries - 1)
                        {
                            ShowOfflineErrorLabel();
                            return null;
                        }
                        await Task.Delay(2000);
                    }
                    catch (Exception)
                    {
                        ShowOfflineErrorLabel();
                        return null;
                    }
                }
            }

            return null;
        }

        private void DisplayGoogleLensResults(string imageUrl)
        {
            string googleLensUrl = $"https://lens.google.com/uploadbyurl?url={imageUrl}";
            webView.Source = new Uri(googleLensUrl);
            webView.Visible = true;
            webView.BringToFront();
            this.Show();
        }

        private void ShowOfflinePage()
        {
            string offlinePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "offline.html");
            if (File.Exists(offlinePath))
            {
                webView.Source = new Uri(offlinePath);
                webView.Visible = true;
                webView.BringToFront();
                this.Show();
            }
            else
            {
                MessageBox.Show("Offline page not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateRoundRegion()
        {
            int radius = 10;
            using (var path = new GraphicsPath())
            {
                float r = radius;
                float w = this.Width;
                float h = this.Height;
                path.StartFigure();
                path.AddArc(0, 0, r, r, 180, 90);
                path.AddArc(w - r, 0, r, r, 270, 90);
                path.AddArc(w - r, h - r, r, r, 0, 90);
                path.AddArc(0, h - r, r, r, 90, 90);
                path.CloseFigure();

                using (var g = this.CreateGraphics())
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                }
                this.Region = new Region(path);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CreateRoundRegion();
        }

        private void ShowForm2()
        {
            Form2 form2 = new Form2();
            form2.Show();
            this.BringToFront();
        }

        private void ShowErrorMessage(string message)
        {
            Label errorLabel = new Label();
            errorLabel.Text = message;
            errorLabel.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            errorLabel.ForeColor = Color.Red;
            errorLabel.AutoSize = true;
            errorLabel.BackColor = Color.Transparent;
            errorLabel.TextAlign = ContentAlignment.MiddleCenter;

            this.Invoke((MethodInvoker)(() =>
            {
                errorLabel.Left = (this.ClientSize.Width - errorLabel.Width) / 2;
                errorLabel.Top = this.ClientSize.Height - 100;
                this.Controls.Add(errorLabel);
                errorLabel.BringToFront();
            }));
        }

        private void ShowOfflineErrorLabel()
        {
            if (pictureBox3 != null)
            {
                this.Controls.Remove(pictureBox3);
                pictureBox3.Dispose();
                pictureBox3 = null;
            }
            if (pictureBox4 != null)
            {
                this.Controls.Remove(pictureBox4);
                pictureBox4.Dispose();
                pictureBox4 = null;
            }

            if (offlineErrorLabel == null)
            {
                offlineErrorLabel = new Label();
                offlineErrorLabel.Text = "You are either offline or currently one \n of the APIs is not functioning correctly.";
                offlineErrorLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 30, FontStyle.Bold);
                offlineErrorLabel.TextAlign = ContentAlignment.MiddleCenter;
                offlineErrorLabel.BackColor = Color.Transparent;
                offlineErrorLabel.AutoSize = true;
                this.Controls.Add(offlineErrorLabel);
            }

            // Set label color based on theme
            if (ThemeHelper.IsWindowsInDarkMode())
            {
                offlineErrorLabel.ForeColor = Color.White;
            }
            else
            {
                offlineErrorLabel.ForeColor = Color.Black;
            }

            if (pictureBox1 != null)
            {
                offlineErrorLabel.Left = (this.ClientSize.Width - offlineErrorLabel.Width) / 2;
                offlineErrorLabel.Top = pictureBox1.Bottom + 20;
            }
            else
            {
                offlineErrorLabel.Left = (this.ClientSize.Width - offlineErrorLabel.Width) / 2;
                offlineErrorLabel.Top = (this.ClientSize.Height - offlineErrorLabel.Height) / 2;
            }

            offlineErrorLabel.BringToFront();
            offlineErrorLabel.Visible = true;
        }
    }
}
