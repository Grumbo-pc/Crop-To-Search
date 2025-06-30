using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Crop_To_Search
{
    public class CustomColorProgressBar : ProgressBar
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(typeof(Color), "#0078d4")]
        public Color BarColor { get; set; } = ColorTranslator.FromHtml("#0078d4");

        public CustomColorProgressBar()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.BackColor = ThemeHelper.IsWindowsInDarkMode() ? ColorTranslator.FromHtml("#282a2c") : Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = this.ClientRectangle;
            Graphics g = e.Graphics;

            Color currentMarginColor = ThemeHelper.IsWindowsInDarkMode() ? ColorTranslator.FromHtml("#282a2c") : Color.White;
            this.BackColor = currentMarginColor;

            using (SolidBrush marginBrush = new SolidBrush(currentMarginColor))
            {
                g.FillRectangle(marginBrush, rect);
            }

            rect.Inflate(-2, -2);

            using (SolidBrush backgroundBrush = new SolidBrush(currentMarginColor))
            {
                g.FillRectangle(backgroundBrush, rect);
            }

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
