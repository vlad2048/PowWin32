using PowTrees.Algorithms;

namespace PowTrees.Utils;

public static class TreeUtils
{
	public static IReadOnlyDictionary<TNod<T>, TNod<U>> BuildNodeLookup<T, U>(TNod<T> rootSrc, TNod<U> rootDst) =>
		rootSrc.ZipTreeN(rootDst)
			.ToDictionary(e => e.V.Item1, e => e.V.Item2);
}