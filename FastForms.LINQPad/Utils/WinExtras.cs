using PowWin32.Diag;
using PowWin32.Geom;
using static Vanara.PInvoke.User32;
using System.Text;
using Vanara.PInvoke;

namespace FastForms.LINQPad.Utils;

static class WinExtras
{
	public static bool GetIsActive(this HWND hwnd) => User32.GetActiveWindow() == hwnd;
	public static bool GetIsEnabled(this HWND hwnd) => User32.IsWindowEnabled(hwnd);
	public static ShowWindowCommand GetShowCmd(this HWND hwnd)
	{
		var plc = new WINDOWPLACEMENT();
		GetWindowPlacement(hwnd, ref plc);
		return plc.showCmd;
	}

	public static string GetClassName(this HWND hWnd)
	{
		var sb = new StringBuilder(257);
		User32.GetClassName(hWnd, sb, sb.Capacity); //.Check();
		return sb.ToString();
	}


	/*public static R? GetWinROpt(this HWND hwnd) => GetWindowRect(hwnd, out var r) switch
	{
		false => null,
		true => r
	};
	public static R? GetClientROpt(this HWND hwnd) => GetClientRect(hwnd, out var r) switch
	{
		false => null,
		true => r
	};*/




	public static R GetClientR2Screen(this HWND hwnd) => Client2Screen(hwnd, GetClientR(hwnd));

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
		var tl = hwnd.Screen2Client(e.Pos);
		var br = hwnd.Screen2Client(e.BottomRight);
		return new R(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y);
	}

	public static R Client2Screen(this HWND hwnd, R e)
	{
		var tl = hwnd.Client2Screen(e.Pos);
		var br = hwnd.Client2Screen(e.BottomRight);
		return new R(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y);
	}


	public static R GetClientR(this HWND hwnd)
	{
		GetClientRect(hwnd, out var r).Check();
		return r;
	}
	public static R GetWinR(this HWND hwnd)
	{
		GetWindowRect(hwnd, out var r).Check();
		return r;
	}

	public static WindowStyles GetStyles(this HWND hwnd) => (WindowStyles)hwnd.GetParam(WindowLongFlags.GWL_STYLE);
	public static WindowStylesEx GetStylesEx(this HWND hwnd) => (WindowStylesEx)hwnd.GetParam(WindowLongFlags.GWL_EXSTYLE);

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
}