using System.Runtime.CompilerServices;

namespace PowWin32.Diag;

public static class Asserts
{
	public static void Ass(
		bool condition,
		[CallerArgumentExpression(nameof(condition))] string? conditionStr = null,
		[CallerMemberName] string? memberName = null,
		[CallerFilePath] string? filePath = null,
		[CallerLineNumber] int lineNumber = 0
	)
	{
		if (!condition)
			throw new ArgumentException($"Assertion failed: {conditionStr} @ {FmtCallerAttrs(memberName, filePath, lineNumber)}");
	}

	public static void AssMsg(bool condition, string message)
	{
		if (!condition)
			throw new ArgumentException(message);
	}

	public static T Ass<T>(this T obj, Func<T, bool> pred, [CallerArgumentExpression(nameof(pred))] string? predStr = null) => pred(obj) switch
	{
		true => obj,
		false => throw new ArgumentException($"Assertion on {typeof(T).Name} failed: {predStr} => FALSE"),
	};


	private static string FmtCallerAttrs(string? memberName, string? filePath, int lineNumber) => $"{memberName}:{lineNumber} ({filePath})";
}