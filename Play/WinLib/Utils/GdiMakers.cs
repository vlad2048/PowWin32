using System.Drawing;

namespace WinLib.Utils;

public static class GdiMakers
{
	public static Brush MkBrush(uint v) => new SolidBrush(MkColor(v, 255));
	public static Pen MkPen(uint v) => new(MkColor(v, 255));
	public static Color MkColor(uint v) => Color.FromArgb(unchecked((int)v));
	public static Color MkColor(uint v, int alpha) => Color.FromArgb(alpha, MkColor(v & 0xFFFFFF));
}