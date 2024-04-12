using PowWin32.Windows.Structs;
using PowWin32.Windows;
using Vanara.PInvoke;
using PowWin32.Geom;
using PowWin32.Windows.ReactiveLight;
using static Vanara.PInvoke.Kernel32;

namespace PlayFast.Demos;

static class TypicalWin32App
{
	public static void Run()
	{
		var sys = new SysWin();
		Class.CreateWindow(sys, Styles, WinR, 0, "MyWin");

		sys.Evt.WhenMessage.Subs((ref WindowMessage e) =>
		{
			L($"[{e.Id}]");
			if (e.Id == User32.WindowMessage.WM_LBUTTONDOWN)
			{
				L("WM_LBUTTONDOWN");
				sys.Destroy();
				//sys.Dispose();
				//User32.DestroyWindow(sys.Handle);
			}
		});

		MsgPump.Run(sys);
	}


	private static readonly R WinR = new(-350, 100, 250, 200);

	private static readonly Gdi32.SafeHBRUSH BkgBrush = MkBrushGdi(0x000000);

	private static readonly WinClass Class = new(
		"MyWin",
		styles: User32.WindowClassStyles.CS_HREDRAW | User32.WindowClassStyles.CS_VREDRAW,
		hBrush: BkgBrush
	);
	private static readonly WinStylesDef Styles = new(
		User32.WindowStyles.WS_VISIBLE |
		User32.WindowStyles.WS_CLIPSIBLINGS | User32.WindowStyles.WS_CLIPCHILDREN |
		User32.WindowStyles.WS_MAXIMIZEBOX | User32.WindowStyles.WS_MINIMIZEBOX | User32.WindowStyles.WS_SIZEBOX | User32.WindowStyles.WS_SYSMENU | User32.WindowStyles.WS_DLGFRAME | User32.WindowStyles.WS_BORDER,
		User32.WindowStylesEx.WS_EX_WINDOWEDGE
	);
}