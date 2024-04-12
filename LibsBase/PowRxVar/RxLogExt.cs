using System.Runtime.CompilerServices;

namespace PowRxVar;

public static class RxLogExt
{
	public static void Log<T>(this IObservable<T> source, [CallerArgumentExpression(nameof(source))] string? sourceStr = null) =>
		source.Subscribe(e => L($"[{sourceStr}] <- {e}"));


	private static void L(string s) => Console.WriteLine(s);
}