using FastForms.Docking.Logic.DropLogic_.Structs;
using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Docking.Logic.Tree_;
using FastForms.Docking.Structs;
using PowWin32.Geom;

namespace FastForms.Docking.Logic.DropLogic_;

static class DropExec
{
	public static TreeMod Exec(Docker docker, ITarget target, Pane[] panes)
	{
		var root = docker.Root;

		if (target is MergeTarget { Holder: var mergeHolder })
		{
			mergeHolder.State.AddPanes(panes);
			return new TreeMod(TreeModType.Layout, root.GetNod(mergeHolder), null, []);
		}


		var dstHolder = HolderNode.Make(target.DstType, panes);
		var dstNod = Nod.Make<INode>(dstHolder);


		switch (target)
		{
			case InitTarget { Type: var dstType }:
				var initRoot = root.GetTypeRoot(dstType);
				Ass(initRoot.Kids.Count == 0, "Root should be empty");
				initRoot.Kids.Add(dstNod);
				return new TreeMod(TreeModType.Layout, initRoot, null, [dstHolder]);

			case ISplitTarget { Dir: var dir } splitTarget:
				dstNod = target switch
				{
					SplitCreateDocRootTarget => Nod.Make(new DocRootNode(), dstNod),
					_ => dstNod,
				};
				var splitNod = splitTarget switch
				{
					SplitTarget {Holder: var holder} => root.GetNod(holder),
					SplitRootTarget {Type: NodeType.Tool} => root.Kids.Single(),
					SplitRootTarget {Type: NodeType.Doc} => root.GetTypeRoot(NodeType.Doc),
					SplitCreateDocRootTarget { Holder: var holder } => root.GetNod(holder),
					_ => throw new ArgumentException(),
				};
				var ptr = splitNod.GetDadPtr(root);
				ptr.Kid = MkSplit(target.DstType, ptr.Kid, dstNod, dir);
				return new TreeMod(TreeModType.Layout, ptr.Node.GetDadPtr(root).Node, null, [dstHolder]);

			default:
				throw new ArgumentException();
		}
	}



	// @formatter:off
	private static TNod<INode> MkSplit(NodeType type, TNod<INode> nodePrev, TNod<INode> nodeNext, SDir sdir) => sdir switch {
		SDir.Left	=> Nod.Make(new SplitNode(type, Dir.Horz), nodeNext, nodePrev),
		SDir.Right	=> Nod.Make(new SplitNode(type, Dir.Horz), nodePrev, nodeNext),
		SDir.Up		=> Nod.Make(new SplitNode(type, Dir.Vert), nodeNext, nodePrev),
		SDir.Down	=> Nod.Make(new SplitNode(type, Dir.Vert), nodePrev, nodeNext),
		_ => throw new ArgumentException()
	};
	// @formatter:on
}



file static class DropExecFileExt
{
	public static TNod<INode> GetTypeRoot(this TNod<INode> root, NodeType type) => type switch
	{
		NodeType.Tool => root.Ass(e => e.V is ToolRootNode),
		NodeType.Doc => root.Single(e => e.V is DocRootNode),
		_ => throw new ArgumentException()
	};

	public static TNod<INode> GetNod(this TNod<INode> root, INode node) => root.First(e => e.V == node);

	/*public static NodeType GetNodeType(this Pane[] panes) =>
		panes.Length switch
		{
			0 => throw new ArgumentException("Cannot be empty"),
			_ => (panes.All(e => e.Type is NodeType.Tool), panes.All(e => e.Type is NodeType.Doc)) switch
			{
				(true, false) => NodeType.Tool,
				(false, true) => NodeType.Doc,
				_ => throw new ArgumentException("Impossible")
			}
		};*/
}