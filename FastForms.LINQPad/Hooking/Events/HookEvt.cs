using PowWin32.Geom;
using PowWin32.Windows.Structs;
using Vanara.PInvoke;

namespace FastForms.LINQPad.Hooking.Events;

public interface IHookEvt
{
	HWND Hwnd { get; }
}

public sealed record WndProc_HookEvt(
	HWND Hwnd,
	WM MsgId,
	nint WParam,
	nint LParam
) : IHookEvt
{
	public WindowMessage WinMsg => new(Hwnd, MsgId, WParam, LParam);
}

public sealed record WndProcRet_HookEvt(
	HWND Hwnd,
	WM MsgId,
	nint WParam,
	nint LParam,
	nint Result
) : IHookEvt
{
	public WindowMessage WinMsg => new(Hwnd, MsgId, WParam, LParam)
	{
		Result = Result
	};
}

public sealed record Mouse_HookEvt(
	HWND Hwnd,
	WM MsgId,
	User32.HitTestValues HitTestResult,
	Pt Pt,
	nuint Extra
) : IHookEvt;

public sealed record Cbt_HookEvt(
	HWND Hwnd,
	User32.HCBT Type,
	nint LParam
) : IHookEvt;


public static class HookEvt
{
	public static WndProc_HookEvt WndProc(HWND hwnd, WM msgId, nint wParam, nint lParam) => new(hwnd, msgId, wParam, lParam);
	public static WndProcRet_HookEvt WndProcRet(HWND hwnd, WM msgId, nint wParam, nint lParam, nint result) => new(hwnd, msgId, wParam, lParam, result);
	public static Mouse_HookEvt Mouse(HWND hwnd, WM msgId, User32.HitTestValues hitTestResult, Pt pt, nuint extra) => new(hwnd, msgId, hitTestResult, pt, extra);
	public static Cbt_HookEvt Cbt(HWND hwnd, User32.HCBT type, nint lParam) => new(hwnd, type, lParam);
}