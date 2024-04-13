using FastForms.Docking.Enums;
using FastForms.Docking.Logic.Layout_;
using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Docking.Logic.Tree_;
using PowBasics.CollectionsExt;
using PowRxVar;
using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.Structs;
using PowWin32.Windows.StructsPackets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FastForms.Utils.Win32;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;
using FastForms.Docking.Utils;
using FastForms.Docking.Logic.DockerWin_.Painting;
using FastForms.Docking.Logic.DockerWin_.Structs;
using FastForms.Docking.Utils.Btns_;
using FastForms.Utils.GdiUtils;
using FastForms.Docking.Structs;
using PowTrees.Algorithms;
using FastForms.Docking.Logic.DockerInteractions_;
using FastForms.Utils.WinEventUtils;
using PowWin32.Windows.StructsPInvoke;


namespace FastForms.Docking;

public sealed class Docker
{
	private readonly Subject<ITreeMod> whenTreeMod;
	private IObservable<ITreeMod> WhenTreeMod => whenTreeMod.AsObservable();
	private bool IsMainWindow { get; }


	internal SysWin MainWindow { get; }
	internal void TriggerTreeMod(ITreeMod mod) => whenTreeMod.OnNext(mod);


	public Disp D { get; }
	public SysWin Sys { get; }

	public IRoVar<TreeType> TreeType { get; }
	public IRoVar<bool> IsHolderFrame { get; }
	public IRoVar<bool> IsMaximized { get; }

	public IRwVar<HolderNode?> ActiveHolder { get; }
	public TNod<INode> Root { get; }
	public string Name { get; set; }
	public override string ToString() => Name;


	public static Docker MakeForMainWindow(TNod<INode> root, SysWin mainWindow, bool dbg = false) => new(root, mainWindow, null, dbg);
	public static Docker MakeExtra(TNod<INode> root, SysWin mainWindow, R otherR, bool dbg = false) => new(root, mainWindow, otherR, dbg);


	private Docker(TNod<INode> root, SysWin mainWindow, R? otherR, bool dbg)
	{
		AssMsg(root.V is ToolRootNode, "The root needs to be a ToolRootNode");

		// Init
		// ====
		IsMainWindow = otherR == null;
		MainWindow = mainWindow;
		Name = $"Docker(main:{IsMainWindow})";
		Root = root;
		D = IsMainWindow switch
		{
			true => mainWindow.D,
			false => new Disp(nameof(Docker))
		};
		whenTreeMod = new Subject<ITreeMod>().D(D);
		ActiveHolder = Var.Make<HolderNode?>(null, D);
		ActiveHolder.Log();

		TreeType = Vars.GetTreeType(Root, WhenTreeMod, D);
		IsHolderFrame = Vars.GetIsHolderFrame(Root, WhenTreeMod, IsMainWindow, D);

		// Create a SysWin if one is not provided
		// ======================================
		Sys = IsMainWindow switch
		{
			true => mainWindow,
			false => SysWinMaker.Make(D, TreeType, IsHolderFrame, otherR ?? throw new ArgumentException("Impossible"), mainWindow, dbg),
		};
		Sys.SetProp(DockingConsts.PropNames.Docker, this);

		IsMaximized = Sys.GetIsMaximized();



		if (dbg)
		{
			TreeType.Log();
			WhenTreeMod.Log();
		}


		// Rect to tree changes: Attach windows to parent & Layout
		// =====================
		WhenTreeMod.Subscribe(mod =>
		{
			switch (mod)
			{
				case InitTreeMod:
					Root.V.R = Sys.ClientR;
					LayoutCalculator.Compute(Root, TreeType.V);
					Root.OfTypeNod<INode, HolderNode>().ForEach(holder => holder.State.Attach(this));
					LayoutApplier.Apply(Root);
					break;

				case PaintNodeTreeMod { Node: var node }:
					Root.Find(node).OfTypeNod<INode, HolderNode>().ForEach(holder => holder.State.Repaint());
					break;

				case AddHoldersTreeMod { Holders: var holders }:
					Root.V.R = Sys.ClientR;
					LayoutCalculator.Compute(Root, TreeType.V);
					holders.ForEach(holder => holder.State.Attach(this));
					LayoutApplier.Apply(Root);
					break;

				case RecomputeLayoutTreeMod:
					LayoutCalculator.Compute(Root, TreeType.V);
					LayoutApplier.Apply(Root);
					break;

				default:
					throw new ArgumentException();
			}

		}).D(D);

		Sys.Evt.WhenSize.ToUnit().Subscribe(_ =>
		{
			Root.V.R = Sys.ClientR;
			LayoutCalculator.Compute(Root, TreeType.V);
			LayoutApplier.Apply(Root);
		}).D(D);

		TreeType.Subscribe(_ =>
		{
			if (dbg) L("TreeType -> Invalidate");
			Sys.Invalidate();
		}).D(D);


		// Handle Docking
		// ==============
		DockerDocking.Setup(this);


		// Initial Layout
		// ==============
		TriggerTreeMod(new InitTreeMod());

		// Handle Split Resizing
		// =====================
		SplitResizing.Setup(this);

		ActiveHolder.Subscribe(_ =>
		{
			TriggerTreeMod(new PaintNodeTreeMod(Root.V));
		}).D(D);


		// Show Window
		// ===========
		Sys.Show();


		var idx = 0;

		Sys.WhenKey(VirtualKey.S).Subscribe(_ =>
		{
			var splitNode = Root.Select(e => e.V).OfType<SplitNode>().FirstOrDefault();
			if (splitNode == null) return;
			splitNode.Pos = (idx++ % 2 == 0) ? 50 : 200;
			TriggerTreeMod(new RecomputeLayoutTreeMod());
		}).D(D);
	}
}




file static class Vars
{
	public static IRoVar<TreeType> GetTreeType(TNod<INode> root, IObservable<ITreeMod> whenTreeMod, Disp d) =>
		Var.Make(
			root.ComputeTreeType(),
			whenTreeMod.Select(_ => root.ComputeTreeType()),
			d
		);

	public static IRoVar<bool> GetIsHolderFrame(TNod<INode> root, IObservable<ITreeMod> whenTreeMod, bool isMainWindow, Disp d)
	{
		bool Compute() => !isMainWindow && root.Kids is [{ V: ToolHolderNode }];
		return Var.Make(
			Compute(),
			whenTreeMod.Select(_ => Compute()),
			d
		);
	}


	private static TreeType ComputeTreeType(this TNod<INode> root)
	{
		var toolCnt = root.Count(e => e.V is ToolHolderNode);
		var docCnt = root.Count(e => e.V is DocHolderNode);
		return (toolCnt, docCnt) switch
		{
			(0, 0) => TreeType.Empty,
			(_, 0) => TreeType.Tool,
			(0, _) => TreeType.Doc,
			_ => TreeType.Mixed
		};
	}
}





file static class SysWinMaker
{
	private static readonly Gdi32.SafeHBRUSH BkgBrush = MkBrushGdi(0xEEEEF2);
	private static readonly WinClass Class = new(
		"DockerWin",
		styles: User32.WindowClassStyles.CS_HREDRAW | User32.WindowClassStyles.CS_VREDRAW,
		hBrush: BkgBrush
	);
	private static readonly WinStylesDef Styles = new(
		User32.WindowStyles.WS_CLIPSIBLINGS | User32.WindowStyles.WS_CLIPCHILDREN |
		User32.WindowStyles.WS_POPUP |
		User32.WindowStyles.WS_SYSMENU | User32.WindowStyles.WS_MAXIMIZEBOX | User32.WindowStyles.WS_MINIMIZEBOX | User32.WindowStyles.WS_SIZEBOX | User32.WindowStyles.WS_DLGFRAME | User32.WindowStyles.WS_BORDER,
		User32.WindowStylesEx.WS_EX_WINDOWEDGE
	);



	public static SysWin Make(Disp d, IRoVar<TreeType> treeType, IRoVar<bool> isHolderFrame, R otherR, SysWin? mainWindow, bool dbg)
	{
		var sys = new SysWin(d, clientR => DockerLayout.AdjustClientR(clientR, isHolderFrame.V));

		sys.EnableMouseTracking();
		var isMaximized = sys.GetIsMaximized();
		NcCalcTweaker.Tweak(sys, () => isMaximized.V ? DockerLayout.WinMargMax : DockerLayout.WinMargStd);
		var btns = new BtnSet<DockerWinSysBtnId>(sys, DockerWinPainterStyle.BtnStyle, () => new Pt(0, 1));
		HitTester.ForFrame(sys, DockerLayout.CaptionHeight + 1, btns.IsOverAnyButton);
		btns.WhenClicked.Subscribe(btn => btn.Execute(sys)).D(sys.D);

		Var.Merge(treeType, isMaximized).Subscribe(_ => btns.ShowButtons(DockerWinSysBtnUtils.GetBtns(treeType.V, isHolderFrame.V, isMaximized.V))).D(sys.D);

		sys.Evt.WhenPaint.Subs((ref PaintPacket e) =>
		{
			if (dbg) L($"Paint: {treeType.V}");
			if (isHolderFrame.V) return;
			using var _ = e.Paint(out var gfx);
			DockerWinPainter.Paint(
				gfx,
				sys.GetClientR(),
				"FastForms",
				sys.IsActive(),
				btns
			);
		});

		Class.CreateWindow(sys, Styles, otherR, mainWindow?.Handle ?? 0, "DockerWin");
		return sys;
	}
}




