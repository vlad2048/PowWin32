using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace WinTest.Utils;

static class WinUtils
{
	// Scheduling
	// ==========
	// ReSharper disable once CollectionNeverQueried.Local
	private static readonly List<Timerproc> procs = new();
	public static void Schedule(HWND hwnd, int id, TimeSpan period, bool recurring, Action action)
	{
		Timerproc proc = null!;
		proc = (wnd, _, @event, _) =>
		{
			if (!recurring)
			{
				KillTimer(wnd, @event);
				procs.Remove(proc);
			}
			action();
		};
		procs.Add(proc);
		SetTimer(hwnd, id, (uint)period.TotalMilliseconds, proc);
	}



	public static Pt GetCursorPos()
	{
		User32.GetCursorPos(out var pt);
		return pt;
	}

	public static ShowWindowCommand GetShowCmd(this SysWin win)
	{
		var plc = new WINDOWPLACEMENT();
		GetWindowPlacement(win.Handle, ref plc);
		return plc.showCmd;
	}

	public static R GetWinR(this SysWin win)
	{
		GetWindowRect(win.Handle, out var r);
		return r;
	}

	public static R GetClientR(this SysWin win)
	{
		GetClientRect(win.Handle, out var r);
		return r;
	}

	public static R GetClientR2Screen(this SysWin win)
	{
		GetClientRect(win.Handle, out var r);
		return win.Client2Screen(r);
	}

	public static R GetClientR2Screen(this HWND win)
	{
		GetClientRect(win, out var r);
		return win.Client2Screen(r);
	}


	public static Pt Screen2Client(this SysWin win, Pt pt)
	{
		POINT p = pt;
		ScreenToClient(win.Handle, ref p);
		return p;
	}

	public static Pt Client2Screen(this SysWin win, Pt pt)
	{
		POINT p = pt;
		ClientToScreen(win.Handle, ref p);
		return p;
	}
	public static Pt Client2Screen(this HWND win, Pt pt)
	{
		POINT p = pt;
		ClientToScreen(win, ref p);
		return p;
	}
	public static R Client2Screen(this SysWin win, R r)
	{
		var tl = win.Client2Screen(r.Pos);
		var br = win.Client2Screen(r.BottomRight);
		return new R(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y);
	}
	public static R Client2Screen(this HWND win, R r)
	{
		var tl = win.Client2Screen(r.Pos);
		var br = win.Client2Screen(r.BottomRight);
		return new R(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y);
	}
	public static R Screen2Client(this SysWin win, R r)
	{
		var tl = win.Screen2Client(r.Pos);
		var br = win.Screen2Client(r.BottomRight);
		return new R(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y);
	}


	public static WindowStyles GetStyles(this SysWin win) => (WindowStyles)win.GetParam(WindowLongFlags.GWL_STYLE).ToSafeInt32();
	public static WindowStylesEx GetStylesEx(this SysWin win) => (WindowStylesEx)win.GetParam(WindowLongFlags.GWL_EXSTYLE).ToSafeInt32();
	public static void SetStyles(this SysWin win, WindowStyles styles) => win.SetParam(WindowLongFlags.GWL_STYLE, (int)styles);
	public static void SetStylesEx(this SysWin win, WindowStylesEx styles) => win.SetParam(WindowLongFlags.GWL_EXSTYLE, (int)styles);
	public static void MutateStyles(this SysWin win, Func<WindowStyles, WindowStyles> fun) => win.SetStyles(fun(win.GetStyles()));
	public static void MutateExStyles(this SysWin win, Func<WindowStylesEx, WindowStylesEx> fun) => win.SetStylesEx(fun(win.GetStylesEx()));


	private static nint GetParam(this SysWin win, WindowLongFlags index) => GetWindowLongAuto(win.Handle, index);
	private static void SetParam(this SysWin win, WindowLongFlags index, nint v) => SetWindowLong(win.Handle, index, v);

}