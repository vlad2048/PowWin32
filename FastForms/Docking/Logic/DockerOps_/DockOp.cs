using FastForms.Docking.Enums;
using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Docking.Logic.Tree_;
using FastForms.Docking.Structs;
using PowWin32.Geom;

// ReSharper disable once CheckNamespace
namespace FastForms.Docking;

public static class DockOp
{
	private const SDir DefaultToolDir = SDir.Right;
	private const SDir DefaultDocDir = SDir.Left;

	public static void Dock(this Docker docker, Pane[] panes, Dock? dock = null)
	{
		dock ??= Enums.Dock.Empty;
		Pane[] panesTool = [..panes.Where(e => e.Type == NodeType.Tool)];
		Pane[] panesDoc = [..panes.Where(e => e.Type == NodeType.Doc)];

		if (panesTool.Length > 0)
		{
			var target = dock.Interpret(NodeType.Tool, docker);
			var holderNod = Nod.Make<INode>(new ToolHolderNode(panesTool, null));
			docker.DockToTarget(holderNod, target);
		}

		if (panesDoc.Length > 0)
		{
			var target = dock.Interpret(NodeType.Doc, docker);
			var holderNod = Nod.Make<INode>(new DocHolderNode(panesDoc));
			docker.DockToTarget(holderNod, target);
		}
	}



	private static ITarget Interpret(this Dock dstDock, NodeType srcType, Docker docker)
	{
		TNod<INode>? toolHolder;
		TNod<INode>? docHolder;

		var root = docker.Root;

		var (dstHolder, dstDir) = (root.GetHolderContainingPane(dstDock.Pane), dstDock.Dir);

		if (root.Kids.Count == 0 || (srcType is NodeType.Doc && root.TryFind<DocRootNode>(out var docRoot) && docRoot.Kids.Count == 0))
			return new InitTarget(srcType);


		return (srcType, dstHolder, dstDir) switch
		{
			(NodeType.Tool, null, null) => root.TryFind<ToolHolderNode>(out toolHolder) switch
			{
				true => new MergeTarget((ToolHolderNode)toolHolder.V),
				false => new SplitRootTarget(NodeType.Tool, DefaultToolDir),
			},
			(NodeType.Tool, null, { } dstDirVal) => new SplitRootTarget(NodeType.Tool, dstDirVal),
			(NodeType.Tool, { } holder, null) => new MergeTarget(holder),
			(NodeType.Tool, { } holder, { } dstDirVal) => new SplitTarget(holder, dstDirVal),

			(NodeType.Doc, null or ToolHolderNode, null) => root.TryFind<DocHolderNode>(out docHolder) switch
			{
				true => new MergeTarget((DocHolderNode)docHolder.V),
				false => root.TryFind<DocRootNode>(out _) switch
				{
					true => throw new ArgumentException("The case of a DocRoot with no DocHolderNode should have been handled above (InitTarget)"),
					false => root.TryFind<ToolHolderNode>(out toolHolder) switch
					{
						true => new SplitCreateDocRootTarget((ToolHolderNode)toolHolder.V, DefaultDocDir),
						false => throw new ArgumentException("The case of an empty ToolRoot should have been handled above (InitTarget)"),
					},
				},
			},
			(NodeType.Doc, null or ToolHolderNode, { } dstDirVal) => root.TryFind<DocHolderNode>(out docHolder) switch
			{
				true => new SplitTarget((DocHolderNode)docHolder.V, dstDirVal),
				false => root.TryFind<DocRootNode>(out _) switch
				{
					true => throw new ArgumentException("The case of a DocRoot with no DocHolderNode should have been handled above (InitTarget)"),
					false => root.TryFind<ToolHolderNode>(out toolHolder) switch
					{
						true => new SplitCreateDocRootTarget((ToolHolderNode)toolHolder.V, dstDirVal),
						false => throw new ArgumentException("The case of an empty ToolRoot should have been handled above (InitTarget)"),
					},
				},
			},
			(NodeType.Doc, DocHolderNode holder, null) => new MergeTarget(holder),
			(NodeType.Doc, DocHolderNode holder, { } dstDirVal) => new SplitTarget(holder, dstDirVal),

			_ => throw new ArgumentException(),
		};
	}
}
