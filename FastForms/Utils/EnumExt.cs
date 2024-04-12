using PowBasics.CollectionsExt;

namespace FastForms.Utils;

static class EnumExt
{
	public static int ToInt<E>(this E value) where E : struct, Enum => (int)(object)value;
	public static E ToEnum<E>(this int value) where E : struct, Enum => (E)(object)value;

	public static int? IndexOfOrNull<T>(this IEnumerable<T> source, Func<T, bool> predicate)
	{
		var idx = source.IndexOf(predicate);
		return idx == -1 ? null : idx;
	}
}