using System.Diagnostics.CodeAnalysis;

namespace PowMaybe;

public static class May
{
	// ************
	// * Creation *
	// ************
	public static Maybe<T> Some<T>(T v) => new Maybe<T>.Some(v);
	public static Maybe<T> None<T>() => new Maybe<T>.None();

	public static Maybe<T> ToMaybe<T>(this T? v) where T : class => v switch
	{
		null => None<T>(),
		not null => Some(v)
	};


	// **************
	// * Unwrapping *
	// **************
	public static bool IsSome<T>(this Maybe<T> may) => may.IsSome(out _);

	public static bool IsSome<T>(this Maybe<T> may, [NotNullWhen(true)] out T? val)
	{
		switch (may)
		{
			case Maybe<T>.Some { V: var valV }:
				val = valV!;
				return true;

			case Maybe<T>.None:
				val = default;
				return false;

			default:
				throw new ArgumentException();
		}
	}

	public static bool IsNone<T>(this Maybe<T> may) => may.IsNone(out _);

	public static bool IsNone<T>(this Maybe<T> may, [NotNullWhen(false)] out T? val)
	{
		switch (may)
		{
			case Maybe<T>.Some { V: var valV }:
				val = valV!;
				return false;

			case Maybe<T>.None:
				val = default;
				return true;

			default:
				throw new ArgumentException();
		}
	}

	public static T Ensure<T>(this Maybe<T> may) => may.IsSome(out var val) switch
	{
		true => val!,
		false => throw new ArgumentException()
	};

	public static T FailWith<T>(this Maybe<T> may, T def) => may.IsSome(out var val) ? val : def;

	public static T[] ToArray<T>(this Maybe<T> may) => may.IsSome(out var val) switch
	{
		true => new[] { val },
		false => Array.Empty<T>()
	};

	/// <summary>
	/// Converts a Maybe&lt;T&gt; to:
	/// <list type="bullet">
	/// <item><term>A nullable reference</term><description>if T is a reference type</description></item>
	/// <item><term>A nullable value</term><description>if T is a nullable value type</description></item>
	/// <item><term>default(T)</term><description>if T is a non nullable value type</description></item>
	/// </list>
	/// </summary>
	/// <typeparam name="T">Type</typeparam>
	/// <param name="v">Value to convert</param>
	/// <returns>Conversion to nullable</returns>
	public static T? ToNullable<T>(this Maybe<T> v) => v.IsSome(out var val) switch
	{
		true => val,
		false => default
	};


	// ***********
	// * Testing *
	// ***********
	public static bool IsSomeAndEqualTo<T>(this Maybe<T> may, T elt) => may.IsSome(out var val) switch
	{
		true => val.Equals(elt),
		false => false
	};
}