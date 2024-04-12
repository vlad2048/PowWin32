namespace PowTrees.Algorithms;

public static class Algo_IsEqual
{
	public static bool IsEqual<T>(this TNod<T> rootA, TNod<T> rootB)
	{
		bool Recurse(TNod<T> nodeA, TNod<T> nodeB)
		{
			if (!Equals(nodeA.V, nodeB.V) || nodeA.Kids.Count != nodeB.Kids.Count)
				return false;
			foreach (var t in nodeA.Kids.Zip(nodeB.Kids))
			{
				if (!Recurse(t.First, t.Second))
					return false;
			}
			return true;
		}

		return Recurse(rootA, rootB);
	}
}