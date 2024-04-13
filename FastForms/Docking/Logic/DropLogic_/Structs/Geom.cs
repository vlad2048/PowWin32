using PowWin32.Geom;

namespace FastForms.Docking.Logic.DropLogic_.Structs;


sealed record TabGeom(SDir Dir, int Pos, int Lng);

sealed record Geom(R R, TabGeom? Tab)
{
	public static Geom ForNode(R r) => new(r, null);
	public static Geom ForNodeSide(R r, SDir dir) => new(r.GetSubR(dir), null);
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
