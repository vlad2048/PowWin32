using FastForms.Docking.Logic.DropLogic_.Painting;
using FastForms.Docking.Logic.DropLogic_.Structs;
using FastForms.Utils.GdiUtils;
using PowMaybe;
using PowWin32.Windows;
using PowWin32.Windows.Structs;
using PowWin32.Geom;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;
using FastForms.Utils.Win32;
using Style = FastForms.Docking.Logic.DropLogic_.Painting.DropPainterStyle;

namespace FastForms.Docking.Logic.DropLogic_.Wins;

sealed class DropWin : IDisposable
{
	private static readonly WinClass Class = new(
		"DropWin",
		styles: User32.WindowClassStyles.CS_HREDRAW | User32.WindowClassStyles.CS_VREDRAW,
		hBrush: Gdi32.GetStockObject(Gdi32.StockObjectType.NULL_BRUSH)
	);
	private static readonly WinStylesDef Styles = new(
		User32.WindowStyles.WS_VISIBLE | User32.WindowStyles.WS_POPUP |
		User32.WindowStyles.WS_CLIPSIBLINGS | User32.WindowStyles.WS_CLIPCHILDREN,
		User32.WindowStylesEx.WS_EX_LAYERED
	);

	public void Dispose() => sys.Destroy();

	private readonly SysWin sys = new();

	public DropWin()
	{
		Class.CreateWindow(sys, Styles, R.Empty, 0, "DropWin");
	}

	public void Set(Maybe<Drop> mayDrop)
	{
		if (mayDrop.IsNone(out var drop))
		{
			sys.Hide();
			return;
		}

		var bbox = drop.Geom.BBox();

		LayeredWindowUtils.PaintDrop(
			sys.Handle,
			bbox,
			gfx =>
			{
				//gfx.Clear(Color.Transparent);
				var geom = drop.Geom.Sub(bbox.Pos);
				gfx.PaintGeom(geom);
			}
		);
		sys.Show();
	}
}



file static class GeomExt
{
	private const int TabWidth = Style.GeomTabWidth;

	public static R BBox(this Geom geom) => geom.Tab switch
	{
		null => geom.R,
		not null => geom.Tab.Dir switch
		{
			SDir.Up => geom.R + Marg.MkUp(TabWidth),
			SDir.Right => geom.R + Marg.MkRight(TabWidth),
			SDir.Down => geom.R + Marg.MkDown(TabWidth),
			SDir.Left => geom.R + Marg.MkLeft(TabWidth),
			_ => throw new ArgumentException()
		}
	};

	public static Geom Sub(this Geom geom, Pt ofs) => geom with { R = geom.R - ofs };
}