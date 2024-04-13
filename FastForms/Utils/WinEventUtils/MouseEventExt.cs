using System.Reactive;
using System.Reactive.Linq;
using PowMaybe;
using PowRxVar;
using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.StructsPackets;
using Vanara.PInvoke;

namespace FastForms.Utils.WinEventUtils;

public static class MouseEventExt
{
	public static IObservable<Pt> WhenBtnDown(this SysWin sys, MouseButton btn = MouseButton.Left) => sys.Evt.WhenMouseButton.ToObs().Where(e => e.IsDown && e.Button == btn).Select(e => e.Point);

	public static IObservable<Unit> WhenBtnUp(this SysWin sys, MouseButton btn = MouseButton.Left) => sys.Evt.WhenMouseButton.ToObs().Where(e => !e.IsDown && e.Button == btn).ToUnit();

	public static IObservable<Pt> WhenMove(this SysWin sys) => sys.Evt.WhenMouseMove.ToObs().Select(e => e.Point);

	public static IObservable<Unit> WhenLeave(this SysWin sys) => sys.Evt.WhenMouseLeave.ToUnit();

	public static IObservable<Unit> WhenCaptureLost(this SysWin sys) => sys.Evt.WhenCaptureChanged
		.ToObs()
		.Where(e => e.CapturingHwnd != sys.Handle)
		.ToUnit();


	private static readonly User32.SafeHCURSOR defaultCursor = User32.LoadCursor(default, User32.IDC_ARROW);

	public static void MouseCaptureCursor<S>(
		this SysWin sys,
		MouseButton btn,
		Func<Pt, Maybe<User32.SafeHCURSOR>> cursorFun,
		Func<Pt, S> onStart,
		Action<Pt, S> onMove
	)
	{
		var state = May.None<S>();

		void Start(S s)
		{
			state = May.Some(s);
			User32.SetCapture(sys.Handle);
		}
		void Stop()
		{
			User32.ReleaseCapture();
		}

		sys.WhenMove()
			.Subscribe(p =>
			{
				if (state.IsNone(out var state_))
					User32.SetCursor(cursorFun(p).IsSome(out var cur) ? cur : defaultCursor);
				else
					onMove(p, state_);
			}).D(sys.D);


		sys.WhenBtnDown(btn)
			.Where(p => state.IsNone() && cursorFun(p).IsSome())
			.Subscribe(p =>
			{
				var st = onStart(p);
				Start(st);
			}).D(sys.D);

		sys.WhenBtnUp(btn)
			.Where(_ => state.IsSome())
			.Subscribe(_ =>
			{
				Stop();
			}).D(sys.D);

		sys.WhenCaptureLost().Subscribe(_ =>
		{
			if (state.IsNone(out var stateVal)) return;

			//onRelease(stateVal);

			state = May.None<S>();
		}).D(sys.D);
	}




	public static Func<bool> MouseCapture<S>(
		this SysWin sys,
		MouseButton btn,
		Func<Pt, Maybe<S>> onCapture,
		Action<Pt, S, Action> onMove,
		Action<S> onRelease
	)
	{
		var state = May.None<S>();

		void Start(S s)
		{
			state = May.Some(s);
			User32.SetCapture(sys.Handle);
		}
		void Stop()
		{
			User32.ReleaseCapture();
		}



		sys.WhenBtnDown(btn)
			.Where(_ => state.IsNone())
			.Subscribe(p =>
			{
				var mayStateNext = onCapture(p);
				if (mayStateNext.IsSome(out var stateNext))
					Start(stateNext);
			}).D(sys.D);

		sys.WhenMove()
			.Where(_ => state.IsSome())
			.Subscribe(p =>
			{
				onMove(p, state.Ensure(), Stop);
			}).D(sys.D);

		sys.WhenBtnUp(btn)
			.Where(_ => state.IsSome())
			.Subscribe(_ =>
			{
				Stop();
			}).D(sys.D);

		sys.WhenCaptureLost().Subscribe(_ =>
		{
			if (state.IsNone(out var stateVal)) return;

			onRelease(stateVal);
			state = May.None<S>();
		}).D(sys.D);


		return () => state.IsSome();
	}
}