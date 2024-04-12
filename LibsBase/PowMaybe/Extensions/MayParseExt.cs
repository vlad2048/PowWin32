// ReSharper disable once CheckNamespace
namespace PowMaybe;

public static class MayParseExt
{
	public static Maybe<byte> TryParseByteMaybe(this string s) => byte.TryParse(s, out var v) switch
	{
		true => May.Some(v),
		false => May.None<byte>()
	};

	public static Maybe<int> TryParseIntMaybe(this string s) => int.TryParse(s, out var v) switch
	{
		true => May.Some(v),
		false => May.None<int>()
	};

	public static Maybe<double> TryParseDoubleMaybe(this string s) => double.TryParse(s, out var v) switch
	{
		true => May.Some(v),
		false => May.None<double>()
	};
}