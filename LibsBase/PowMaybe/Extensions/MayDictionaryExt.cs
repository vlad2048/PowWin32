// ReSharper disable once CheckNamespace
namespace PowMaybe;

public static class MayDictionaryExt
{
	public static Maybe<V> GetOrMaybe<K, V>(this IReadOnlyDictionary<K, V> dict, K key) where K : notnull => dict.TryGetValue(key, out var val) switch
	{
		true => May.Some(val!),
		false => May.None<V>()
	};
}