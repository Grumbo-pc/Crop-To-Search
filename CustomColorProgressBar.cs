using System;
using System.Drawing;
using System.Windows.Forms;

namespace Crop_To_Search
{
    public class CustomColorProgressBar : ProgressBar
    {
        public Color BarColor { get; set; } = ColorTranslator.FromHtml("#0078d4");

        public CustomColorProgressBar()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = this.ClientRectangle;
            Graphics g = e.Graphics;

            ProgressBarRenderer.DrawHorizontalBar(g, rect);
            rect.Inflate(-2, -2);

            if (this.Value > 0)
            {
                int width = (int)((float)this.Value / this.Maximum * rect.Width);
                using (SolidBrush brush = new SolidBrush(BarColor))
                {
                    g.FillRectangle(brush, rect.X, rect.Y, width, rect.Height);
                }
            }
        }
    }
}
