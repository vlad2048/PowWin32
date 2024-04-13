using FastForms.Docking.Enums;
using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;
using PowBasics.CollectionsExt;
using PowWin32.Geom;

namespace FastForms.Docking.Logic.Layout_;


static class LayoutCalculator
{
	public static void Compute(TNod<INode> root, TreeType treeType)
	{
		var treeMarg = treeType.GetMargin();

		Rec(root, root.V.R);

		root
			.Where(e => e.V.R.IsDegenerate())
			.ForEach(e => e.V.R = R.Empty);


		void Rec(TNod<INode> node, R nodeR)
		{
			var isInit = node.V.R == R.Empty;
			var deltaSz = nodeR.Size - node.V.R.Size;
			node.V.R = nodeR;

			switch (node.V)
			{
				case RootNode:
				{
					AssMsg(node.Kids.Count <= 1, "RootNode has the wrong number of kids");
					if (node.Kids.Count == 1)
					{
						var isRootHolder = node.V.Type == NodeType.Tool && node.Kids[0].V is HolderNode;
						var kidR = nodeR - (isRootHolder ? Marg.Empty : DockingConsts.MarginDoc.ToMarg());
						Rec(node.Kids[0], kidR);
					}

					break;
				}
				case SplitNode e:
				{
					AssMsg(node.Kids.Count == 2, "SplitNode has the wrong number of kids");

					var splitPosResize = isInit switch
					{
						true => nodeR.Dir(e.Dir) / 2,
						false => ComputeResizeSplit(node, deltaSz)
					};
					e.Pos = splitPosResize;

					var splitPos = AdjustSplitPos(e, treeMarg);
					e.Pos = splitPos;

					var (r1, r2) = SplitR(nodeR, e.Dir, e.Pos, treeMarg);
					Rec(node.Kids[0], r1);
					Rec(node.Kids[1], r2);
					break;
				}
				case HolderNode:
				{
					break;
				}
				default:
					throw new ArgumentException();
			}
		}
	}



	private static int ComputeResizeSplit(TNod<INode> splitNod, Sz deltaSz)
	{
		var splitNode = (SplitNode)splitNod.V;
		var delta = deltaSz.Dir(splitNode.Dir);
		if (delta == 0) return splitNode.Pos;

		var (w0, w1) = ComputeSplitWeight(splitNod);
		var splitNext = splitNode.Pos + w0 * delta / (w0 + w1);
		return splitNext;
	}


	private static (int, int) ComputeSplitWeight(TNod<INode> splitNod)
	{
		if (splitNod.AreAllSplitInTheSameDir())
		{
			var w0 = 1 + splitNod.Kids[0].Count(e => e.V is SplitNode);
			var w1 = 1 + splitNod.Kids[1].Count(e => e.V is SplitNode);
			return (w0, w1);
		}
		else
		{
			return (1, 1);
		}
	}

	private static bool AreAllSplitInTheSameDir(this TNod<INode> splitNod) =>
		splitNod
			.Where(e => e.V is SplitNode)
			.All(e => ((SplitNode)e.V).Dir == ((SplitNode)splitNod.V).Dir);




	private static int AdjustSplitPos(SplitNode splitNode, int treeMarg)
	{
		var (min, max) = splitNode.GetPosBounds(treeMarg);
		var splitPosNext = splitNode.Pos.Clamp(min, max);
		return splitPosNext;
	}

	public static (int, int) GetPosBounds(this SplitNode split, int mg)
	{
		var min = mg + DockingConsts.MinHolderSize + mg / 2;
		var max = split.R.Dir(split.Dir) - (mg + DockingConsts.MinHolderSize + mg / 2);
		max = Math.Max(min, max);
		return (min, max);
	}

	public static int Clamp(this int val, int min, int max)
	{
		if (max < min) throw new ArgumentException("max < min");
		if (val < min) return min;
		if (val > max) return max;
		return val;
	}



	



	private static (R, R) SplitR(R r, Dir dir, int pos, int treeMarg) => dir switch
	{
		Dir.Horz => (
			new R(
				r.X,
				r.Y,
				pos - treeMarg / 2,
				r.Height
			),
			new R(
				r.X + pos + treeMarg / 2,
				r.Y,
				r.Width - pos - treeMarg / 2,
				r.Height
			)
		),
		Dir.Vert => (
			new R(
				r.X,
				r.Y,
				r.Width,
				pos - treeMarg / 2
			),
			new R(
				r.X,
				r.Y + pos + treeMarg / 2,
				r.Width,
				r.Height - pos - treeMarg / 2
			)
		),
		_ => throw new ArgumentException()
	};


	private static Marg ToMarg(this int v) => new(v, v, v, v);
}
