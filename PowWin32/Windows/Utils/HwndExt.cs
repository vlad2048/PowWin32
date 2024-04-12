/*
using PowWin32.Diag;
using PowWin32.Geom;
using System.Runtime.InteropServices;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace PowWin32.Windows.Utils;

public static class HwndExt
{
	// ***********
	// * Drawing *
	// ***********
	public static void Invalidate(this HWND hwnd) => InvalidateRect(hwnd, 0, false).Check();
	public static void Invalidate(this HWND hwnd, R r)
	{
		RECT rect = r;
		User32.InvalidateRect(hwnd, rect, false).Check();
	}

	[DllImport(Lib.User32, SetLastError = false, ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool InvalidateRect(HWND hWnd, [In] nint lpRect, [MarshalAs(UnmanagedType.Bool)] bool bErase);


	// **************
	// * Rectangles *
	// **************
	public static void SetWindowPos(this HWND hwnd, R r, SetWindowPosFlags flags) => User32.SetWindowPos(hwnd, 0, r.X, r.Y, r.Width, r.Height, flags);
	public static void SetWindowPos_MoveSize(this HWND hwnd, R r) => hwnd.SetWindowPos(r,
		SetWindowPosFlags.SWP_NOACTIVATE |
		SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOZORDER
	);
	public static void SetWindowPos_MoveSizeShow(this HWND hwnd, R r) => hwnd.SetWindowPos(r,
		SetWindowPosFlags.SWP_NOACTIVATE |
		SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOZORDER |
		SetWindowPosFlags.SWP_SHOWWINDOW
	);

	public static R GetWinR(this HWND hwnd)
	{
		GetWindowRect(hwnd, out var r).Check();
		return r;
	}

	public static R GetClientR(this HWND hwnd)
	{
		GetClientRect(hwnd, out var r).Check();
		return r;
	}

	public static R GetClientRScreen(this HWND hwnd) => Client2Screen(hwnd, GetClientR(hwnd));

	public static R GetDwmR(this HWND hwnd)
	{
		DwmApi.DwmGetWindowAttribute<RECT>(hwnd, DwmApi.DWMWINDOWATTRIBUTE.DWMWA_EXTENDED_FRAME_BOUNDS, out var r).Check();
		return r;
	}

	public static Pt Screen2Client(this HWND hwnd, Pt e)
	{
		POINT s = e;
		ScreenToClient(hwnd, ref s).Check();
		return s;
	}

	public static Pt Client2Screen(this HWND hwnd, Pt e)
	{
		POINT s = e;
		ClientToScreen(hwnd, ref s).Check();
		return s;
	}

	public static R Screen2Client(this HWND hwnd, R e)
	{
		var tl = Screen2Client(hwnd, e.Pos);
		var br = Screen2Client(hwnd, e.BottomRight);
		return new R(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y);
	}

	public static R Client2Screen(this HWND hwnd, R e)
	{
		var tl = Client2Screen(hwnd, e.Pos);
		var br = Client2Screen(hwnd, e.BottomRight);
		return new R(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y);
	}


	// *********
	// * State *
	// *********
	public static bool IsWindow(this HWND hwnd) => User32.IsWindow(hwnd);

	public static void Show(this HWND hwnd) => ShowWindow(hwnd, ShowWindowCommand.SW_SHOWNORMAL);

	public static ShowWindowCommand GetShowCmd(this HWND hwnd)
	{
		var plc = new WINDOWPLACEMENT();
		GetWindowPlacement(hwnd, ref plc);
		return plc.showCmd;
	}

	public static bool IsMaximized(this HWND hwnd) => hwnd.GetShowCmd() == ShowWindowCommand.SW_MAXIMIZE;

	public static bool IsActive(this HWND hwnd) => GetActiveWindow() == hwnd;

	//public static HWND GetParentWindow(this HWND hwnd) => GetAncestor(hwnd, GetAncestorFlag.GA_PARENT);
	//public static HWND GetRootWindow(this HWND hwnd) => GetAncestor(hwnd, GetAncestorFlag.GA_ROOT);



	// **********
	// * Params *
	// **********
	public static nint GetParam(this HWND hwnd, WindowLongFlags flag)
	{
		Kernel32.SetLastError(0);
		var res = GetWindowLongPtr(hwnd, flag);
		ErrorExt.CheckLastErrorIf(res == 0);
		return res;
	}

	public static nint SetParam(this HWND hwnd, WindowLongFlags flag, nint v)
	{
		Kernel32.SetLastError(0);
		var res = SetWindowLong(hwnd, flag, v);
		ErrorExt.CheckLastErrorIf(res == 0);
		return res;
	}


	// **********
	// * Styles *
	// **********
	public static WindowStyles GetStyles(this HWND hwnd) => (WindowStyles)hwnd.GetParam(WindowLongFlags.GWL_STYLE);
	public static void SetStyles(this HWND hwnd, WindowStyles v) => hwnd.SetParam(WindowLongFlags.GWL_STYLE, (int)v);

	public static bool HasStyle(this HWND hwnd, WindowStyles v) => (hwnd.GetStyles() & v) == v;

	public static void AddStyle(this HWND hwnd, WindowStyles v)
	{
		var styles = hwnd.GetStyles();
		hwnd.SetStyles(styles | v);
	}

	public static void DelStyle(this HWND hwnd, WindowStyles v)
	{
		var styles = hwnd.GetStyles();
		hwnd.SetStyles(styles & ~v);
	}

	public static WindowStylesEx GetStylesEx(this HWND hwnd) => (WindowStylesEx)hwnd.GetParam(WindowLongFlags.GWL_EXSTYLE);
	public static void SetStylesEx(this HWND hwnd, WindowStylesEx v) => hwnd.SetParam(WindowLongFlags.GWL_EXSTYLE, (int)v);

	public static bool HasStyleEx(this HWND hwnd, WindowStylesEx v) => (hwnd.GetStylesEx() & v) == v;

	public static void AddStyleEx(this HWND hwnd, WindowStylesEx v)
	{
		var styles = hwnd.GetStylesEx();
		hwnd.SetStylesEx(styles | v);
	}

	public static void DelStyleEx(this HWND hwnd, WindowStylesEx v)
	{
		var styles = hwnd.GetStylesEx();
		hwnd.SetStylesEx(styles & ~v);
	}


	// ************
	// * Schedule *
	// ************
	// ReSharper disable once CollectionNeverQueried.Local
	private static readonly List<Timerproc> procs = new();
	public static void Schedule(this HWND hwnd, int id, TimeSpan period, bool recurring, Action action)
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
}
*/