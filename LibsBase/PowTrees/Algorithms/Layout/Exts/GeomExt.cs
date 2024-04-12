using PowWin32.Geom;

namespace PowTrees.Algorithms.Layout.Exts;

static class GeomExt
{
	public static Sz MakeBigger(this Sz sz, Sz delta)
		=> new(sz.Width + delta.Width, sz.Height + delta.Height);

	public static Sz MakeSmaller(this Sz sz, Sz delta)
		=> new(sz.Width - delta.Width, sz.Height - delta.Height);
}