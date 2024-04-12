using FastForms.Docking.Logic.DropLogic_.Structs;
using FastForms.Utils.GdiUtils;
using PowWin32.Geom;
using Style = FastForms.Docking.Logic.DropLogic_.Painting.DropPainterStyle;

namespace FastForms.Docking.Logic.DropLogic_.Painting;


static class GeomPainter
{
	private const int TabWidth = Style.GeomTabWidth;
	private const int Mg = Style.GeomMarg;

	public static void PaintGeom(this Graphics gfx, Geom geom)
	{
		switch (geom.Tab)
		{
			case null:
				gfx.PaintGeomPlain(geom.R);
				break;

			case not null:
				gfx.PaintGeomTab(geom.R, geom.Tab);
				break;
		}
	}

	private static void PaintGeomPlain(this Graphics gfx, R r) => gfx.FillMargRect(r, null);

	private static void PaintGeomTab(this Graphics gfx, R r, TabGeom tab)
	{
		if (r.IsDegenerate()) return;
		var tabR = GetTabR(r, tab);
		if (tabR.IsDegenerate())
		{
			gfx.FillMargRect(r, null);
			return;
		}

		var clip = gfx.Clip;

		gfx.ExcludeClip(tabR);
		gfx.FillMargRect(r, null);

		gfx.Clip = clip;

		gfx.FillMargRect(tabR, tab.Dir);
	}


	private static R GetTabR(R r, TabGeom tab) => tab.Dir switch
	{
		SDir.Up => new R(r.X + tab.Pos, r.Y - TabWidth, tab.Lng, r.Y + Mg),
		SDir.Down => new R(r.X + tab.Pos, r.Bottom - Mg, tab.Lng, Mg + TabWidth),
		_ => throw new ArgumentException("Other directions not supported"),
	};


	private static void FillMargRect(this Graphics gfx, R r, SDir? dir)
	{
		if (r.IsDegenerate()) return;
		var marg = dir switch
		{
			null => Marg.All(Mg),
			SDir.Up => new(Mg, Mg, 0, Mg),
			SDir.Right => new(Mg, Mg, Mg, 0),
			SDir.Down => new(0, Mg, Mg, Mg),
			SDir.Left => new(Mg, 0, Mg, Mg),
			_ => throw new ArgumentException(),
		};
		var innerR = r - marg;
		if (innerR.IsDegenerate())
		{
			gfx.FillRect(r, Style.GeomOuter);
			return;
		}
		var clip = gfx.Clip;
		gfx.FillRect(innerR, Style.GeomInner);
		gfx.ExcludeClip(innerR);
		gfx.FillRect(r, Style.GeomOuter);
		gfx.Clip = clip;
	}
}