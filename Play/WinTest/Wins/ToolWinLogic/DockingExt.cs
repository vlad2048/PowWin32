using PowWin32.Geom;
using PowWin32.Windows.Utils;
using static Vanara.PInvoke.User32;
using Vanara.PInvoke;
using WinTest.Utils;

namespace WinTest.Wins.ToolWinLogic;

static class DockingExt
{
	public static void Undock(this ToolWin win)
	{
		if (!win.IsDocked) return;
		var handle = win.Sys.Handle;

		var r = win.Sys.GetWinR() + new Marg(0, 8, 8, 8);

		win.Sys.SetStyles(Styles.ToolWin_StylesUndocked.Styles & ~WindowStyles.WS_VISIBLE);
		SetParent(handle, 0);
		SetWindowPos(handle, 0, r.X, r.Y, r.Width, r.Height, SetWindowPosFlags.SWP_FRAMECHANGED | SetWindowPosFlags.SWP_NOOWNERZORDER);

		Console.WriteLine("[Undock] before set Visible");

		win.Sys.MutateStyles(e => e | WindowStyles.WS_VISIBLE);
		RedrawWindow(win.Sys.Handle, default, HRGN.NULL, RedrawWindowFlags.RDW_INVALIDATE | RedrawWindowFlags.RDW_UPDATENOW);
	}

	public static void Dock(this ToolWin win)
	{
		if (win.IsDocked) return;
		var handle = win.Sys.Handle;

		var kidR = win.Sys.GetWinR() - new Marg(0, 8, 8, 8);
		var dadR = win.Owner.GetClientR2Screen();
		var r = kidR - dadR.Pos;

		win.Sys.SetStyles(Styles.ToolWin_StylesDocked.Styles);
		SetParent(handle, win.Owner);
		SetWindowPos(handle, 0, r.X, r.Y, r.Width, r.Height, SetWindowPosFlags.SWP_SHOWWINDOW | SetWindowPosFlags.SWP_FRAMECHANGED);
		win.IsDocked = true;
	}



	/*
	//public static void Undock(this ToolWin win, Pt grabOfs, Pt dif)
	public static void Undock(this ToolWin win)
	{
		if (!win.IsDocked) return;
		var handle = win.Sys.Handle;



		//L("Visible <- false");
		win.Sys.MutateStyles(e => e & ~WindowStyles.WS_VISIBLE);
		RedrawWindow(win.Owner, default, HRGN.NULL, RedrawWindowFlags.RDW_INVALIDATE | RedrawWindowFlags.RDW_UPDATENOW);



		//L("Undock");
		var kidR = win.Sys.GetWinR();
		var r = kidR + new Marg(0, 8, 8, 8);
		win.Sys.SetStyles(WindowStyles.WS_POPUP | WindowStyles.WS_SYSMENU | WindowStyles.WS_SIZEBOX | WindowStyles.WS_DLGFRAME | WindowStyles.WS_BORDER);
		//win.Sys.SetStyles(Styles.ToolWin_StylesUndocked.Styles & ~WindowStyles.WS_VISIBLE);
		//win.Sys.SetStylesEx(Styles.ToolWin_StylesUndocked.StylesEx);
		SetWindowPos(handle, 0, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOSENDCHANGING | SetWindowPosFlags.SWP_HIDEWINDOW | SetWindowPosFlags.SWP_NOREDRAW);
		SetParent(handle, 0);

		//r += dif;
		SetWindowPos(handle, 0, r.X, r.Y, r.Width, r.Height, SetWindowPosFlags.SWP_FRAMECHANGED | SetWindowPosFlags.SWP_NOOWNERZORDER);
		var curPos = WinUtils.GetCursorPos();
		PostMessage(handle, (uint)WM.WM_SYSCOMMAND, 0xF010 | 0x0002, curPos.FromPt());

		//TweakPos = true;
		//MonGrabOfs = grabOfs;

		win.IsDocked = false;

		//Thread.Sleep(1000);



		//L("Visible <- true");
		//win.Sys.SaveScreenshots();
		//SetWindowRgn(win.Sys.Handle, HRGN.NULL, false);

		win.Sys.MutateStyles(e => e | WindowStyles.WS_VISIBLE);
		RedrawWindow(win.Sys.Handle, default, HRGN.NULL, RedrawWindowFlags.RDW_INVALIDATE | RedrawWindowFlags.RDW_UPDATENOW);
	}

	private static void L(string s) => Console.WriteLine(s);

	public static void Dock(this ToolWin win)
	{
		if (win.IsDocked) return;
		var handle = win.Sys.Handle;

		var kidR = win.Sys.GetWinR() - new Marg(0, 8, 8, 8);
		var dadR = win.Owner.GetClientR2Screen();
		var r = kidR - dadR.Pos;

		win.Sys.SetStyles(Styles.ToolWin_StylesDocked.Styles);
		//win.Sys.SetStylesEx(Styles.ToolWin_StylesDocked.StylesEx);
		//WinUtils.SetStyles(Handle, WindowStyles.WS_CHILD | WindowStyles.WS_VISIBLE);
		//User32Methods.ShowWindow(Handle, ShowWindowCommands.SW_HIDE);

		SetParent(handle, win.Owner);
		SetForegroundWindow(win.Owner);
		SetRedraw(win.Owner, false);
		SetR(handle, r);
		UpdateWindow(win.Owner);
		SetRedraw(win.Owner, true);
		win.IsDocked = true;
		RedrawWindow(win.Owner, default, 0, RedrawWindowFlags.RDW_INVALIDATE | RedrawWindowFlags.RDW_UPDATENOW | RedrawWindowFlags.RDW_ERASE | RedrawWindowFlags.RDW_ALLCHILDREN);
		//User32Methods.RedrawWindow(Handle, RedrawWindowFlags.RDW_INVALIDATE | RedrawWindowFlags.RDW_UPDATENOW | RedrawWindowFlags.RDW_ERASE | RedrawWindowFlags.RDW_ALLCHILDREN);
		InvalidateRect(handle, default, false);
	}


	private static void SetR(HWND hwnd, R r) => SetWindowPos(hwnd, 0, r.X, r.Y, r.Width, r.Height, SetWindowPosFlags.SWP_SHOWWINDOW | SetWindowPosFlags.SWP_FRAMECHANGED);
	public static void SetRedraw(HWND hwnd, bool redraw) => SendMessage(hwnd, (uint)WM.WM_SETREDRAW, new nint(redraw ? 1 : 0), 0);
	*/
}