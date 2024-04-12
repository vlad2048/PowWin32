using System.Reactive.Concurrency;
using PowRxVar;
using PowWin32.Diag;
using PowWin32.Geom;
using PowWin32.Utils;
using PowWin32.Windows.Events;
using PowWin32.Windows.StructsPInvokeWM;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace PowWin32.Windows;


public sealed class SysWin
{
	public Disp D { get; }

	private readonly Func<R, R> clientRFun;
	private SafeHWND? hwnd;
	private nint classWndProcPtr;
	private WindowProcWM wndProc = null!;

	public event Action? Destroyed;
	public HWND Handle => hwnd ?? HWND.NULL;
	public bool IsCreated => Handle != 0;
	public NativeWindowEvents Evt { get; }
	public R ClientR => clientRFun(Handle.GetClientR());

	public SysWin(Disp? d = null, Func<R, R>? clientRFun = null)
	{
		D = d ?? new Disp(nameof(SysWin));
		this.clientRFun = clientRFun ?? (e => e);
		Evt = new NativeWindowEvents(D);
	}

	public R GetClientR() => Handle.GetClientR();


	public void Destroy()
	{
		if (Handle != 0)
			RxSchedUtils.Sched.Schedule(() => DestroyWindow(Handle).Check());
		else
			D.Dispose();
	}

	private void OnNCDESTROY()
	{
		Destroyed?.Invoke();
		D.Dispose();
	}



	private void OnMessage(ref WindowMessage msg)
	{
		Evt.DispatchMessage(ref msg);
		if (!msg.Handled)
			OnMessageDefault(ref msg);
		Evt.DispatchMessagePost(ref msg);
	}

	public void OnMessageDefault(ref WindowMessage msg)
	{
		msg.Result = CallWindowProc(
			classWndProcPtr,
			msg.Hwnd,
			(uint)msg.Id,
			msg.WParam,
			msg.LParam
		);
	}


	public void Attach(HWND hwnd_, nint classWndProcPtr_)
	{
		if (Handle != 0 || hwnd_ == 0 || classWndProcPtr_ == 0) throw new ArgumentException("Handle is not 0 or hwnd_ or classWndProcPtr is 0");
		hwnd = new SafeHWND(hwnd_.DangerousGetHandle());
		classWndProcPtr = classWndProcPtr_;
		wndProc = WndProc; // store to avoid GC collecting it

		this.SetParam(WindowLongFlags.GWLP_WNDPROC, wndProc.ToPtr());
	}

	public nint WndProc(HWND hwnd_, WM msgId, nint wParam, nint lParam)
	{
		var msg = new WindowMessage
		{
			Id = msgId,
			WParam = wParam,
			LParam = lParam,
			Result = 0,
			Hwnd = hwnd_
		};

		try
		{
			OnMessage(ref msg);
			return msg.Result;
		}
		catch (Exception)
		{
			//if (!HandleException(ex, this)) throw;
			return 0;
		}
		finally
		{
			if (msg.Id == WM.WM_NCDESTROY)
			{
				OnNCDESTROY();
			}
		}
	}
}