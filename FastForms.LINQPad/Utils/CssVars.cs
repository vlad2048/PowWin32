using System.Runtime.CompilerServices;
using LINQPad;

namespace FastForms.LINQPad.Utils;

public static class CssVars
{
	// Public
	// ======
	public static string v(this int val, [CallerArgumentExpression(nameof(val))] string? valExpr = null) => Get($"#{val:x6}", valExpr);
	public static string v(this string val, [CallerArgumentExpression(nameof(val))] string? valExpr = null) => Get(val, valExpr);

	// Internal
	// ========
	internal static void Reset()
	{
		varMap.Clear();
		nonVarSet.Clear();
	}

	// Private
	// =======
	private static readonly Dictionary<string, string> varMap = new();
	private static readonly HashSet<string> nonVarSet = new();
	private static string Get(string val, [CallerArgumentExpression(nameof(val))] string? valExpr = null)
	{
		var varName = GetValName(valExpr);
		if (nonVarSet.Contains(varName)) return val;

		if (varMap.TryGetValue(varName, out var existingVal))
		{
			if (val != existingVal)
			{
				varMap.Remove(varName);
				nonVarSet.Add(varName);
				return val;
			}
		}
		else
		{
			SetJS(varName, val);
			varMap[varName] = val;
		}
		return $"var(--{varName})";
	}
	private static void SetJS(string name, string val) =>
		Util.HtmlHead.AddStyles($$"""
		                          	:root {
		                          		--{{name}}: {{val}};
		                          	}
		                          """);
	private static string GetValName(string? expr)
	{
		if (string.IsNullOrEmpty(expr)) throw new ArgumentException();
		return expr
			.Replace("\"", "")
			.Replace(" ", "")
			.Replace(".", "");
	}
}