using PowWin32.Geom;

namespace FastForms.Utils.GdiUtils;


public static class GdiTextExt
{
	private const TextFormatFlags Flags =
		0
		| TextFormatFlags.NoPadding
		| TextFormatFlags.WordEllipsis
		| TextFormatFlags.PreserveGraphicsTranslateTransform
		;

	public static void DrawText(this Graphics gfx, R r, string text, Font font, Color frontColor, Color backColor) =>
		TextRenderer.DrawText(gfx, text, font, r, frontColor, backColor, Flags);

	public static void DrawText(this Graphics gfx, Pt p, string text, Font font, Color frontColor, Color backColor) =>
		TextRenderer.DrawText(gfx, text, font, new Point(p.X, p.Y), frontColor, backColor, Flags);

	public static Sz DrawTextMeasure(this Graphics gfx, Pt p, string text, Font font, Color frontColor, Color backColor)
	{
		gfx.DrawText(p, text, font, frontColor, backColor);
		return font.MeasureText(text);
	}

	public static Sz MeasureText(this Font font, string text) => TextRenderer.MeasureText(text, font, Sz.MaxValue, Flags).ToSz();
}