using System.Diagnostics.CodeAnalysis;
using FastForms.Docking.Logic.Layout_.Nodes;
using PowTrees.Algorithms;

namespace FastForms.Docking.Logic.Tree_;

static class TreeExt
{
	public static TNod<INode> Find(this TNod<INode> root, INode node) => root.First(e => e.V == node);

	public static TNod<INode> Find<T>(this TNod<INode> root) where T : INode => root.First(e => e.V is T);

	public static bool TryFind<T>(this TNod<INode> root, [NotNullWhen(true)] out TNod<INode>? result) where T : INode
	{
		result = root.FirstOrDefault(e => e.V is T);
		return result != null;
	}

	public static HolderNode? GetHolderContainingPane(this TNod<INode> root, Pane? pane) => pane switch
	{
		null => null,
		not null => root.Select(e => e.V).OfType<HolderNode>().FirstOrDefault(e => e.State.Panes.Arr.V.Contains(pane))
	};
}