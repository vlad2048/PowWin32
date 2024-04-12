using System.Runtime.CompilerServices;

namespace PowWin32.Diag;

public static class Asserts
{
	public static void Ass(bool condition, string message)
	{
		if (!condition)
			throw new ArgumentException(message);
	}

	public static T Ass<T>(this T obj, Func<T, bool> pred, [CallerArgumentExpression(nameof(pred))] string? predStr = null) => pred(obj) switch
	{
		true => obj,
		false => throw new ArgumentException($"Assertion on {typeof(T).Name} failed: {predStr} => FALSE"),
	};
}