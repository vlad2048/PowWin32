using System.Drawing;
using PowWin32.Geom;

namespace WinTest.Utils;

static class Col
{
	public static void FillDraw(this Graphics gfx, R r, Brush brush, Pen pen)
	{
		gfx.FillRectangle(brush, r.X, r.Y, r.Width - 1, r.Height - 1);
		gfx.DrawRectangle(pen, r.X, r.Y, r.Width - 1, r.Height - 1);
	}
	public static Color Make(uint v) => Color.FromArgb(unchecked((int)v));
}