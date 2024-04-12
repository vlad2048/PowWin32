using FastForms.Docking;
using PowWin32.Geom;
using PowWin32.Windows.Structs;
using PowWin32.Windows;
using PowWin32.Windows.StructsPackets;
using System.Drawing;
using FastForms;
using FastForms.Utils;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;
using static Vanara.PInvoke.Shell32;

namespace PlayFast;

static class ExplorerRunner
{
	public static void Run()
	{
		var thread = new Thread(RunInner);
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();
		thread.Join();
	}

	private static void RunInner()
	{
		//var hr = Ole32.CoInitializeEx(0, Ole32.COINIT.COINIT_APARTMENTTHREADED);
		var hr = Ole32.CoInitializeEx(0, Ole32.COINIT.COINIT_MULTITHREADED);
		Console.WriteLine($"HR = {hr}");
		var win = new SimpleWin(new R(50, 30, 400, 600));
		MsgPump.Run(win.Sys);
	}
}


sealed class SimpleWin
{
	private static readonly WinClass Class = new(
		"SimpleWin",
		styles: User32.WindowClassStyles.CS_HREDRAW | User32.WindowClassStyles.CS_VREDRAW,
		hBrush: Gdi32.GetStockObject(Gdi32.StockObjectType.NULL_BRUSH)
	);
	private static readonly WinStylesDef Styles = new(
		User32.WindowStyles.WS_VISIBLE | User32.WindowStyles.WS_CLIPSIBLINGS |
		User32.WindowStyles.WS_MAXIMIZEBOX | User32.WindowStyles.WS_MINIMIZEBOX | User32.WindowStyles.WS_SIZEBOX | User32.WindowStyles.WS_SYSMENU | User32.WindowStyles.WS_DLGFRAME | User32.WindowStyles.WS_BORDER,
		User32.WindowStylesEx.WS_EX_WINDOWEDGE
	);

	private static readonly Brush DrawBrush = MkBrush(0x8CBDFF);
	private static readonly Pen DrawPen = MkPen(0xFF2865);


	public SysWin Sys { get; } = new();

	public SimpleWin(R r)
	{
		Sys.Evt.WhenCreate.Subs((ref CreatePacket e) =>
		{
			var browser = new IExplorerBrowser();
			browser.SetOptions(EXPLORER_BROWSER_OPTIONS.EBO_SHOWFRAMES);

			var hwnd = Sys.Handle.DangerousGetHandle();
			var rect = Sys.Handle.GetClientR().ToRect();
			var opts = new PFOLDERSETTINGS(
				FOLDERVIEWMODE.FVM_DETAILS,
				FOLDERFLAGS.FWF_NONE
			);
			browser.Initialize(hwnd, rect, opts);

			SHParseDisplayName(@"C:\Utils", 0, out var pidl, 0, out _);
			browser.BrowseToIDList(pidl, 0);

			Sys.Evt.WhenSize.Subs((ref SizePacket e) =>
			{
				var rr = Sys.Handle.GetClientR().ToRect();
				browser.SetRect(0, rr);
			});

			Sys.Evt.WhenDestroy.Subs((ref Packet e) =>
			{
				browser.Destroy();
			});
		});

		Class.CreateWindow(Sys, Styles, r, 0, "SimpleWin");
	}
}


static class ExplorerRunnerExt
{
	public static RECT ToRect(this R r) => new(r.X, r.Y, r.X + r.Width, r.Y + r.Height);
}
