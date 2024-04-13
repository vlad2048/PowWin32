using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Docking.Logic.Tree_;
using FastForms.Docking.Structs;
using PowBasics.CollectionsExt;
using PowTrees.Algorithms;
using PowWin32.Geom;
using PowWin32.Utils;

// ReSharper disable once CheckNamespace
namespace FastForms.Docking;

static class DockToTargetOp
{
	/*

	Attaches a tree Node to the docker tree at the target location.

	srcNod can only be:
		- HolderNode
		- SplitNode with no DocRootNode under it
	
	if target is MergeTarget { Holder }
	------------------------
		- gets all the panes in all the holders in srcNod (and empty the holders)
		- merge the panes into the target.Holder (this reparents them, see PaneManager.Setup)
		- dispose of the emptied holders

	otherwise
	---------
		- insert srcNod as is into the docker tree
		- notify the Docker of the change (to make sure the Holders and the Panes windows are reparented)

	*/
	public static void DockToTarget(this Docker docker, TNod<INode> srcNod, ITarget target)
	{
		Ass(srcNod.V is HolderNode || (srcNod.V is SplitNode && srcNod.All(e => e.V is not DocRootNode)));
		Ass(srcNod.OfTypeNod<INode, HolderNode>().Sum(e => e.State.Panes.Count) > 0);
		var srcHolders = srcNod.OfTypeNod<INode, HolderNode>().ToArray();

		if (target is MergeTarget { Holder: var dstHolder })
		{
			var srcPanes = srcHolders.SelectMany(e => e.State.Panes.Arr.V).ToArray();
			dstHolder.State.AddPanes(srcPanes);

			// Without this delay we're getting an ObjectDisposedException in a ILiteSubject
			Obs.Timer(TimeSpan.Zero)
				.ObserveOnUIThread()
				.Subscribe(_ =>
				{
					srcHolders.ForEach(holder => holder.D.Dispose());
				});
		}
		else
		{
			var root = docker.Root;

			switch (target)
			{
				case InitTarget { Type: NodeType.Tool }:
					Ass(root.Kids.Count == 0);
					root.Kids.Add(srcNod);
					break;
				case InitTarget { Type: NodeType.Doc }:
					if (root.Kids.Count == 0)
					{
						root.Kids.Add(N.RootDoc(srcNod));
					}
					else
					{
						var docRoot = root.Find<DocRootNode>();
						Ass(docRoot.Kids.Count == 0);
						docRoot.Kids.Add(srcNod);
					}
					break;

				case ISplitTarget { Dir: var dir } splitTarget:
					srcNod = target switch
					{
						SplitCreateDocRootTarget => N.RootDoc(srcNod),
						_ => srcNod,
					};
					var splitNod = splitTarget switch
					{
						SplitTarget { Holder: var holder } => root.Find(holder),
						SplitRootTarget { Type: NodeType.Tool } => root.Kids.Single(),
						SplitRootTarget { Type: NodeType.Doc } => root.Find<DocRootNode>(),
						SplitCreateDocRootTarget { Holder: var holder } => root.Find(holder),
						_ => throw new ArgumentException(),
					};
					var dad = splitNod.GetDadPtr(root);
					dad.Kid = MkSplit(target.DstType, dad.Kid, srcNod, dir);
					break;

				default:
					throw new ArgumentException();
			}

			docker.TriggerTreeMod(new AddHoldersTreeMod(srcHolders));
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