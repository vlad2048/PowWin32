using FastForms.Docking;
using FastForms.Docking.Logic.HolderWin_.Structs;
using FastForms.Utils.GdiUtils;
using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.Structs;
using PowWin32.Windows.StructsPackets;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;
	
namespace FastForms;

public sealed class AppWin
{
	private static readonly Gdi32.SafeHBRUSH BkgBrush = MkBrushGdi(0xEEEEF2);

	private static readonly WinClass Class = new(
		"AppWin",
		styles: 0, //User32.WindowClassStyles.CS_HREDRAW | User32.WindowClassStyles.CS_VREDRAW,
		hBrush: Gdi32.GetStockObject(Gdi32.StockObjectType.NULL_BRUSH)
		//hBrush: BkgBrush
	);
	private static readonly WinStylesDef Styles = new(
		User32.WindowStyles.WS_CLIPSIBLINGS | User32.WindowStyles.WS_CLIPCHILDREN |
		User32.WindowStyles.WS_MAXIMIZEBOX | User32.WindowStyles.WS_MINIMIZEBOX | User32.WindowStyles.WS_SIZEBOX | User32.WindowStyles.WS_SYSMENU | User32.WindowStyles.WS_DLGFRAME | User32.WindowStyles.WS_BORDER,
		User32.WindowStylesEx.WS_EX_WINDOWEDGE
	);

	private static readonly Brush BackBrush = MkBrush(0x808080);


	public SysWin Sys { get; } = new();
	public Docker Docker { get; }

	public AppWin(R r, bool withDocRoot = true)
	{
		Class.CreateWindow(Sys, Styles, r, 0, "AppWin");
		var dockerRoot = N.RootTool(withDocRoot ? N.RootDoc() : null);
		Docker = Docker.MakeForMainWindow(dockerRoot, Sys);
		_ = HolderBtnBmps.Bmps;

		Sys.Evt.WhenPaint.Subs((ref PaintPacket e) =>
		{
			using var _ = e.Paint(out var gfx);
			gfx.FillRect(Sys.ClientR, BackBrush);
		});

		Sys.Show();
	}
}