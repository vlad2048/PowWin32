using FastForms.Docking.Enums;
using FastForms.Docking.Logic.DropZones_.Structs;
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
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FastForms.Utils.Win32;
using PowTrees.Algorithms;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;
using FastForms.Docking.Utils;
using FastForms.Docking.Logic.DockerWin_.Painting;
using FastForms.Docking.Logic.DockerWin_.Structs;
using FastForms.Docking.Logic.DropLogic_;
using FastForms.Docking.Logic.HolderWin_.Painting;
using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Utils.Btns_;
using FastForms.Utils.GdiUtils;
using FastForms.Docking.Logic.DropLogic_.Structs;
using FastForms.Docking.Structs;


namespace FastForms.Docking;

public sealed class Docker
{
	private readonly SysWin mainWindow;

	private readonly Subject<TreeMod> whenTreeMod;
	private IObservable<TreeMod> WhenTreeMod => whenTreeMod.AsObservable();
	private void TriggerTreeMod(TreeMod mod) => whenTreeMod.OnNext(mod);
	//private void TriggerTreeMod(TreeModType type, TNod<INode> nod, params HolderNode[] holderAdds) => TriggerTreeMod(new TreeMod(type, nod, holderAdds));


	private bool IsMainWindow { get; }

	public Disp D { get; }
	public SysWin Sys { get; }
	public IRoVar<TreeType> TreeType { get; }
	public IRoVar<bool> IsMaximized { get; }
	public IRwVar<HolderNode?> ActiveHolder { get; }
	public TNod<INode> Root { get; }
	public string Name { get; set; }

	public override string ToString() => Name;


	public static Docker MakeForMainWindow(TNod<INode> root, SysWin mainWindow) => new(root, mainWindow, null);
	public static Docker MakeExtra(TNod<INode> root, SysWin mainWindow, R otherR) => new(root, mainWindow, otherR);


	private Docker(TNod<INode> root, SysWin mainWindow, R? otherR)
	{
		Ass(root.V is ToolRootNode, "The root needs to be a ToolRootNode");

		// Init
		// ====
		IsMainWindow = otherR == null;
		this.mainWindow = mainWindow;
		Name = $"Docker(main:{IsMainWindow})";
		Root = root;
		D = IsMainWindow switch
		{
			true => mainWindow.D,
			false => new Disp(nameof(Docker))
		};
		var treeType = Var.Make(Root.ComputeTreeType(IsMainWindow), D);
		TreeType = treeType;
		whenTreeMod = new Subject<TreeMod>().D(D);
		ActiveHolder = Var.Make<HolderNode?>(null, D);


		// Create a SysWin if one is not provided
		// ======================================
		Sys = IsMainWindow switch
		{
			true => mainWindow,
			false => DockerFileUtils.MakeSysWin(D, treeType, otherR ?? throw new ArgumentException("Impossible"), mainWindow),
		};
		Sys.SetProp(DockingConsts.PropNames.Docker, this);
		IsMaximized = Sys.GetIsMaximizedSpec(TreeType);


		// Rect to tree changes: Attach windows to parent & Layout
		// =====================
		WhenTreeMod.Subscribe(mod =>
		{
			treeType.V = Root.ComputeTreeType(IsMainWindow);
			if (mod.NodR.HasValue) mod.Nod.V.R = mod.NodR.Value;
			LayoutCalculator.Compute(mod.Nod, TreeType.V);
			mod.HolderAdds.ForEach(holder => holder.State.Attach(this));
			LayoutApplier.Apply(Root);
		}).D(D);

		Sys.Evt.WhenSize.ToUnit().Subscribe(_ =>
		{
			Root.V.R = Sys.ClientR;
			LayoutCalculator.Compute(Root, TreeType.V);
			LayoutApplier.Apply(Root);
		}).D(D);


		// Handle Docking
		// ==============
		Dropper.Setup(this);


		// Initial Layout
		// ==============
		TriggerTreeMod(TreeMod.MakeInit(Root, Sys.ClientR));


		// Show Window
		// ===========
		Sys.Show();
	}



	internal void ExecDrop(ITarget target, Pane[] panes)
	{
		var mod = DropExec.Exec(this, target, panes);
		TriggerTreeMod(mod);
	}




	public void AddPanes(Pane[] panes, Dock? dock = null)
	{
		if (panes.Length == 0) return;
		var mod = DockerFileUtils.AddPanes(this, panes, dock);
		TriggerTreeMod(mod);
	}


	public (Docker, HolderNode) UndockPane(Pane pane, TabLabelLay jerkLay)
	{
		var holderSrc = Root.GetHolderContainingPane(pane) ?? throw new ArgumentException("Failed to find Holder");
		Ass(holderSrc.State.Panes.Count >= 2, "Cannot undock a pane if it's the last one in the holder");

		holderSrc.State.Panes.Del(pane);

		var holderDst = holderSrc.Type switch
		{
			NodeType.Tool => HolderNode.Make(NodeType.Tool, [pane], jerkLay),
			NodeType.Doc => HolderNode.Make(NodeType.Doc, [pane]),
			_ => throw new ArgumentException()
		};
		var dockerDstRoot = holderSrc.Type switch
		{
			NodeType.Tool => N.RootTool(Nod.Make<INode>(holderDst)),
			NodeType.Doc => N.RootTool(N.RootDoc(Nod.Make<INode>(holderDst))),
			_ => throw new ArgumentException()
		};

		var winR = holderSrc.State.Sys.GetWinR() + DockerLayout.WinMargStd - HolderLayout.WinBorderMarg;
		var dockerDst = MakeExtra(dockerDstRoot, mainWindow, winR);

		return (dockerDst, holderDst);
	}


	public Docker UndockHolder(HolderNode holder)
	{
		var holderNode = Root.First(e => e.V == holder);
		var mod = NodeRemover.Remove(Root, holderNode);
		TriggerTreeMod(mod);


		var winR = holder.State.Sys.GetWinR() + DockerLayout.WinMargStd - HolderLayout.WinBorderMarg;
		var dockerRoot = holder.Type switch
		{
			NodeType.Tool => N.RootTool(holderNode),
			NodeType.Doc => N.RootTool(N.RootDoc(holderNode)),
			_ => throw new ArgumentException()
		};
		var dockerDst = MakeExtra(dockerRoot, mainWindow, winR);
		return dockerDst;
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



	public static SysWin MakeSysWin(Disp d, IRoVar<TreeType> treeType, R otherR, SysWin? mainWindow)
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







	public static TreeMod AddPanes(Docker docker, Pane[] panes, Dock? dock)
	{
		Ass(panes.Length > 0, "Invalid");
		Ass(panes.All(e => e.Type == panes[0].Type), "Invalid Panes");
		var paneType = panes[0].Type;

		var dockNod = dock != null ? new DockNod(dock.SDir, docker.Root.GetHolderContainingPane(dock.Pane)) : DockNod.Empty;

		var drop = dockNod.Interpret(paneType, docker.Root);


		switch (drop)
		{
			case IAddDrop { Holder: var addHolder }:
			{
				var holder = addHolder;
				Ass(holder.Type == paneType, "Wrong node types");
				holder.State.AddPanes(panes);
				return TreeMod.Make(docker.Root.Find(addHolder));
			}

			case INewDrop newDrop:
			{
				var holder = HolderNode.Make(paneType, panes);
				newDrop.Exec(holder, docker.Root);
				//holder.State.Attach(docker);
				return new TreeMod(TreeModType.Layout, docker.Root, null, [holder]);
			}

			default:
				throw new ArgumentException();
		}
	}



	public static IRoVar<bool> GetIsMaximizedSpec(this SysWin sys, IRoVar<TreeType> treeType)
	{
		var sysIsMax = sys.GetIsMaximized();
		return Var.Make(
			sysIsMax.V && treeType.V is TreeType.ToolSingle,
			sysIsMax.Select(isMax => isMax && treeType.V is TreeType.ToolSingle),
			sys.D
		);
	}
}




