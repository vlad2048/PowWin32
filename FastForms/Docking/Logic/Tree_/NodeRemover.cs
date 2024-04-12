using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Docking.Structs;

namespace FastForms.Docking.Logic.Tree_;

static class NodeRemover
{
	/*
	━ ┃ ┫ ┏ ┗

	Remove(@)
	---------
	        ┏━ @
	x ━━ s ━┫      --->     x ━━ y
	        ┗━ y

	*/

	public static TreeMod Remove(TNod<INode> root, TNod<INode> target)
	{
		var dad = target.GetDadPtr(root);
		if (dad.Node.V is not SplitNode) throw new ArgumentException("Can only remove a node from a SplitNode");

		var grandDad = dad.Node.GetDadPtr(root);
		grandDad.Kid = dad.OtherKid;

		return TreeMod.Make(grandDad.Node);
	}
}