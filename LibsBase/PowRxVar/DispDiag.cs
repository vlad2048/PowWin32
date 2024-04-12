using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reactive.Disposables;

namespace PowRxVar;

public sealed record DispStats(
	int Created,
	int Disposed
);

public static class DispDiag
{
	// **********
	// * Public *
	// **********
	public const string? BreakpointOnDispName = null;
	public static bool DisposeInReverseOrder { get; set; } = false;
	public static bool DispMakerLoggingEnabled { get; set; }

	public static DispStats GetStats() => new(countCreated, countDisposed);

	public static bool CheckForUndisposedDisps(bool waitForKey = true)
	{
		var isIssue = LogAndTellIfThereAreUndisposedDisps_Inner();
		if (isIssue && waitForKey)
			Console.ReadKey();
		return isIssue;
	}

	public static void ResetDispsForTests()
	{
		indentLevel = 0;
		map.Clear();
		countMap.Clear();
		countCreated = 0;
		countDisposed = 0;
	}


	// ************
	// * Internal *
	// ************
	internal static string NotifyNewDisp(Disp d, string nameBase, string srcFile, int srcLine)
	{
		countCreated++;
		var name = GetFullName(nameBase);
		if (name == BreakpointOnDispName)
			Debugger.Break();
		Print(name, true, indentLevel++);
		map[d] = new DispNfo(d, name, srcFile, srcLine);
		Disposable.Create(() =>
		{
			countDisposed++;
			Print(name, false, --indentLevel);
			map[d] = map[d].FlagDispose();
		}).D(d);
		return name;
	}


	// ***********
	// * Private *
	// ***********
	private const int IndentSize = 2;
	private const int ColName = 0x8537b0;
	private const int ColNew = 0x3fe861;
	private const int ColDispose = 0xeb3f76;

	private static int indentLevel;
	private static readonly ConcurrentDictionary<Disp, DispNfo> map = new();
	private static readonly Dictionary<string, int> countMap = new();
	private static int countCreated;
	private static int countDisposed;



	private static void Print(string name, bool isNew, int indent)
	{
		if (!DispMakerLoggingEnabled) return;
		var indentStr = new string(' ', indent * IndentSize);
		Console.Write(indentStr);
		Console.Write(name, ColName);
		var (txt, col) = isNew ? (".new()", ColNew) : (".Dispose()", ColDispose);
		Console.WriteLine(txt, col);
	}





	private static bool LogAndTellIfThereAreUndisposedDisps_Inner()
	{
		var allDisps = map.Values.Where(e => !e.Disposed).ToArray();

		void LogCounts()
		{
			LStr("");
			LStr($"  # Disps created : {countCreated}");
			LStr($"  # Disps disposed: {countDisposed}");
			LStr("");
		}
		if (allDisps.Length == 0)
		{
			LStr("");
			LTitle("All Disps released");
			LogCounts();
			return false;
		}
		else
		{
			var topDisps = allDisps.RemoveSubs();
			LTitle($"{topDisps.Length} unreleased top level Disps (total: {allDisps.Length})");
			var pad = topDisps.Max(e => e.Name.Length);
			foreach (var d in topDisps)
				LStr($"    {d.Fmt(pad)}");
			LogCounts();
			return true;
		}
	}



	private sealed record DispNfo(Disp Disp, string Name, string File, int Line)
	{
		public bool Disposed { get; private init; }

		public string Fmt(int pad) => $"{Name.PadRight(pad)} @ {Path.GetFileName(File)}:{Line}";
		public override string ToString() => $"{Name} @ {Path.GetFileName(File)}:{Line}";

		public DispNfo FlagDispose() => Disposed switch
		{
			true => throw new ArgumentException("Already disposed"),
			false => this with { Disposed = true }
		};
	}



	private static void LStr(string s) => Console.WriteLine(s);
	private static void LTitle(string s)
	{
		LStr(s);
		LStr(new string('=', s.Length));
	}

	private static DispNfo[] RemoveSubs(this DispNfo[] ds) =>
		ds
			.Where(d => ds.Where(e => e != d).All(e => !e.Disp.Contains(d.Disp)))
			.ToArray();


	private static string GetFullName(string name)
	{
		if (!countMap.TryAdd(name, 0))
			countMap[name]++;
		var cnt = countMap[name];
		return cnt switch
		{
			0 => name,
			_ => $"{name}[{cnt}]"
		};
	}
}