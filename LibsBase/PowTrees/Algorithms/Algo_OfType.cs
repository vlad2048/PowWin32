namespace PowTrees.Algorithms;

public static class Algo_OfType
{
	public static IEnumerable<U> OfTypeNod<T, U>(this TNod<T> root) where U : T =>
		root
			.Select(e => e.V)
			.OfType<U>();

	public static U? FirstOfTypeOrDefault<T, U>(this TNod<T> root) where U : T =>
		root
			.Select(e => e.V)
			.OfType<U>()
			.FirstOrDefault();
}