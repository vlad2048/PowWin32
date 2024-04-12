using FastForms.Docking.Enums;
using FastForms.Docking.Logic.DropZones_.Structs;
using FastForms.Docking.Logic.HolderWin_;
using FastForms.Docking.Logic.Layout_;
using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Docking.Logic.Tree_;
using PowBasics.CollectionsExt;
using PowRxVar;
using PowTrees.Algorithms;
using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.Structs;
using Vanara.PInvoke;

namespace FastForms.Docking;

/*
sealed class DockerState
{
	private bool IsMainWindow { get; }
	private IRwVar<TreeType> treeType { get; }

	public Disp D { get; }
	public SysWin Sys { get; }
	public IRoVar<TreeType> TreeType => treeType;
	public IRwVar<HolderWin?> ActiveHolder { get; }
	public TNod<INode> Root { get; }


	public DockerState(TNod<INode> root, SysWin? mainSys, R? otherR)
	{
		Root = root;
		IsMainWindow = mainSys != null;
		D = mainSys?.D ?? new Disp(nameof(DockerState));
		treeType = Var.Make(Root.ComputeTreeType(IsMainWindow), D);
		Sys = mainSys ?? DockerStateFileUtils.MakeSysWin(D, treeType, otherR ?? throw new ArgumentException("Impossible"));

		ActiveHolder = Var.Make<HolderWin?>(null, D);

		Root.V.R = docker.Sys.ClientR;
		LayoutCalculator.Compute(Root, TreeType.V);
	}




	// This cannot be called in the constructor as at that stage docker.State == null
	// So we're calling it right after instead
	public void Realize()
	{
		Root
			.OfTypeNod<INode, HolderNode>()
			.ForEach(holder => holder.Realize(docker));
	}


	public void RecalcLayout()
	{
		Root.V.R = docker.Sys.ClientR;
		LayoutCalculator.Compute(Root, TreeType.V);
		LayoutApplier.Apply(Root);
	}



	public void AddPanes(Pane[] panes, Dock? dock)
	{
		if (panes.Length == 0) return;
		Ass(panes.All(e => e.Type == panes[0].Type), "Invalid Panes");
		var paneType = panes[0].Type;

		var dockNod = dock != null ? new DockNod(dock.SDir, GetHolderContainingPane(dock.Pane)) : DockNod.Empty;

		var drop = dockNod.Interpret(paneType, Root);

		HolderNode holder;

		switch (drop)
		{
			case IAddDrop { Holder: var addHolder }:
			{
				holder = addHolder;
				Ass(holder.Type == paneType, "Wrong node types");
				break;
			}

			case INewDrop newDrop:
			{
				holder = HolderNode.Make(paneType, panes);
				newDrop.Exec(holder, Root);
				holder.Realize(docker);
				TreeType.V = Root.ComputeTreeType(isMainWindow);
				break;
			}

			default:
				throw new ArgumentException();
		}
	}




	//private HolderNode? GetHolderContainingPane(Pane? pane) => pane != null ? wins.Where(kv => kv.Value.State.PanesArr.Contains(pane)).Select(kv => kv.Key).FirstOrDefault() : null;

	private HolderNode? GetHolderContainingPane(Pane? pane) => pane switch {
		null => null,
		not null => Root.Select(e => e.V).OfType<HolderNode>().FirstOrDefault(e => e.State.PanesArr.Contains(pane))
	};
}
*/





/*
file static class DockerStateFileUtils
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



	public static SysWin MakeSysWin(Disp d, IRoVar<TreeType> treeType, R otherR)
	{
		var sys = new SysWin(d, clientR => Layout.AdjustClientR(clientR, treeType.V is Enums.TreeType.ToolSingle));
		Class.CreateWindow(sys, Styles, otherR, 0, "DockerWin");
		return sys;
	}
}
*/









