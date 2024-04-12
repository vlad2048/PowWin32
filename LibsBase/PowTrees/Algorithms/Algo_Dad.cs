namespace PowTrees.Algorithms;

public static class Algo_Dad
{
	public static TNod<T> FindDad<T>(this TNod<T> node, TNod<T> root) => root.First(e => e.Kids.Contains(node));
}