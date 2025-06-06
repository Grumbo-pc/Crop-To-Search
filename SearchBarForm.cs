﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace Crop_To_Search
{
    public partial class SearchBarForm : Form
    {

        private Form1 _owner;
        private Form3 _form3Instance; 

        public void PositionBelowForm(Form1 form1)
        {
            Rectangle screenBounds = Screen.FromControl(form1).WorkingArea;
            int form1Bottom = form1.Bounds.Bottom;
            int screenBottom = screenBounds.Bottom;
            int targetY = form1Bottom + (screenBottom - form1Bottom - this.Height) / 2;
            int targetX = form1.Left + (form1.Width - this.Width) / 2;
            this.Location = new Point(targetX, targetY);
            if (_form3Instance == null || _form3Instance.IsDisposed)
            {
                _form3Instance = new Form3();
                _form3Instance.FormClosed += (s, args) => _form3Instance = null;
                Point mousePos = Cursor.Position;
                int x = mousePos.X - (_form3Instance.Width / 2);
                int y = mousePos.Y - _form3Instance.Height - 10; 
                Rectangle screen = Screen.FromPoint(mousePos).WorkingArea;
                x = Math.Max(screen.Left, Math.Min(x, screen.Right - _form3Instance.Width));
                y = Math.Max(screen.Top, Math.Min(y, screen.Bottom - _form3Instance.Height));

                _form3Instance.StartPosition = FormStartPosition.Manual;
                _form3Instance.Location = new Point(x, y);

                _form3Instance.Show();
            }
            else
            {
                _form3Instance.BringToFront();
                _form3Instance.Focus();
            }
        }
        public SearchBarForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.BackColor = Color.White;
            this.Shown += (s, e) => PositionBottomCenterByScreenPercent(0.05f);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ApplySmoothRoundRegion();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplySmoothRoundRegion();
        }

        private void ApplySmoothRoundRegion()
        {
            int radius = 55; 
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

        public SearchBarForm(Form1 owner)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.BackColor = Color.White;
            this._owner = owner;
            this.Shown += SearchBarForm_Shown;
        }

        private void SearchBarForm_Shown(object sender, EventArgs e)
        {
            PositionBottomCenter(40);
        }
        public void PositionBottomCenter(int pixelOffsetFromBottom = 40)
        {
            Rectangle screenBounds = _owner != null
                ? Screen.FromControl(_owner).WorkingArea
                : Screen.PrimaryScreen.WorkingArea;
            int targetX = screenBounds.Left + (screenBounds.Width - this.Width) / 2;
            int targetY = screenBounds.Bottom - this.Height - pixelOffsetFromBottom;

            this.Location = new Point(targetX, targetY);
        }
        public void PositionBottomCenterByScreenPercent(float percentFromBottom = 0.05f)
        {
            percentFromBottom = Math.Max(0, Math.Min(1, percentFromBottom));

            Rectangle screenBounds = Screen.PrimaryScreen.WorkingArea;
            int offset = (int)(screenBounds.Height * percentFromBottom);

            int targetX = screenBounds.Left + (screenBounds.Width - this.Width) / 2;
            int targetY = screenBounds.Bottom - this.Height - offset;

            this.Location = new Point(targetX, targetY);
        }

        private void SearchBarForm_Load(object sender, EventArgs e)
        {

        }
        private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; 
                string query = textBoxSearch.Text.Trim();
                if (!string.IsNullOrEmpty(query))
                {
                    string url = "https://www.google.com/search?q=" + Uri.EscapeDataString(query);
                    try
                    {
                        Process.Start(url);
                    }
                    catch
                    {
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                    }
                    Application.Exit();
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            ShowForm3();
        }
        private void ShowForm3()
        {
            if (_form3Instance == null || _form3Instance.IsDisposed)
            {
                _form3Instance = new Form3();
                _form3Instance.FormClosed += (s, args) => _form3Instance = null;
                Point mousePos = Cursor.Position;
                _form3Instance.Show();
                int x = mousePos.X - (_form3Instance.Width / 2);
                int y = mousePos.Y - _form3Instance.Height - 10;
                Rectangle screen = Screen.FromPoint(mousePos).WorkingArea;
                x = Math.Max(screen.Left, Math.Min(x, screen.Right - _form3Instance.Width));
                y = Math.Max(screen.Top, Math.Min(y, screen.Bottom - _form3Instance.Height));

                _form3Instance.Location = new Point(x, y);
            }
            else
            {
                _form3Instance.BringToFront();
                _form3Instance.Focus();
            }
        }
    }
}
