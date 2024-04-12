using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using FastForms.LINQPad.Hooking.Events;
using PowRxVar;
using Vanara.PInvoke;

namespace FastForms.LINQPad.Hooking;

public sealed class HookDispatcher : IDisposable
{
	private readonly Dictionary<User32.HookType, User32.HookProc> hookProcMap;
	private IEnumerable<User32.HookType> HookTypes => hookProcMap.Keys;

	private readonly Disp d;
	public void Dispose() => d.Dispose();

	private readonly Subject<IHookEvt> whenEvt;
	private readonly Dictionary<User32.HookType, User32.SafeHHOOK> connections;
	private readonly HashSet<HWND> deadWins = new();

	public IObservable<IHookEvt> WhenEvt => whenEvt;

	public HookDispatcher(Disp d)
	{
		this.d = d;
		whenEvt = new Subject<IHookEvt>().D(d);

		hookProcMap = new() {
			{ User32.HookType.WH_CALLWNDPROC, HookProc_CALLWNDPROC },
			{ User32.HookType.WH_CALLWNDPROCRET, HookProc_CALLWNDPROCRET },
			{ User32.HookType.WH_MOUSE, HookProc_MOUSE },
			{ User32.HookType.WH_CBT, HookProc_CBT },
		};

		var threadId = Kernel32.GetCurrentThreadId();
		connections = HookTypes.ToDictionary(e => e, e => User32.SetWindowsHookEx(e, hookProcMap[e], 0, (int)threadId)).D(d);
	}





	private nint HookProc_CALLWNDPROC(int nCode, nint wParam, nint lParam)
	{
		var nfo = Marshal.PtrToStructure<User32.CWPSTRUCT>(lParam);
		if (deadWins.Contains(nfo.hwnd)) return User32.CallNextHookEx(0, nCode, wParam, lParam);

		var evt = HookEvt.WndProc(nfo.hwnd, (WM)nfo.message, nfo.wParam, nfo.lParam);
		whenEvt.OnNext(evt);

		if (evt.MsgId == WM.WM_NCDESTROY)
		{
			deadWins.Add(evt.Hwnd);
		}

		return nCode switch
		{
			0 => 0,
			< 0 => User32.CallNextHookEx(0, nCode, wParam, lParam),
			_ => 0
		};
	}



	private nint HookProc_CALLWNDPROCRET(int nCode, nint wParam, nint lParam)
	{
		var nfo = Marshal.PtrToStructure<User32.CWPRETSTRUCT>(lParam);
		if (deadWins.Contains(nfo.hwnd)) return User32.CallNextHookEx(0, nCode, wParam, lParam);

		var evt = HookEvt.WndProcRet(nfo.hwnd, (WM)nfo.message, nfo.wParam, nfo.lParam, nfo.lResult);
		whenEvt.OnNext(evt);

		return User32.CallNextHookEx(0, nCode, wParam, lParam);
	}


	private nint HookProc_MOUSE(int nCode, nint wParam, nint lParam)
	{
		var nfo = Marshal.PtrToStructure<User32.MOUSEHOOKSTRUCT>(lParam);
		if (deadWins.Contains(nfo.hwnd)) return User32.CallNextHookEx(0, nCode, wParam, lParam);

		var evt = HookEvt.Mouse(nfo.hwnd, (WM)wParam, (User32.HitTestValues)nfo.wHitTestCode, nfo.pt, nfo.dwExtraInfo);
		whenEvt.OnNext(evt);

		return User32.CallNextHookEx(0, nCode, wParam, lParam);
	}


	private nint HookProc_CBT(int nCode, nint wParam, nint lParam)
	{
		var cbtType = (User32.HCBT)nCode;
		switch (cbtType)
		{
			case User32.HCBT.HCBT_ACTIVATE:
			case User32.HCBT.HCBT_CREATEWND:
			case User32.HCBT.HCBT_DESTROYWND:
			case User32.HCBT.HCBT_MINMAX:
			case User32.HCBT.HCBT_MOVESIZE:
			case User32.HCBT.HCBT_SETFOCUS:
			{
				var hwnd = wParam;
				if (deadWins.Contains(hwnd)) return User32.CallNextHookEx(0, nCode, wParam, lParam);
				//var nfo = Marshal.PtrToStructure<CBT_CREATEWND>(lParam);
				var evt = HookEvt.Cbt(hwnd, cbtType, lParam);
				whenEvt.OnNext(evt);
				break;
			}
		}

		return User32.CallNextHookEx(0, nCode, wParam, lParam);
	}
}