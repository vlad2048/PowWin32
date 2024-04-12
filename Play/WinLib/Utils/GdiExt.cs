using PowWin32.Geom;
using System.Drawing;

namespace WinLib.Utils;

public static class GdiExt
{
	public static void FillDraw(this Graphics gfx, R r, Brush brush, Pen pen)
	{
		gfx.FillRectangle(brush, r.X, r.Y, r.Width - 1, r.Height - 1);
		gfx.DrawRectangle(pen, r.X, r.Y, r.Width - 1, r.Height - 1);
	}
}