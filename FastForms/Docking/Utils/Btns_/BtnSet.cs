using FastForms.Docking.Utils.Btns_.Structs;
using FastForms.Utils;
using PowRxVar;
using PowWin32.Geom;
using PowWin32.Windows;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FastForms.Utils.Win32;
using FastForms.Utils.WinEventUtils;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace FastForms.Docking.Utils.Btns_;

sealed class BtnSet<E> where E : struct, Enum
{
	private sealed record BtnLay(E Btn, R R);

	private interface ISt;
	private sealed record NoneSt : ISt;
	private sealed record HoverSt(E Btn) : ISt;
	private sealed record PressSt(E Btn, bool IsOver) : ISt;

	private interface IEvt;
	private sealed record MoveEvt(Pt Mouse) : IEvt;
	private sealed record LeaveEvt : IEvt;
	private sealed record DownEvt : IEvt;
	private sealed record UpEvt : IEvt;
	private sealed record CaptureChangedEvt : IEvt;


	private readonly SysWin sys;
	private readonly BtnSetStyle style;
	private readonly Subject<E> whenClicked;
	private readonly IRwVar<E[]> enabledBtns;
	private readonly IRwVar<ISt> st;
	private BtnLay[] lays = [];
	private E? GetBtnAt(Pt mousePos)
	{
		foreach (var lay in lays)
			if (lay.R.Contains(mousePos))
				return lay.Btn;
		return null;
	}

	public void ShowButtons(E[] btns) => enabledBtns.V = btns;
	public IObservable<E> WhenClicked => whenClicked.AsObservable();
	public bool IsOverAnyButton(Pt mousePos) => GetBtnAt(mousePos).HasValue;
	public int VisibleBtnCount => lays.Length;


	public BtnSet(
		SysWin sys,
		BtnSetStyle style,
		Func<Pt> topRightOffsetFun
	)
	{
		this.sys = sys;
		this.style = style;
		whenClicked = new Subject<E>().D(sys.D);
		enabledBtns = Var.Make<E[]>([], sys.D);
		st = Var.Make<ISt>(new NoneSt(), sys.D);

		st.Subscribe(_ => sys.Invalidate()).D(sys.D);


		Obs.Merge(
				enabledBtns.ToUnit(),
				sys.Evt.WhenSize.ToUnit()
			)
			.Subscribe(_ =>
			{
				var isWin = sys.IsWindow();
				if (!isWin) return;
				lays = ComputeLayout(enabledBtns.V, sys.GetClientR(), topRightOffsetFun(), style);
			}).D(sys.D);


		//st.Log();

		GetEvents(sys)
			//.Do(e => L($"[{e}]"))
			.Subscribe(ProcessEvent).D(sys.D);
	}


	public void Paint(Graphics gfx, bool active)
	{
		foreach (var lay in lays)
		{
			var state = st.V switch
			{
				HoverSt { Btn: var btn } when btn.Equals(lay.Btn) => BtnMouseState.Hover,
				PressSt { Btn: var btn, IsOver: true } when btn.Equals(lay.Btn) => BtnMouseState.Pressed,
				PressSt { Btn: var btn, IsOver: false } when btn.Equals(lay.Btn) => BtnMouseState.Hover,
				_ => BtnMouseState.Normal,
			};
			var drawNfo = style.Map[(lay.Btn.ToInt(), state, active)];
			gfx.PaintBtn(drawNfo, lay.R.Pos);
		}
	}


	private void ProcessEvent(IEvt evt)
	{
		switch (st.V)
		{
			case NoneSt:
			{
				switch (evt)
				{
					case MoveEvt { Mouse: var mouse }:
						var btnMouse = GetBtnAt(mouse);
						if (btnMouse.HasValue)
							st.V = new HoverSt(btnMouse.Value);
						break;
				}
				break;
			}

			case HoverSt { Btn: var btnSt }:
			{
				switch (evt)
				{
					case MoveEvt { Mouse: var mouse }:
						var btnMouse = GetBtnAt(mouse);
						switch (btnMouse)
						{
							case null:
								st.V = new NoneSt();
								break;
							case not null when !btnMouse.Value.Equals(btnSt):
								st.V = new HoverSt(btnMouse.Value);
								break;
						}
						break;
					case LeaveEvt:
						st.V = new NoneSt();
						break;
					case DownEvt:
						st.V = new PressSt(btnSt, true);
						User32.SetCapture(sys.Handle);
						break;
				}
				break;
			}

			case PressSt { Btn: var btnSt, IsOver: var isOver }:
			{
				switch (isOver)
				{
					case true:
						switch (evt)
						{
							case MoveEvt { Mouse: var mouse } when !GetBtnAt(mouse).Equals(btnSt):
								st.V = new PressSt(btnSt, false);
								break;
							case UpEvt:
								User32.ReleaseCapture();
								break;
							case CaptureChangedEvt:
								whenClicked.OnNext(btnSt);
								st.V = new NoneSt();
								break;
						}
						break;

					case false:
						switch (evt)
						{
							case MoveEvt { Mouse: var mouse } when GetBtnAt(mouse).Equals(btnSt):
								st.V = new PressSt(btnSt, true);
								break;
							case UpEvt:
								User32.ReleaseCapture();
								break;
							case CaptureChangedEvt:
								var mouseScr = MouseTracking.GetCursorPos();
								var mouseCli = sys.Screen2Client(mouseScr);
								var mouseBtn = GetBtnAt(mouseCli);
								st.V = mouseBtn switch
								{
									null => new NoneSt(),
									not null => new HoverSt(mouseBtn.Value)
								};
								break;
						}
						break;
				}
				break;
			}

			default:
				throw new ArgumentException();
		}
	}


	private static IObservable<IEvt> GetEvents(SysWin sys) =>
		Obs.Merge<IEvt>(
			sys.WhenMove().Select(e => new MoveEvt(e)),
			sys.WhenLeave().Select(_ => new LeaveEvt()),
			sys.WhenBtnDown().Select(_ => new DownEvt()),
			sys.WhenBtnUp().Select(_ => new UpEvt()),
			sys.WhenCaptureLost().Select(_ => new CaptureChangedEvt())
		);



	private static BtnLay[] ComputeLayout(E[] btns, R clientR, Pt topRightOffset, BtnSetStyle style)
	{
		var list = new List<BtnLay>();
		var x = clientR.Right - topRightOffset.X;
		var y = clientR.Y + topRightOffset.Y;
		foreach (var btn in btns.Reverse())
		{
			var bmpSz = style.BmpSizes[btn.ToInt()];
			x -= bmpSz.Width;
			var btnR = new R(x, y, bmpSz.Width, bmpSz.Height);
			list.Add(new BtnLay(btn, btnR));
			x -= style.Gutter;
		}
		return [..Enumerable.Reverse(list)];
	}
}