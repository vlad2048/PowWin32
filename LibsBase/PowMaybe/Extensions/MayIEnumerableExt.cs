// ReSharper disable once CheckNamespace
namespace PowMaybe;

public static class MayIEnumerableExt
{
	public static IEnumerable<T> WhereSome<T>(this IEnumerable<Maybe<T>> source) =>
		source.Where(e => e.IsSome()).Select(e => e.Ensure());

	public static Maybe<T> FirstOrMaybe<T>(this IEnumerable<T> source, Func<T, bool>? predicate = null)
	{
		foreach (var elt in source)
			if (predicate == null || predicate(elt))
				return May.Some(elt);
		return May.None<T>();
	}

	public static Maybe<T> LastOrMaybe<T>(this IEnumerable<T> source, Func<T, bool>? predicate = null)
	{
		foreach (var elt in source.Reverse())
			if (predicate == null || predicate(elt))
				return May.Some(elt);
		return May.None<T>();
	}

	public static Maybe<int> IndexOfMaybe<T>(this IEnumerable<T> source, Func<T, bool> predicate)
	{
		var idx = 0;
		foreach (var elt in source)
		{
			if (predicate(elt))
				return May.Some(idx);
			idx++;
		}
		return May.None<int>();
	}
}