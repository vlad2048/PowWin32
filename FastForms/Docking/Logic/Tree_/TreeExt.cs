using FastForms.Docking.Logic.Layout_.Nodes;

namespace FastForms.Docking.Logic.Tree_;

static class TreeExt
{
	public static TNod<INode> Find(this TNod<INode> root, INode node) => root.First(e => e.V == node);

	public static HolderNode? GetHolderContainingPane(this TNod<INode> root, Pane? pane) => pane switch
	{
		null => null,
		not null => root.Select(e => e.V).OfType<HolderNode>().FirstOrDefault(e => e.State.Panes.Arr.V.Contains(pane))
	};
}