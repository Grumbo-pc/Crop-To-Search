using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using NAudio.Wave;
using System.Diagnostics;
using System.IO;
using NAudio.Wave.SampleProviders;
using System.ComponentModel; 

namespace Crop_To_Search
{
    public partial class Form3 : Form
    {
        private WaveInEvent micSource;
        private WasapiLoopbackCapture speakerSource;
        private BufferedWaveProvider micBuffer;
        private BufferedWaveProvider speakerBuffer;
        private WaveFileWriter writer;
        private string outputFilePath;
        private Timer fadeTimer;
        private Color originalColor = Color.White;
        private Color fadeColor = ColorTranslator.FromHtml("#5880EA");
        private float fadeProgress = 0f;
        private bool fadingToCyan = true;
        private const int FadeDurationMs = 1000;
        private const int FadeStepMs = 20;

        public Form3()
        {
            InitializeComponent();
            if (ThemeHelper.IsWindowsInDarkMode())
            {
                originalColor = ColorTranslator.FromHtml("#282a2c");
                label1.ForeColor = Color.White;
                label2.ForeColor = Color.White;
            }
            else
            {
                originalColor = Color.White;
                label1.ForeColor = Color.Black; 
                label2.ForeColor = Color.Black;
            }
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Size = new Size(300, 80);
            this.BackColor = originalColor;
            fadeTimer = new Timer();
            fadeTimer.Interval = FadeStepMs;
            fadeTimer.Tick += FadeTimer_Tick;
            fadeTimer.Start();
        }
        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            float step = (float)FadeStepMs / FadeDurationMs;
            if (fadingToCyan)
                fadeProgress += step;
            else
                fadeProgress -= step;

            fadeProgress = Math.Max(0f, Math.Min(1f, fadeProgress));

            this.BackColor = LerpColor(originalColor, fadeColor, fadeProgress);

            if (fadeProgress >= 1f)
                fadingToCyan = false;
            else if (fadeProgress <= 0f)
                fadingToCyan = true;
        }

        private Color LerpColor(Color from, Color to, float t)
        {
            int r = (int)(from.R + (to.R - from.R) * t);
            int g = (int)(from.G + (to.G - from.G) * t);
            int b = (int)(from.B + (to.B - from.B) * t);
            return Color.FromArgb(r, g, b);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ApplyRoundCorners(10);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplyRoundCorners(10);
        }

        private void ApplyRoundCorners(int radius)
        {
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
                this.Region = new Region(path);
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            StartCombinedRecording();
        }

        public async void StartCombinedRecording()
        {
            outputFilePath = Path.Combine(Path.GetTempPath(), "result.wav");

            // Set up microphone input (44.1kHz mono)
            micSource = new WaveInEvent();
            micSource.WaveFormat = new WaveFormat(44100, 1);
            micBuffer = new BufferedWaveProvider(micSource.WaveFormat);

            micSource.DataAvailable += (s, a) =>
            {
                micBuffer.AddSamples(a.Buffer, 0, a.BytesRecorded);
            };

            // Set up speaker (loopback) input (system default format, often 48kHz stereo)
            speakerSource = new WasapiLoopbackCapture();
            speakerBuffer = new BufferedWaveProvider(speakerSource.WaveFormat);

            speakerSource.DataAvailable += (s, a) =>
            {
                speakerBuffer.AddSamples(a.Buffer, 0, a.BytesRecorded);
            };

            micSource.StartRecording();
            speakerSource.StartRecording();

            // Convert both sources to 44.1kHz stereo for mixing
            var micProvider = micBuffer.ToSampleProvider();
            if (micProvider.WaveFormat.SampleRate != 44100)
                micProvider = new WdlResamplingSampleProvider(micProvider, 44100);
            micProvider = micProvider.ToStereo();

            var speakerProvider = speakerBuffer.ToSampleProvider();
            if (speakerProvider.WaveFormat.SampleRate != 44100)
                speakerProvider = new WdlResamplingSampleProvider(speakerProvider, 44100);
            if (speakerProvider.WaveFormat.Channels == 1)
                speakerProvider = speakerProvider.ToStereo();

            var mixer = new MixingSampleProvider(new[] { micProvider, speakerProvider });
            mixer.ReadFully = true;

            writer = new WaveFileWriter(outputFilePath, WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));

            var buffer = new float[44100 * 2]; // 1 second buffer for stereo
            int secondsRecorded = 0;

            while (secondsRecorded < 7) //second timer set
            {
                int samplesRead = mixer.Read(buffer, 0, buffer.Length);
                if (samplesRead > 0)
                {
                    writer.WriteSamples(buffer, 0, samplesRead);
                }
                await Task.Delay(1000);
                secondsRecorded++;
            }

            micSource.StopRecording();
            speakerSource.StopRecording();
            writer.Dispose();

            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ACRCloudRecognitionTest.exe");
            var startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(startInfo);
            string filePath = Path.Combine(Path.GetTempPath(), "screenshot.png");
            File.Delete(filePath);
            Application.Exit();
        }
    }

    // Extension method must be in a static class
    public static class SampleProviderExtensions
    {
        public static ISampleProvider ToStereo(this ISampleProvider source)
        {
            if (source.WaveFormat.Channels == 2)
                return source;
            return new MonoToStereoSampleProvider(source);
        }
    }

    public class RoundButton : Button
    {
        [DefaultValue(10)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int CornerRadius { get; set; } = 10;

        public RoundButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = Color.White; // Or set to match your form's background
            this.TabStop = false;
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Paint parent background in corners for seamless blending
            if (Parent != null)
            {
                var g = pevent.Graphics;
                var state = g.Save();
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = this.ClientRectangle;
                g.TranslateTransform(-this.Left, -this.Top);
                var pe = new PaintEventArgs(g, new Rectangle(this.Left, this.Top, this.Width, this.Height));
                InvokePaintBackground(Parent, pe);
                InvokePaint(Parent, pe);
                g.Restore(state);
            }
            else
            {
                base.OnPaintBackground(pevent);
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = new GraphicsPath())
            {
                float r = CornerRadius;
                float w = this.Width;
                float h = this.Height;
                path.StartFigure();
                path.AddArc(0, 0, r, r, 180, 90);
                path.AddArc(w - r, 0, r, r, 270, 90);
                path.AddArc(w - r, h - r, r, r, 0, 90);
                path.AddArc(0, h - r, r, r, 90, 90);
                path.CloseFigure();

                this.Region = new Region(path);

                using (var brush = new SolidBrush(this.BackColor))
                    pevent.Graphics.FillPath(brush, path);

                TextRenderer.DrawText(pevent.Graphics, this.Text, this.Font, this.ClientRectangle, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }
    }
}
