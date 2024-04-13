using PowWin32.Geom;
using static Vanara.PInvoke.Gdi32;
using Style = FastForms.Docking.Logic.DropLogic_.Painting.DropPainterStyle;

namespace FastForms.Docking.Logic.DropLogic_.Structs;


sealed record TabGeom(SDir Dir, int Pos, int Lng);

sealed record Geom(R R, TabGeom? Tab)
{
	private const int TabWidth = Style.GeomTabWidth;
	private const int Mg = Style.GeomMarg;

	public static Geom ForNode(R r) => new(r, null);
	public static Geom ForNodeSide(R r, SDir dir) => new(r.GetSubR(dir), null);

	public static Geom ForTab(R r, TabGeom tab)
	{
		var mainR = tab.Dir switch
		{
			SDir.Up => r - Marg.MkUp(TabWidth),
			SDir.Down => r - Marg.MkDown(TabWidth),
			_ => throw new ArgumentException("Other directions not supported"),
		};
		return new Geom(mainR, tab);
	}

	/*public static Geom ForTab(R r, SDir dir, int pos, int lng)
	{
		var mainR = dir switch
		{
			SDir.Up => r - Marg.MkUp(TabWidth),
			SDir.Down => r - Marg.MkDown(TabWidth),
			_ => throw new ArgumentException("Other directions not supported"),
		};
		return new Geom(mainR, new TabGeom(dir, pos, lng));
	}*/
}




file static class GeomFileExt
{
	public static R GetSubR(this R r, SDir dir) =>
		dir switch
		{
			SDir.Up => r - Marg.MkDown(r.Height / 2),
			SDir.Down => r - Marg.MkUp(r.Height / 2),
			SDir.Left => r - Marg.MkRight(r.Width / 2),
			SDir.Right => r - Marg.MkLeft(r.Width / 2),
			_ => throw new ArgumentException()
		};
}
