using FastForms.Docking.Logic.DockerWin_.Painting;
using FastForms.Docking.Logic.HolderWin_.Painting;
using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Docking.Logic.Tree_;
using FastForms.Docking.Structs;
using FastForms.Utils.Win32;
using PowWin32.Geom;
using PowWin32.Windows.Utils;

// ReSharper disable once CheckNamespace
namespace FastForms.Docking;

static class UndockHolderOp
{
	public static void UndockHolder(this Docker docker, HolderNode holder, Pt grabPos)
	{
		var root = docker.Root;
		var holderNod = root.Find(holder);
		root.RemoveNode(holderNod);
		docker.TriggerTreeMod(new RecomputeLayoutTreeMod());

		var winR = holder.State.Sys.GetWinR() + DockerLayout.WinMargStd - HolderLayout.WinBorderMarg;
		var dockerRoot = holder.Type switch
		{
			NodeType.Tool => N.RootTool(holderNod),
			NodeType.Doc => N.RootTool(N.RootDoc(holderNod)),
			_ => throw new ArgumentException()
		};

		var dockerDst = Docker.MakeExtra(dockerRoot, docker.MainWindow, winR);
		WinMoveInitiator.Start(dockerDst.Sys, grabPos, () => { });
	}


	/*
	━ ┃ ┫ ┏ ┗

	Remove(@)
	---------
	         ┏━ @
	x ━━━ s ━┫      --->     x ━━━ y
	         ┗━ y

	Rᵀ ━━━ @        --->     Rᵀ ━━━ ∅

	*/
	private static void RemoveNode(this TNod<INode> root, TNod<INode> target)
	{
		var dad = target.GetDadPtr(root);

		switch (dad.Node.V)
		{
			case ToolRootNode:
				Ass(dad.Node.Kids.Count == 1);
				dad.Node.Kids.Clear();
				break;

			case SplitNode:
				var grandDad = dad.Node.GetDadPtr(root);
				grandDad.Kid = dad.OtherKid;
				break;

			default:
				throw new ArgumentException("Cannot remove a node in this case");
		}
	}
}