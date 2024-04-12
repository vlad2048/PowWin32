using System.Runtime.InteropServices;
using System.Text;
using FastForms.LINQPad.Hooking.Events;
using FastForms.LINQPad.Utils;
using PowRxVar;
using PowWin32.Diag;
using PowWin32.Geom;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace FastForms.LINQPad.WinLogging;

sealed record WinId(HWND Hwnd, string ClassName)
{
    public static WinId FromHwnd(HWND hwnd) => new(hwnd, WinExtras.GetClassName(hwnd));
    public static WinId? FromHwndOpt(HWND hwnd) => (hwnd == 0) ? null : FromHwnd(hwnd);
}
sealed record WinGeomNfo(R WindowRect, R ClientRect)
{
    public static readonly WinGeomNfo Empty = new(R.Empty, R.Empty);
}
sealed record WinStateNfo(bool IsVisible, bool IsEnabled, ShowWindowCommand ShowCmd)
{
    public static readonly WinStateNfo Empty = new(false, false, 0);
}

sealed record WinStylesNfo(User32.WindowStyles Styles, WindowStylesEx ExStyles)
{
	public static readonly WinStylesNfo Empty = new(0, 0);
}

sealed record CapNfo(bool IsCapture, bool IsActive, bool IsFocus, bool IsMoveSize)
{
	public static readonly CapNfo Empty = new(false, false, false, false);
}

sealed class WinState
{
    public WinId Id { get; }
    public IRwVar<WinId?> ParentId { get; }
    public IRwVar<WinId?> OwnerId { get; }
    public IRwVar<CapNfo> Capture { get; }
    public IRwVar<WinGeomNfo> GeomNfo { get; }
    public IRwVar<WinStateNfo> StateNfo { get; }
    public IRwVar<WinStylesNfo> StylesNfo { get; }

    public WinState(HWND hwnd, Disp d)
    {
        Id = WinId.FromHwnd(hwnd);
        ParentId = Var.Make<WinId?>(null, d);
        OwnerId = Var.Make<WinId?>(null, d);
        Capture = Var.Make(CapNfo.Empty, d);
        GeomNfo = Var.Make(WinGeomNfo.Empty, d);
        StateNfo = Var.Make(WinStateNfo.Empty, d);
        StylesNfo = Var.Make(WinStylesNfo.Empty, d);

        Update_All();

    }

    public void ProcessEvent(IHookEvt evt)
    {
	    switch (evt)
	    {
		    case WndProc_HookEvt e:
		    {
			    var id = e.MsgId;

				if (id is WM.WM_CREATE or WM.WM_WINDOWPOSCHANGING)
					Update_Hierarchy();

				if (id is WM.WM_WINDOWPOSCHANGING or WM.WM_WINDOWPOSCHANGED or WM.WM_SYNCPAINT)
					Update_Styles();

				if (id is WM.WM_WINDOWPOSCHANGING or WM.WM_NCPAINT or WM.WM_ACTIVATEAPP or WM.WM_ENABLE)
				{
					Update_Geom();
					Update_State();
				}

				break;
		    }
		}
    }

    private void Update_All()
    {
	    Update_Hierarchy();
	    Update_Geom();
	    Update_State();
	    Update_Styles();
	    Update_Capture();

    }

    public void Update_Capture()
    {
		//Sync.Chk(ThreadType.Win32, "UPDATECAP_THREAD_CHECK");
		//var cap = User32.GetCapture();
		//Console.WriteLine($"____cap:{cap.Fmt()}");

		var nfo = new GUITHREADINFO
		{
			cbSize = (uint)Marshal.SizeOf<GUITHREADINFO>(),
		};

		//Sync.Chk(ThreadType.LinqPad, "UPDATECAP_THREAD_CHECK");

		var res = GetGUIThreadInfo(Sync.Win32ThreadId, ref nfo);

		//Console.WriteLine($"VS_TID: {Sync.Win32ThreadId}   cap:{nfo.hwndCapture.Fmt()}  res={res}");

		Capture.V = new CapNfo(
			nfo.hwndCapture == Id.Hwnd,
			nfo.hwndActive == Id.Hwnd,
			nfo.hwndFocus == Id.Hwnd,
			nfo.hwndMoveSize == Id.Hwnd
		);
    }

	private void Update_Hierarchy()
    {
	    ParentId.V = WinId.FromHwndOpt(User32.GetAncestor(Id.Hwnd, User32.GetAncestorFlag.GA_PARENT));
	    OwnerId.V = WinId.FromHwndOpt(User32.GetWindow(Id.Hwnd, User32.GetWindowCmd.GW_OWNER));
    }

    private void Update_Geom()
    {
	    GeomNfo.V = new WinGeomNfo(
			Id.Hwnd.GetClientR2Screen(),
			Id.Hwnd.GetClientR()
	    );
    }

    private void Update_State()
    {
	    StateNfo.V = new WinStateNfo(
		    Id.Hwnd.GetStyles().HasFlag(User32.WindowStyles.WS_VISIBLE),
		    Id.Hwnd.GetIsEnabled(),
		    Id.Hwnd.GetShowCmd()
		);
    }

    private void Update_Styles()
    {
	    StylesNfo.V = new WinStylesNfo(
		    Id.Hwnd.GetStyles(),
		    Id.Hwnd.GetStylesEx()
		);
    }
}
