using System.Diagnostics;
using System.Runtime.CompilerServices;
using Vanara.PInvoke;

namespace PowWin32.Diag;

public static class ErrorExt
{
	private const bool BreakOnError = true;

	// **********************************************
	// * Log error if return value is 0, false, ... *
	// **********************************************
	public static ushort Check(
		this ushort v,
		[CallerArgumentExpression(nameof(v))] string functionName = "n/a",
		[CallerMemberName] string memberName = "n/a",
		[CallerFilePath] string filePath = "n/a",
		[CallerLineNumber] int lineNumber = 0
	)
	{
		LogIf(v == 0, functionName, memberName, filePath, lineNumber);
		return v;
	}

	public static bool Check(
		this bool v,
		[CallerArgumentExpression(nameof(v))] string functionName = "n/a",
		[CallerMemberName] string memberName = "n/a",
		[CallerFilePath] string filePath = "n/a",
		[CallerLineNumber] int lineNumber = 0
	)
	{
		LogIf(!v, functionName, memberName, filePath, lineNumber);
		return v;
	}

	public static nint Check(
		this nint v,
		[CallerArgumentExpression(nameof(v))] string functionName = "n/a",
		[CallerMemberName] string memberName = "n/a",
		[CallerFilePath] string filePath = "n/a",
		[CallerLineNumber] int lineNumber = 0
	)
	{
		LogIf(v == 0, functionName, memberName, filePath, lineNumber);
		return v;
	}

	public static T Check<T>(
		this T v,
		[CallerArgumentExpression(nameof(v))] string functionName = "n/a",
		[CallerMemberName] string memberName = "n/a",
		[CallerFilePath] string filePath = "n/a",
		[CallerLineNumber] int lineNumber = 0
	) where T : IHandle
	{
		LogIf(v.DangerousGetHandle() == 0, functionName, memberName, filePath, lineNumber);
		return v;
	}

	/*
	public static T Check<T>(
		this T v,
		[CallerArgumentExpression(nameof(v))] string functionName = "n/a",
		[CallerMemberName] string memberName = "n/a",
		[CallerFilePath] string filePath = "n/a",
		[CallerLineNumber] int lineNumber = 0
	) where T : SafeHANDLE
	{
		LogIf(v.IsInvalid, functionName, memberName, filePath, lineNumber);
		return v;
	}
	*/

	// *******************************************
	// * Log error if HRESULT indicates an error *
	// *******************************************
	public static HRESULT Check(
		this HRESULT v,
		[CallerArgumentExpression(nameof(v))] string functionName = "n/a",
		[CallerMemberName] string memberName = "n/a",
		[CallerFilePath] string filePath = "n/a",
		[CallerLineNumber] int lineNumber = 0
	)
	{
		LogHResultIfError(v, functionName, memberName, filePath, lineNumber);
		return v;
	}


	// ************************************************
	// * Log error is GetLastError indicates an error *
	// ************************************************
	public static void CheckLastErrorIf(
		bool condition,
		[CallerMemberName] string memberName = "n/a",
		[CallerFilePath] string filePath = "n/a",
		[CallerLineNumber] int lineNumber = 0
	)
	{
		if (!condition) return;
		var err = Kernel32.GetLastError();
		if (err.Failed)
		{
			Log(err, "n/a", memberName, filePath, lineNumber);

			if (BreakOnError) Debugger.Break();
		}
	}




	private static void LogHResultIfError(
		HRESULT hr,
		string functionName,
		string memberName,
		string filePath,
		int lineNumber
	)
	{
		if (hr.Succeeded) return;

		var delim = new string('=', 64);
		L();
		L(delim);
		L();
		LTitle($"COM error: '{hr}':");
		L();
		LInfo("HResult.Code", hr.Code);
		LInfo("HResult.Facility", $"{hr.Facility}");
		LInfo("HResult.Severity", $"{hr.Severity}");
		L();
		LInfo("Function", functionName);
		LInfo("File", filePath);
		LInfo("  Line", lineNumber);
		LInfo("  Member", memberName);
		L();
		L(delim);
		L();

		if (BreakOnError) Debugger.Break();
	}



	private static void LogIf(
		bool condition,
		string functionName,
		string memberName,
		string filePath,
		int lineNumber
	)
	{
		if (!condition) return;
		var err = Kernel32.GetLastError();
		Log(err, functionName, memberName, filePath, lineNumber);

		if (BreakOnError) Debugger.Break();
	}


	private static void Log(
		Win32Error err,
		string functionName,
		string memberName,
		string filePath,
		int lineNumber
	)
	{
		var ex = err.GetException();
		var delim = new string('=', 64);
		L();
		L(delim);
		L();
		LTitle($"Win32 error: '{ex.Message}':");
		L();
		LInfo("ErrorCode", (uint)err);
		LInfoHex("HResult", ex.HResult);
		L();
		LInfo("Function", functionName);
		LInfo("File", filePath);
		LInfo("  Line", lineNumber);
		LInfo("  Member", memberName);
		L();
		if (ex.StackTrace != null)
		{
			L(ex.StackTrace);
			L();
		}
		L(delim);
		L();
	}




	private const int KeyPad = 9;
	private static void LTitle(string s)
	{
		L($"  {s}");
		L($"  {new string('-', s.Length)}");
	}
	private static void LInfo(string key, string val) => L($"    {key,-KeyPad}: {val}");
	private static void LInfo(string key, uint val) => L($"    {key,-KeyPad}: {val}");
	private static void LInfo(string key, int val) => L($"    {key,-KeyPad}: {val}");
	private static void LInfoHex(string key, int val) => L($"    {key,-KeyPad}: {val:X8}");
	private static void L() => L("");
	private static void L(string s) => Console.Error.WriteLine(s);
}