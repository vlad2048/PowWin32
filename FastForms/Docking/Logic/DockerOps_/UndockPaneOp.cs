using FastForms.Docking.Logic.DockerWin_.Painting;
using FastForms.Docking.Logic.HolderWin_.Painting;
using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Docking.Logic.Tree_;
using FastForms.Utils.Win32;
using PowWin32.Geom;
using PowWin32.Windows.Utils;

// ReSharper disable once CheckNamespace
namespace FastForms.Docking;

static class UndockPaneOp
{
	public static void UndockPane(this Docker docker, Pane pane, TabLabelLay jerkLay, Pt grabPos)
	{
		var holderSrc = docker.Root.GetHolderContainingPane(pane) ?? throw new ArgumentException("Failed to find Holder");
		AssMsg(holderSrc.State.Panes.Count >= 2, "Cannot undock a pane if it's the last one in the holder");

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
		var dockerDst = Docker.MakeExtra(dockerDstRoot, docker.MainWindow, winR);

		WinMoveInitiator.Start(dockerDst.Sys, grabPos, () => holderDst.State.JerkLay.V = null);
	}
}