using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;
using PowTrees.Algorithms;

namespace FastForms.Docking.Logic.Tree_;

sealed record DadPtr(
	TNod<INode> Node,
	int KidIdx
)
{
	public NodeType Type => Node.V.Type;
	public TNod<INode> Kid
	{
		get => Node.Kids[KidIdx];
		set => Node.Kids[KidIdx] = value;
	}

	public TNod<INode> OtherKid
	{
		get => Node.Kids[OtherIdx];
		set => Node.Kids[OtherIdx] = value;
	}

	private int OtherIdx
	{
		get
		{
			if (Node.V is not SplitNode) throw new ArgumentException("Can only get the OtherKid for a SplitNode");
			return KidIdx switch
			{
				0 => 1,
				1 => 0,
				_ => throw new ArgumentException()
			};
		}
	}
}

static class DadPtrLogic
{
	public static DadPtr GetDadPtr(this TNod<INode> targetNode, TNod<INode> root)
	{
		var dad = targetNode.FindDad(root);
		var kidIdx = dad.Kids.IndexOf(targetNode);
		if (kidIdx == -1) throw new ArgumentException("Failed to find the kid");
		return new DadPtr(dad, kidIdx);
	}
}