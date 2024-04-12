using FastForms.Structs;
using PowWin32.Geom;

namespace FastForms.Utils.GdiUtils;


/*

Graphics.FillRectangle 0,0 5x3
	xxxxx
	xxxxx
	xxxxx

Graphics.DrawRectangle 0,0 5x3
	xxxxxx
	xxxxxx
	xxxxxx
	xxxxxx


*/

public static class GdiRectExt
{
	public static void FillRect(this Graphics gfx, R r, Brush brush)
	{
		if (r == R.Empty) return;
		gfx.FillRectangle(brush, r.ToR());
	}
	public static void DrawRect(this Graphics gfx, R r, Pen pen, Side? sides = null)
	{
		if (r == R.Empty) return;
		sides ??= Side.All;
		if (sides == Side.All)
		{
			gfx.DrawRectangle(pen, r.Dec().ToR());
		}
		else
		{
			var s = sides.Value;
			var ((x1, y1), (x2, y2)) = (r.Pos, r.Dec().BottomRight);
			if (s.HasFlag(Side.Up)) gfx.DrawLine(pen, x1, y1, x2, y1);
			if (s.HasFlag(Side.Down)) gfx.DrawLine(pen, x1, y2, x2, y2);
			if (s.HasFlag(Side.Left)) gfx.DrawLine(pen, x1, y1, x1, y2);
			if (s.HasFlag(Side.Right)) gfx.DrawLine(pen, x2, y1, x2, y2);
		}
	}
	public static void FillRectPattern(this Graphics gfx, R r, Brush brush)
	{
		if (r == R.Empty) return;
		gfx.TranslateTransform(r.X, r.Y);
		gfx.FillRectangle(brush, r.WithZeroPos().ToR());
		gfx.TranslateTransform(-r.X, -r.Y);
	}

	public static void FillDraw(this Graphics gfx, R r, Brush brush, Pen pen)
	{
		gfx.FillRectangle(brush, r.X, r.Y, r.Width - 1, r.Height - 1);
		gfx.DrawRectangle(pen, r.X, r.Y, r.Width - 1, r.Height - 1);
	}

	private static R Dec(this R r) => (r is { Width: > 0, Height: > 0 }) switch
	{
		true => r with { Width = r.Width - 1, Height = r.Height - 1 },
		false => R.Empty
	};
}