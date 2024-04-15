using FastForms.Docking.Enums;
using FastForms.Docking.Logic.HolderWin_.Painting;
using PowRxVar;
using PowWin32.Windows;
using PowWin32.Windows.Structs;
using System.Reactive.Linq;
using FastForms.Docking.Logic.HolderWin_.Structs;
using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Docking.Utils.Btns_;
using PowWin32.Geom;
using PowWin32.Windows.Events;
using Vanara.PInvoke;
using FastForms.Docking.Utils;
using FastForms.Utils.GdiUtils;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.StructsPackets;
using PowWin32.Windows.Utils;
using System.Reactive;
using System.Reactive.Subjects;
using PowBasics.CollectionsExt;
using PowWin32.Diag;
using FastForms.Docking.Logic.HolderWin_.Logic;
using FastForms.Utils.RxUtils;
using FastForms.Utils.Win32;
using FastForms.Docking.Logic.DockerInteractions_;
using Layout = FastForms.Docking.Logic.HolderWin_.Painting.HolderLayout;

namespace FastForms.Docking.Logic.HolderWin_;


public sealed class HolderState
{
	private static readonly WinClass Class = new(
		"HolderNode",
		styles: User32.WindowClassStyles.CS_HREDRAW | User32.WindowClassStyles.CS_VREDRAW,
		hBrush: Gdi32.GetStockObject(Gdi32.StockObjectType.NULL_BRUSH)
	);

	private static readonly WinStylesDef Styles = new(
		User32.WindowStyles.WS_VISIBLE |
		User32.WindowStyles.WS_CLIPSIBLINGS | User32.WindowStyles.WS_CLIPCHILDREN |
		User32.WindowStyles.WS_CHILD,
		0
	);

	private static readonly Brush BackBrush = MkBrush(0x000000);

	
	
	private Disp D { get; }
	private SysWin? sys;
	private readonly HolderNode holderNode;
	private readonly IRwVar<Docker?> docker;
	private readonly Subject<Pane[]> whenPanesAdded;



	public SysWin Sys => sys ?? throw new ArgumentException("Should not be null");
	public Docker Docker => docker.V ?? throw new ArgumentException("Should not be null");

	public RxArr<Pane> Panes { get; }
	public IObservable<Pane[]> WhenPanesAdded => whenPanesAdded.AsObservable();

	public string Title
	{
		get
		{
			var idx = Panes.Idx.V;
			if (idx >= 0 && idx < Panes.Arr.V.Length) return Panes.Arr.V[idx].Name;
			return "_";
		}
	}

	public IRwVar<IAutohide> Autohide { get; }
	public IRwVar<int?> HoveredTab { get; }
	public IRwVar<TabLabelLay?> JerkLay { get; }

	public IRoVar<TreeType> TreeType { get; }
	public IRoVar<bool> IsHolderFrame { get; }
	public IRoVar<bool> IsMaximized { get; }


	public HolderState(HolderNode holderNode, Pane[] initPanes, TabLabelLay? jerkLay, Disp d)
	{
		this.holderNode = holderNode;
		D = d;
		docker = Var.Make<Docker?>(null, D);

		whenPanesAdded = new Subject<Pane[]>().D(D);
		Panes = new RxArr<Pane>(initPanes, D);

		Autohide = Var.Make<IAutohide>(new AutohideOff(), D);
		HoveredTab = Var.Make<int?>(null, D);
		JerkLay = Var.Make(jerkLay, D);

		TreeType = docker.Switch(Enums.TreeType.Empty, e => e.TreeType, D);
		IsHolderFrame = docker.Switch(false, e => e.IsHolderFrame, D);
		IsMaximized = docker.Switch(false, e => e.IsMaximized, D);
	}

	public override string ToString() => $"Holder({Panes.Arr.V.Select(e => e.Name).JoinText(", ")})";

	public void Attach(Docker docker_)
	{
		docker.V = docker_;

		if (sys == null)
		{
			MakeSysWin();
			whenPanesAdded.OnNext(Panes.Arr.V);
		}
		else
		{
			User32.SetParent(Sys.Handle, Docker.Sys.Handle).Check();
			sys.SetWindowPos_MoveSizeShow(holderNode.R);
		}
	}

	public void Repaint()
	{
		Sys.Invalidate();
		Panes.Arr.V.ForEach(pane => pane.Repaint());
	}


	public void AddPanes(Pane[] panes, int? insertIndex)
	{
		Panes.Add(panes, insertIndex);
		whenPanesAdded.OnNext(panes);
	}




	private void MakeSysWin()
	{
		AssMsg(sys == null, "Should  be null");

		// Create the window
		// =================
		sys = new SysWin(D, clientR => HolderLayout.AdjustClientR(clientR, IsHolderFrame.V));
		sys.EnableMouseTracking();		// needed for tearing off panes of the holder itself


		// Sys Buttons
		// ===========
		var btns = new BtnSet<HolderBtnId>(
			Sys,
			HolderWinPainterStyle.BtnStyle,
			() => IsHolderFrame.V ? new Pt(4, 3) : new Pt(5, 3)
		);
		btns.WhenClicked.Subscribe(btn => btn.Execute(Docker)).D(Sys.D);
		Var.Merge(TreeType, IsHolderFrame, IsMaximized, Autohide).Subscribe(_ => btns.ShowButtons(HolderBtnUtils.GetBtns(TreeType.V, IsHolderFrame.V, IsMaximized.V, Autohide.V))).D(D);


		// HitTesting (let caption mouse events pass through if TreeType == ToolSingle)
		// ==========
		HitTester.ForPassthrough(
			Sys,
			HolderLayout.CaptionHeight + 1,
			btns.IsOverAnyButton,
			() => IsHolderFrame.V
		);


		// Painting the frame
		// ==================
		Var.Merge(Panes.Arr, Panes.Idx, Autohide, HoveredTab).Subscribe(_ => Sys.Invalidate()).D(D);
		//Sys.Evt.DisableEraseBkgnd();
		Sys.Evt.WhenPaint.Subs((ref PaintPacket e) =>
		{
			using var _ = e.Paint(out var gfx);
			//gfx.Clear(Color.Black);
			HolderWinPainter.Paint(
				gfx,
				Sys.GetClientR(),
				Sys.ClientR,
				Title,
				active: Docker.Sys.IsActive() && Docker.ActiveHolder.V == holderNode,
				IsHolderFrame.V,
				btns,
				[.. Panes.Arr.V.Select(e => e.Name)],
				Panes.Idx.V,
				HoveredTab.V,
				JerkLay.V
			);

			var paneR = Layout.GetPaneR(Sys.ClientR, Panes.Count, JerkLay.V);
			gfx.FillRect(paneR, BackBrush);
		});


		// Track the active Pane
		// =====================
		Sys.Evt.WhenUserClicks().Subscribe(_ => Docker.ActiveHolder.V = holderNode).D(Sys.D);


		// Create the Win32 window
		// =======================
		Class.CreateWindow(Sys, Styles, holderNode.R, Docker.Sys.Handle);


		// Manage the Panes (parent, visibility, layout)
		// =============================================
		PaneManager.Setup(Sys, this);


		// Allow user to reorder Panes & Tear a Pane off
		// =============================================
		PaneReorderingAndUndocking.Setup(Sys, this);


		// Allow user the tear the Holder off (if TreeType != ToolSingle)
		// ==================================
		HolderUndocking.Setup(holderNode, btns.IsOverAnyButton);
	}
}




file static class HolderStateFileUtils
{
	public static IRoVar<T> Switch<T>(this IRoVar<Docker?> docker, T init, Func<Docker, IRoVar<T>> sel, Disp d) =>
		Var.Make(
			init,
			docker
				.Where(e => e != null)
				.Select(sel!)
				.Switch(),
			d
		);

	public static IObservable<Unit> WhenUserClicks(this NativeWindowEvents evt) => Obs.Merge(
		evt.WhenMouseButton.Where(e => e.Button == MouseButton.Left).ToUnit(),
		evt.WhenParentNotify.Where(e => e.ChildMsgId == WM.WM_LBUTTONDOWN).ToUnit()
	);
}
