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
using FastForms.Docking.Logic.DropLogic_;
using FastForms.Docking.Logic.HolderWin_.Painting;
using FastForms.Docking.Utils.Btns_;
using FastForms.Utils.GdiUtils;
using FastForms.Docking.Structs;
using PowTrees.Algorithms;


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
	public IRoVar<bool> IsToolSingleMaximized { get; }
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
		var treeType = Var.Make(Root.ComputeTreeType(IsMainWindow), D);
		TreeType = treeType;
		whenTreeMod = new Subject<ITreeMod>().D(D);
		ActiveHolder = Var.Make<HolderNode?>(null, D);


		// Create a SysWin if one is not provided
		// ======================================
		Sys = IsMainWindow switch
		{
			true => mainWindow,
			false => DockerFileUtils.MakeSysWin(D, treeType, otherR ?? throw new ArgumentException("Impossible"), mainWindow, dbg),
		};
		Sys.SetProp(DockingConsts.PropNames.Docker, this);
		IsToolSingleMaximized = Sys.IsToolSingleMaximized(TreeType);



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
					treeType.V = Root.ComputeTreeType(IsMainWindow);
					Root.V.R = Sys.ClientR;
					LayoutCalculator.Compute(Root, TreeType.V);
					Root.OfTypeNod<INode, HolderNode>().ForEach(holder => holder.State.Attach(this));
					LayoutApplier.Apply(Root);
					break;

				case PaintNodeTreeMod { Node: var node }:
					Root.Find(node).OfTypeNod<INode, HolderNode>().ForEach(holder => holder.State.Repaint());
					break;

				case AddHoldersTreeMod { Holders: var holders }:
					treeType.V = Root.ComputeTreeType(IsMainWindow);
					Root.V.R = Sys.ClientR;
					LayoutCalculator.Compute(Root, TreeType.V);
					holders.ForEach(holder => holder.State.Attach(this));
					LayoutApplier.Apply(Root);
					break;

				case RecomputeLayoutTreeMod:
					treeType.V = Root.ComputeTreeType(IsMainWindow);
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
		Dropper.Setup(this);


		// Initial Layout
		// ==============
		TriggerTreeMod(new InitTreeMod());


		// Show Window
		// ===========
		Sys.Show();
	}
}







file static class DockerFileUtils
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



	public static SysWin MakeSysWin(Disp d, IRoVar<TreeType> treeType, R otherR, SysWin? mainWindow, bool dbg)
	{
		var sys = new SysWin(d, clientR => DockerLayout.AdjustClientR(clientR, treeType.V is TreeType.ToolSingle));

		sys.EnableMouseTracking();
		var isMaximized = sys.GetIsMaximized();
		NcCalcTweaker.Tweak(sys, () => isMaximized.V ? DockerLayout.WinMargMax : DockerLayout.WinMargStd);
		var btns = new BtnSet<DockerWinSysBtnId>(sys, DockerWinPainterStyle.BtnStyle, () => new Pt(0, 1));
		HitTester.ForFrame(sys, DockerLayout.CaptionHeight + 1, btns.IsOverAnyButton);
		btns.WhenClicked.Subscribe(btn => btn.Execute(sys)).D(sys.D);

		Var.Merge(treeType, isMaximized).Subscribe(_ => btns.ShowButtons(DockerWinSysBtnUtils.GetBtns(treeType.V, isMaximized.V))).D(sys.D);

		sys.Evt.WhenPaint.Subs((ref PaintPacket e) =>
		{
			if (dbg) L($"Paint: {treeType.V}");
			if (treeType.V is TreeType.ToolSingle) return;
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




	public static IRoVar<bool> IsToolSingleMaximized(this SysWin sys, IRoVar<TreeType> treeType)
	{
		var sysIsMax = sys.GetIsMaximized();
		return Var.Make(
			sysIsMax.V && treeType.V is TreeType.ToolSingle,
			Var.Merge(sysIsMax, treeType).Select(_ => sysIsMax.V && treeType.V is TreeType.ToolSingle),
			sys.D
		);
	}
}




