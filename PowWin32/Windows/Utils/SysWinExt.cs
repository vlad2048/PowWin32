using System.Runtime.InteropServices;
using PowRxVar;
using PowWin32.Diag;
using PowWin32.Geom;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.Structs;
using PowWin32.Windows.StructsPackets;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace PowWin32.Windows.Utils;

public static class SysWinExt
{
	public static bool CreateMove(this SysWin sys, HWND parent, R r, WinClass winClass, WinStylesDef winStyles)
	{
		if (!sys.IsCreated)
		{
			winClass.CreateWindow(sys, winStyles, r, parent);
			return true;
		}
		else
		{
			SetParent(sys.Handle, parent).Check();
			sys.SetWindowPos_MoveSizeShow(r);
			return false;
		}
	}


	// ***********
	// * Drawing *
	// ***********
	public static void Invalidate(this SysWin sys) => InvalidateRect(sys.Handle, 0, false).Check();
	public static void Invalidate(this SysWin sys, R r)
	{
		RECT rect = r;
		User32.InvalidateRect(sys.Handle, rect, false).Check();
	}

	[DllImport(Lib.User32, SetLastError = false, ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool InvalidateRect(HWND hWnd, [In] nint lpRect, [MarshalAs(UnmanagedType.Bool)] bool bErase);


	// **************
	// * Rectangles *
	// **************
	public static void SetWindowPos(this SysWin sys, R r, SetWindowPosFlags flags) => User32.SetWindowPos(sys.Handle, 0, r.X, r.Y, r.Width, r.Height, flags);
	public static void SetWindowPos_MoveSize(this SysWin sys, R r) => sys.SetWindowPos(r,
		SetWindowPosFlags.SWP_NOACTIVATE |
		SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOZORDER
	);
	public static void SetWindowPos_MoveSizeShow(this SysWin sys, R r) => sys.SetWindowPos(r,
		SetWindowPosFlags.SWP_NOACTIVATE |
		SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOZORDER |
		SetWindowPosFlags.SWP_SHOWWINDOW
	);

	public static R GetWinR(this SysWin sys)
	{
		GetWindowRect(sys.Handle, out var r).Check();
		return r;
	}

	public static R GetClientR(this HWND hwnd)
	{
		GetClientRect(hwnd, out var r).Check();
		return r;
	}

	public static R GetClientRScreen(this SysWin sys) => sys.Client2Screen(sys.GetClientR());

	public static R GetDwmR(this SysWin sys) => sys.Handle.GetDwmR();
	public static R GetDwmR(this HWND hwnd)
	{
		DwmApi.DwmGetWindowAttribute<RECT>(hwnd, DwmApi.DWMWINDOWATTRIBUTE.DWMWA_EXTENDED_FRAME_BOUNDS, out var r).Check();
		return r;
	}

	public static Pt Screen2Client(this SysWin sys, Pt e)
	{
		POINT s = e;
		ScreenToClient(sys.Handle, ref s).Check();
		return s;
	}

	public static Pt Client2Screen(this SysWin sys, Pt e)
	{
		POINT s = e;
		ClientToScreen(sys.Handle, ref s).Check();
		return s;
	}

	public static R Screen2Client(this SysWin sys, R e)
	{
		var tl = sys.Screen2Client(e.Pos);
		var br = sys.Screen2Client(e.BottomRight);
		return new R(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y);
	}

	public static R Client2Screen(this SysWin sys, R e)
	{
		var tl = sys.Client2Screen(e.Pos);
		var br = sys.Client2Screen(e.BottomRight);
		return new R(tl.X, tl.Y, br.X - tl.X, br.Y - tl.Y);
	}


	// *********
	// * State *
	// *********
	public static void DetectMessageTriggers<T>(this SysWin sys, Func<T> fun) where T : notnull
	{
		var set = new HashSet<WM>();
		var hasVal = false;
		var valPrev = default(T);

		void AddId(WM id)
		{
			if (!set.Add(id)) return;
			Console.WriteLine($"Trigger: {id}   val:{valPrev}");
		}

		bool IsNew()
		{
			var valNext = fun();
			if (!hasVal || !valPrev!.Equals(valNext))
			{
				hasVal = true;
				valPrev = valNext;
				return true;
			}
			return false;
		}

		sys.Evt.WhenMessage.ToObs().Subscribe(e =>
		{
			if (IsNew())
				AddId(e.Id);
		}).D(sys.D);
	}

	public static bool IsWindow(this SysWin sys) => User32.IsWindow(sys.Handle);

	public static void Show(this SysWin sys) => ShowWindow(sys.Handle, ShowWindowCommand.SW_SHOWNORMAL);
	public static void Hide(this SysWin sys) => ShowWindow(sys.Handle, ShowWindowCommand.SW_HIDE);

	public static ShowWindowCommand GetShowCmd(this SysWin sys)
	{
		var plc = new WINDOWPLACEMENT();
		GetWindowPlacement(sys.Handle, ref plc);
		return plc.showCmd;
	}

	public static bool IsMaximized(this SysWin sys) => sys.GetShowCmd() == ShowWindowCommand.SW_MAXIMIZE;

	public static IRoVar<bool> GetIsMaximized(this SysWin sys) =>
		Var.Make(
			sys.IsMaximized(),
			sys.Evt.WhenWindowPosChanging.Select(e => sys.IsMaximized()),
			//sys.Evt.WhenWindowPosChanging.ToObs().Select(e => e.Flag.HasFlag(WindowSizeFlag.SIZE_MAXIMIZED)),
			sys.D
		);

	public static bool IsActive(this SysWin sys) => GetActiveWindow() == sys.Handle;

	//public static HWND GetParentWindow(this SysWin sys) => GetAncestor(sys.Handle, GetAncestorFlag.GA_PARENT);
	//public static HWND GetRootWindow(this SysWin sys) => GetAncestor(sys.Handle, GetAncestorFlag.GA_ROOT);



	// **********
	// * Styles *
	// **********
	public static WindowStyles GetStyles(this SysWin sys) => (WindowStyles)sys.GetParam(WindowLongFlags.GWL_STYLE);
	public static void SetStyles(this SysWin sys, WindowStyles v) => sys.SetParam(WindowLongFlags.GWL_STYLE, (int)v);

	public static bool HasStyle(this SysWin sys, WindowStyles v) => (sys.GetStyles() & v) == v;

	public static void AddStyle(this SysWin sys, WindowStyles v)
	{
		var styles = sys.GetStyles();
		sys.SetStyles(styles | v);
	}

	public static void DelStyle(this SysWin sys, WindowStyles v)
	{
		var styles = sys.GetStyles();
		sys.SetStyles(styles & ~v);
	}

	public static WindowStylesEx GetStylesEx(this SysWin sys) => (WindowStylesEx)sys.GetParam(WindowLongFlags.GWL_EXSTYLE);
	public static void SetStylesEx(this SysWin sys, WindowStylesEx v) => sys.SetParam(WindowLongFlags.GWL_EXSTYLE, (int)v);

	public static bool HasStyleEx(this SysWin sys, WindowStylesEx v) => (sys.GetStylesEx() & v) == v;

	public static void AddStyleEx(this SysWin sys, WindowStylesEx v)
	{
		var styles = sys.GetStylesEx();
		sys.SetStylesEx(styles | v);
	}

	public static void DelStyleEx(this SysWin sys, WindowStylesEx v)
	{
		var styles = sys.GetStylesEx();
		sys.SetStylesEx(styles & ~v);
	}


	// **********
	// * Params *
	// **********
	public static nint GetParam(this SysWin sys, WindowLongFlags flag)
	{
		Kernel32.SetLastError(0);
		var res = GetWindowLongPtr(sys.Handle, flag);
		ErrorExt.CheckLastErrorIf(res == 0);
		return res;
	}

	public static nint SetParam(this SysWin sys, WindowLongFlags flag, nint v)
	{
		Kernel32.SetLastError(0);
		var res = SetWindowLong(sys.Handle, flag, v);
		ErrorExt.CheckLastErrorIf(res == 0);
		return res;
	}


	// ************
	// * Schedule *
	// ************
	// ReSharper disable once CollectionNeverQueried.Local
	private static readonly List<Timerproc> procs = new();
	public static void Schedule(this SysWin sys, int id, TimeSpan period, bool recurring, Action action)
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
		SetTimer(sys.Handle, id, (uint)period.TotalMilliseconds, proc);
	}

	// *********
	// * Props *
	// *********
	public static void SetProp<T>(this SysWin win, string name, T obj) where T : class
	{
		var gcHandle = GCHandle.Alloc(obj);
		var gcHandlePtr = GCHandle.ToIntPtr(gcHandle);

		if (!User32.SetProp(win.Handle, name, gcHandlePtr).Check())
		{
			gcHandle.Free();
			return;
		}

		win.Evt.WhenNcDestroy.Subs((ref Packet _) =>
		{
			RemoveProp(win.Handle, name);
			gcHandle.Free();
		});
	}

	public static bool HasProp(this HWND hwnd, string name) => User32.GetProp(hwnd, name) != 0;

	public static T GetProp<T>(this HWND hwnd, string name) where T : class
	{
		var gcHandlePtr = User32.GetProp(hwnd, name).Check();
		var gcHandle = GCHandle.FromIntPtr(gcHandlePtr);
		if (gcHandle.Target is not T obj) throw new ArgumentException($"Failed to retrieve window property '{name}'");
		return obj;
	}

	public static T GetProp<T>(this SysWin win, string name) where T : class => win.Handle.GetProp<T>(name);
}