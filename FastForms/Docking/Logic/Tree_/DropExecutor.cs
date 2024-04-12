using FastForms.Docking.Logic.DropZones_.Structs;
using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;
using PowWin32.Geom;

namespace FastForms.Docking.Logic.Tree_;

static class DropExecutor
{
	public static void Exec(this INewDrop drop, HolderNode holderNode, TNod<INode> root)
	{
		var holder = Nod.Make<INode>(holderNode);

		switch (drop)
		{
			case ISideNewDrop { SDir: var sdir } sideDrop:
				var splitTarget = sideDrop.GetSplitTarget(root);
				var holderFinal = sideDrop switch {
					Holder_Side_CreateDocRoot_Drop => Nod.Make(new DocRootNode(), holder),
					_ => holder
				};
				ExecSide(holderFinal, splitTarget, sdir, root);
				break;

			case ToolRoot_Init_Drop:
				Ass(root.Kids.Count == 0, "ToolRoot should be empty");
				switch (holderNode.Type)
				{
					case NodeType.Tool:
						root.Kids.Add(holder);
						break;
					case NodeType.Doc:
						root.Kids.Add(Nod.Make(new DocRootNode(), holder));
						break;
					default:
						throw new ArgumentException();
				}
				break;

			case DocRoot_Init_Drop:
				Ass(holderNode.Type == NodeType.Doc, "Can only init the DocRoot with a Doc");
				var docRoot = root.First(e => e.V is DocRootNode);
				Ass(docRoot.Kids.Count == 0, "DocRoot should be empty");
				break;

			default:
				throw new ArgumentException();
		}
	}


	private static TNod<INode> GetSplitTarget(this ISideNewDrop sideDrop, TNod<INode> root)
	{
		var nodeMap = root.ToDictionary(e => e.V);
		return sideDrop switch
		{
			Holder_Side_Drop { Holder: var holder } => nodeMap[holder],
			Holder_Side_CreateDocRoot_Drop { Holder: var holder } => nodeMap[holder],
			ToolRoot_Side_Drop => root.Kids[0],
			DocRoot_Side_Drop => root.First(e => e.V is DocRootNode),
			_ => throw new ArgumentException()
		};
	}


	private static void ExecSide(TNod<INode> holder, TNod<INode> splitTarget, SDir sdir, TNod<INode> root)
	{
		var ptr = splitTarget.GetDadPtr(root);
		ptr.Kid = MkSplit(ptr.Type, ptr.Kid, holder, sdir);
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
