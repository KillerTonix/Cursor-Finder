using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Cursor_finder
{
    public partial class RippleForm : Form
    {
        private readonly Timer animationTimer = new Timer();
        private int radius = 10;

        public RippleForm()
        {
            this.DoubleBuffered = true;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            
            this.BackColor = Color.Black; // Background color
            this.TransparencyKey = Color.Black; // Make black color transparent
            this.Opacity = 0.7; // Set overall opacity
            this.Text = "Cursor Finder v1.0";

            animationTimer.Interval = 15;
            animationTimer.Tick += (s, e) =>
            {
                radius += 8;
                if (radius > 120) animationTimer.Stop();
                this.Invalidate();
            };

            this.Load += (s, e) => // Make the form click-through
            {
                int initialStyle = GetWindowLong(this.Handle, -20); // GWL_EXSTYLE = -20
                SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20); // WS_EX_LAYERED = 0x80000, WS_EX_TRANSPARENT = 0x20
            };
        }

        // Show the ripple effect at the specified location
        public void ShowAt(Point location) 
        {
            radius = 10;
            this.Location = new Point(location.X - 100, location.Y - 100); // Center the form around the location
            this.Show(); 
            animationTimer.Start(); // Start the animation
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (Pen p = new Pen(Color.Gray, 50))
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.DrawEllipse(p, 100 - radius / 2, 100 - radius / 2, radius, radius);
            }
        }

        #region Click-Through P/Invokes
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        #endregion
    }
}
