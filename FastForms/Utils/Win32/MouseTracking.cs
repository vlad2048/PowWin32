using System.Reactive.Linq;
using System.Runtime.InteropServices;
using PowRxVar;
using PowWin32.Diag;
using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using Vanara.PInvoke;

namespace FastForms.Utils.Win32;

static class MouseTracking
{
	public static Pt GetCursorPos()
	{
		User32.GetCursorPos(out var pt).Check();
		return pt;
	}

	public static void EnableMouseTracking(this SysWin sys)
	{
		var isTracking = Var.Make(false, sys.D);
		isTracking
			.Select(isTracking_ => isTracking_ switch {
				true => sys.Evt.WhenMouseLeave.Select(_ => false),
				false => sys.Evt.WhenMouseMove.Select(_ => true),
			})
			.Switch()
			.Subscribe(isTracking_ => isTracking.V = isTracking_).D(sys.D);

		isTracking.Where(e => e).Subscribe(_ =>
		{
			CallTrackMouseEvent(sys.Handle);
		}).D(sys.D);
	}


	private static void CallTrackMouseEvent(HWND hwnd)
	{
		var opt = new User32.TRACKMOUSEEVENT {
			cbSize = (uint)Marshal.SizeOf<User32.TRACKMOUSEEVENT>(),
			dwFlags = User32.TME.TME_LEAVE,
			hwndTrack = hwnd
		};
		User32.TrackMouseEvent(ref opt);
	}
}