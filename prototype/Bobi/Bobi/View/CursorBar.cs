using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Bobi.View
{
    public partial class CursorBar : Control
    {
        private int baseHeight;

        public CursorBar()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        public int BaseHeight { set { this.baseHeight = value; } }
        public double Zoom { set { Height = (int)Math.Round(this.baseHeight * value); Invalidate();  } }

        protected override void OnPaint(PaintEventArgs pe)
        {
            AudioBlock block = Parent as AudioBlock;
            if (block != null)
            {
                AudioSelection selection = block.Selection as AudioSelection;
                if (selection != null)
                {
                    int from = block.XForTime(selection.From < selection.To ? selection.From : selection.To);
                    int to = block.XForTime(selection.To > selection.From ? selection.To : selection.From);
                    if (selection.IsRange)
                    {
                        pe.Graphics.FillRectangle(block.Colors.AudioSelectionBrush, new Rectangle(from, 0, to - from + 1, Height));
                    }
                    else
                    {
                        pe.Graphics.FillRectangle(block.Colors.AudioSelectionBrush,
                            new Rectangle(block.XForTime(selection.At) - Height / 2, 0, Height, Height));
                    }
                }
                if (block.Playing)
                {
                    int at = block.XForTime(block.PlayingTime);
                    Point[] points = new Point[3];
                    points[0] = new Point(at, 0);
                    points[1] = new Point(at + Height / 2, Height / 2);
                    points[2] = new Point(at, Height);
                    pe.Graphics.DrawLine(block.Colors.AudioPlaybackPen, new Point(at, 0), new Point(at, Height));
                    pe.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    pe.Graphics.FillPolygon(block.Colors.AudioPlaybackBrush, points);
                }
            }
            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

        private void CursorBar_DoubleClick(object sender, EventArgs e)
        {
            if (Parent is AudioBlock) ((AudioBlock)Parent).SelectFromBelow();
        }

        private void CursorBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Parent is AudioBlock) ((AudioBlock)Parent).SelectFromXFromBelow(e.X);
        }

        private void CursorBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Parent is AudioBlock) ((AudioBlock)Parent).SelectToXFromBelow(e.X);
        }

        private void CursorBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) if (Parent is AudioBlock) ((AudioBlock)Parent).SelectToXFromBelow(e.X);
        }
    }
}
