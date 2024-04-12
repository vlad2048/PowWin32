/*
using FastForms.Docking.Enums;
using FastForms.Docking.Logic.HolderNode_;
using FastForms.Docking.Logic.OtherNodes_;
using FastForms.Docking.Structs;
using PowWin32.Windows;

namespace FastForms.Docking.Utils;

static class TreeUtils
{
	public static (HolderNode, INode) GetOrCreateHolderNode(RootNode root, SysWin parentWin, DockRelType pos, HolderNode? parent)
	{
		if (root.IsEmpty())
		{
			var nodeHolder = new HolderNode(parentWin);
			root.Kid = nodeHolder;
			return (nodeHolder, root);
		}

		// After handling the empty root edge case above, the following is true:
		//		- root.Kid != null

		switch (pos)
		{
			case DockRelType.Overlap:
				var overlapNodeHolder = parent ?? root.AllNodes().OfType<HolderNode>().First();
				return (overlapNodeHolder, overlapNodeHolder);

			case DockRelType.Left:
			case DockRelType.Right:
			case DockRelType.Top:
			case DockRelType.Bottom:
			{
				var ptr = NodePtr.Make(root, parent);
				var r = ptr.Kid.R;
				var (splitDir, isSecond) = pos.GetSplitDirIsSecond();

				var nodeHolder = new HolderNode(parentWin);

				var (first, second) = (nodeHolder, ptr.Kid).SwapIf(isSecond);
				var split = new SplitNode(splitDir, first, second, r);
				ptr.ReplaceKid(split);
				return (
					nodeHolder,
					ptr.Parent
				);
			}

			default:
				throw new ArgumentException();
		}
	}

	
	
	private static (T, T) SwapIf<T>(this (T, T) t, bool condition) => condition switch
	{
		false => t,
		true => (t.Item2, t.Item1)
	};

}
*/