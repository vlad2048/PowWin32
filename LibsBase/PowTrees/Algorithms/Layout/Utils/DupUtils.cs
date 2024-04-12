// ReSharper disable once CheckNamespace
namespace PowTrees.Algorithms;

sealed record Dup<T>(T Orig);

static class DupUtils
{
	public static TNod<Dup<T>> Dup<T>(this TNod<T> root) => root.Map(e => new Dup<T>(e));

	public static TNod<R> ZipMapWithTree<T, V, R>(this Dictionary<TNod<T>, V> map, TNod<T> root, Func<T, V, R> combineFun) =>
		root
			.MapN(nod => combineFun(nod.V, map[nod]));
}