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
		var r = splitNode.R;
		var splitPosPrev = splitNode.Pos;

		var min = treeMarg + DockingConsts.MinHolderSize + treeMarg / 2;
		var max = r.Dir(splitNode.Dir) - (treeMarg + DockingConsts.MinHolderSize + treeMarg / 2);
		max = Math.Max(min, max);

		var splitPosNext = splitPosPrev.Clamp(min, max);

		return splitPosNext;
	}

	private static int Clamp(this int val, int min, int max)
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





/*
using FastForms.Docking.Logic.HolderWin_;
using FastForms.Docking.Logic.Layout_.Nodes;
using PowBasics.CollectionsExt;
using PowWin32.Geom;

namespace FastForms.Docking.Logic.Layout_;


sealed record LayoutChanges(
	IReadOnlyDictionary<INode, R> Rs,
	IReadOnlyDictionary<SplitNode, int> Splits
);



static class LayoutCalculator
{
	public static LayoutChanges Compute(INode root, R rootRNext)
	{
		var lay = new Lay(root);
		ComputeRs(lay, rootRNext);
		ZeroDegenerateRs(lay);
		return lay.Build();
	}
	


	public static void Apply(this LayoutChanges layout, INode root)
	{
		root
			.AllNodes()
			.Where(layout.Rs.ContainsKey)
			.ForEach(node => node.R = layout.Rs[node]);

		root
			.AllNodes()
			.OfType<SplitNode>()
			.Where(layout.Splits.ContainsKey)
			.ForEach(splitNode => splitNode.Pos = layout.Splits[splitNode]);
	}




	private sealed class Lay(INode root)
	{
		private readonly Dictionary<INode, R> rs = new();
		private readonly Dictionary<SplitNode, int> splits = new();

		public INode Root => root;
		public LayoutChanges Build() => new(rs, splits);

		public R GetR(INode node) => rs.ContainsKey(node) ? rs[node] : node.R;
		public int GetSplit(SplitNode splitNode) => splits.ContainsKey(splitNode) ? splits[splitNode] : splitNode.Pos;
		public R SetR(INode node, R r)
		{
			var prev = GetR(node);
			if (r == node.R)
				rs.Remove(node);
			else
				rs[node] = r;
			return prev;
		}
		// ReSharper disable once UnusedMethodReturnValue.Local
		public int SetSplit(SplitNode splitNode, int split)
		{
			var prev = GetSplit(splitNode);
			if (split == splitNode.Pos)
				splits.Remove(splitNode);
			else
				splits[splitNode] = split;
			return prev;
		}
	}





	private static void ComputeRs(Lay lay, R rootRNext)
	{
		void Rec(INode node, R rNext)
		{
			var deltaSz = rNext.Size - lay.SetR(node, rNext).Size;

			switch (node)
			{
				case RootNode { Kid: null }:
					break;
				case RootNode { Kid: not null } e:
				{
					var kidR = rNext - DockingConsts.Margin;
					Rec(e.Kid, kidR);
					break;
				}
				case SplitNode e:
				{
					// ReSharper disable once UnusedVariable
					var splitPosResize = ComputeResizeSplit(e, lay, deltaSz);
					var splitPos = AdjustSplitPosIFN(e, lay);
					var (r1, r2) = SplitR(rNext, e.Dir, splitPos);
					Rec(e.First, r1);
					Rec(e.Second, r2);
					break;
				}
				case HolderWin:
				{
					break;
				}
				default:
					throw new ArgumentException();
			}
		}

		Rec(lay.Root, rootRNext);
	}



	private static int ComputeResizeSplit(SplitNode splitNode, Lay lay, Sz deltaSz)
	{
		var splitPrev = lay.GetSplit(splitNode);
		var delta = deltaSz.Dir(splitNode.Dir);
		if (delta == 0) return splitPrev;

		var (w0, w1) = ComputeSplitWeight(splitNode);
		var splitNext = splitPrev + w0 * delta / (w0 + w1);
		lay.SetSplit(splitNode, splitNext);
		return splitNext;
	}


	private static (int, int) ComputeSplitWeight(SplitNode splitNode)
	{
		if (splitNode.AreAllSplitInTheSameDir())
		{
			var w0 = 1 + splitNode.First.AllNodes().OfType<SplitNode>().Count();
			var w1 = 1 + splitNode.Second.AllNodes().OfType<SplitNode>().Count();
			return (w0, w1);
		}
		else
		{
			return (1, 1);
		}
	}

	private static bool AreAllSplitInTheSameDir(this SplitNode splitNode) =>
		splitNode
			.AllNodes()
			.OfType<SplitNode>()
			.All(e => e.Dir == splitNode.Dir);




	private static int AdjustSplitPosIFN(SplitNode splitNode, Lay lay)
	{
		var r = lay.GetR(splitNode);
		var splitPosPrev = lay.GetSplit(splitNode);
		
		var min = DockingConsts.MarginValue + DockingConsts.MinHolderSize + DockingConsts.MarginValue / 2;
		var max = r.Dir(splitNode.Dir) - (DockingConsts.MarginValue + DockingConsts.MinHolderSize + DockingConsts.MarginValue / 2);
		max = Math.Max(min, max);

		var splitPosNext = splitPosPrev.Clamp(min, max);
		lay.SetSplit(splitNode, splitPosNext);

		return splitPosNext;
	}

	private static int Clamp(this int val, int min, int max)
	{
		if (max < min) throw new ArgumentException("max < min");
		if (val < min) return min;
		if (val > max) return max;
		return val;
	}






	private static void ZeroDegenerateRs(Lay lay) =>
		lay.Root
			.AllNodes()
			.Where(node => lay.GetR(node).IsDegenerate())
			.ForEach(node => lay.SetR(node, R.Empty));

	private static bool IsDegenerate(this R r) => r.Width <= 0 || r.Height <= 0;








	private static (R, R) SplitR(R r, Dir dir, int pos) => dir switch
	{
		Dir.Horz => (
			new R(r.X, r.Y, pos - DockingConsts.MarginValue / 2, r.Height),
			new R(r.X + pos + DockingConsts.MarginValue / 2, r.Y, r.Width - pos - DockingConsts.MarginValue / 2, r.Height)
		),
		Dir.Vert => (
			new R(r.X, r.Y, r.Width, pos - DockingConsts.MarginValue / 2),
			new R(r.X, r.Y + pos + DockingConsts.MarginValue / 2, r.Width, r.Height - pos - DockingConsts.MarginValue / 2)
		),
		_ => throw new ArgumentException()
	};
}
*/








/*
static class LayoutCalculator
{
    public static LayoutChanges Compute(INode root, R rootRNext, bool adjustSplits)
    {
	    var rootRPrev = root.R;
	    root.R = rootRNext;

	    var mapRs = ComputeRs(root);

	    var mapSplits = adjustSplits switch {
		    false => new Dictionary<SplitNode, int>(),
		    true => ComputeSplits(root, rootRPrev)
	    };

        return new LayoutChanges(mapRs, mapSplits);
    }


    private static IReadOnlyDictionary<SplitNode, int> ComputeSplits(INode root, R rootRPrev)
    {
	    var rootRNext = root.R;
	    var map = new Dictionary<SplitNode, int>();
	}


    private static IReadOnlyDictionary<INode, R> ComputeRs(INode root)
    {
	    var map = new Dictionary<INode, R>();
	    void Rec(INode node, R r)
	    {
		    map[node] = r;
		    switch (node)
		    {
			    case RootNode { Kid: null }:
				    break;
			    case RootNode { Kid: not null } e:
			    {
				    var kidR = r - DockingConsts.Margin;
				    Rec(e.Kid, kidR);
				    break;
			    }
			    case SplitNode e:
			    {
				    var (r1, r2) = SplitR(r, e.Dir, e.Pos);
				    Rec(e.First, r1);
				    Rec(e.Second, r2);
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

	    Rec(root, root.R);
	    return map;
    }





	public static void Apply(this LayoutChanges layout, INode root)
    {
	    root
		    .AllNodes()
		    .Where(layout.Rs.ContainsKey)
		    .ForEach(node => node.R = layout.Rs[node]);

	    root
		    .AllNodes()
		    .OfType<SplitNode>()
		    .Where(layout.Splits.ContainsKey)
		    .ForEach(splitNode => splitNode.Pos = layout.Splits[splitNode]);
    }







	private static (R, R) SplitR(R r, Dir dir, int pos) => dir switch
    {
        Dir.Horz => (
            new R(r.X, r.Y, pos - DockingConsts.MarginValue / 2, r.Height),
            new R(r.X + pos + DockingConsts.MarginValue / 2, r.Y, r.Width - pos - DockingConsts.MarginValue / 2, r.Height)
        ),
        Dir.Vert => (
            new R(r.X, r.Y, r.Width, pos - DockingConsts.MarginValue / 2),
            new R(r.X, r.Y + pos + DockingConsts.MarginValue / 2, r.Width, r.Height - pos - DockingConsts.MarginValue / 2)
        ),
        _ => throw new ArgumentException()
    };
}
*/