using FastForms.Docking.Logic.Layout_;
using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Docking.Logic.Tree_;
using PowBasics.CollectionsExt;
using PowTrees.Algorithms;

namespace FastForms.Docking.Structs;

interface ITreeMod;

sealed record InitTreeMod : ITreeMod;

sealed record PaintNodeTreeMod(INode Node) : ITreeMod;

sealed record AddHoldersTreeMod(HolderNode[] Holders) : ITreeMod;

sealed record RecomputeLayoutTreeMod : ITreeMod;

sealed record SplitResizeTreeMod : ITreeMod;



static class TreeModApplier
{
	public static void Apply(this ITreeMod mod, Docker docker)
	{
		var (root, sys, treeType) = (docker.Root, docker.Sys, docker.TreeType.V);

		switch (mod)
		{
			case InitTreeMod:
				root.V.R = sys.ClientR;
				LayoutCalculator.Compute(root, treeType);
				root.OfTypeNod<INode, HolderNode>().ForEach(holder => holder.State.Attach(docker));
				LayoutApplier.Apply(root);
				break;

			case PaintNodeTreeMod { Node: var node }:
				root.Find(node).OfTypeNod<INode, HolderNode>().ForEach(holder => holder.State.Repaint());
				break;

			case AddHoldersTreeMod { Holders: var holders }:
				root.V.R = sys.ClientR;
				LayoutCalculator.Compute(root, treeType);
				holders.ForEach(holder => holder.State.Attach(docker));
				LayoutApplier.Apply(root);
				break;

			case RecomputeLayoutTreeMod:
				root.V.R = sys.ClientR;
				LayoutCalculator.Compute(root, treeType);
				LayoutApplier.Apply(root);
				break;

			case SplitResizeTreeMod:
				root.V.R = sys.ClientR;
				LayoutCalculator.Compute(root, treeType);
				LayoutApplier.ApplyRedraw(root, sys);
				break;

			default:
				throw new ArgumentException();
		}
	}
}