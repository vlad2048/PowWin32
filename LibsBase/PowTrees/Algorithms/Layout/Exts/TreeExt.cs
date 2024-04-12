using PowBasics.CollectionsExt;

namespace PowTrees.Algorithms.Layout.Exts;

static class TreeExt
{
	public static TNod<T>[][] GetNodesByLevels<T>(this TNod<T> root)
	{
		var lists = new List<List<TNod<T>>>();
		void AddToLevel(TNod<T> node, int level)
		{
			List<TNod<T>> list;
			if (level < lists.Count)
				list = lists[level];
			else if (level == lists.Count)
				lists.Add(list = new List<TNod<T>>());
			else
				throw new ArgumentException();
			list.Add(node);
		}
		root.ForEachWithLevel(AddToLevel);
		return lists.SelectToArray(e => e.ToArray());
	}

	private static void ForEachWithLevel<T>(this TNod<T> root, Action<TNod<T>, int> action)
	{
		void Recurse(TNod<T> node, int level)
		{
			action(node, level);
			foreach (var child in node.Kids)
				Recurse(child, level + 1);
		}
		Recurse(root, 0);
	}
}