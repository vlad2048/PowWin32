using PowBasics.CollectionsExt;
using PowWin32.Geom;
using PowBasics.StringsExt;

// ReSharper disable once CheckNamespace
namespace PowTrees.Algorithms;

public static class Algo_Logging
{
    public static string[] Log<T>(this TNod<T> root, Action<TreeLogOpt<T>>? optFun = null)
    {
        var opt = TreeLogOpt<T>.Make(optFun);

        var layout = root.Layout(
            e => opt.FmtFun(e).GetSize(),
            layoutOpt =>
            {
                layoutOpt.GutterSz = opt.GutterSz;
                layoutOpt.AlignLevels = opt.AlignLevels;
            });
        var treeSz = layout.Values.Union().Size;
        var buffer = Enumerable.Range(0, treeSz.Height).SelectToArray(_ => new string(' ', treeSz.Width));

        void Print(R r, string s)
        {
            var sLines = s.SplitInLines();
            for (var i = 0; i < sLines.Length; i++)
            {
                var sLine = sLines[i];
                var line = buffer[r.Y + i];
                var n = sLine.Length;
                buffer[r.Y + i] = line[..r.X] + sLine + line[(r.X + n)..];
            }
        }

        foreach (var (n, r) in layout)
            Print(r, opt.FmtFun(n.V));

		ArrowUtils.DrawArrows(root, layout, (pos, str) => Print(new R(pos, new Sz(str.Length, 1)), str));

        return buffer;
    }



    private static Sz GetSize(this string str)
    {
        var lines = str.SplitInLines();
        if (lines.Length == 0) return Sz.Empty;
        return new Sz(
            lines.Max(e => e.Length),
            lines.Length
        );
    }
}












/*
using PowWin32.Geom;
using PowTrees.Algorithms.Layout.Exts;

namespace PowTrees.Algorithms;

public enum TreeLogType
{
	Inline,
	Traversal
}

public sealed class TreeLogOpt<T>
{
	public TreeLogType Type { get; set; } = TreeLogType.Inline;
	public Func<T, string>? FormatFun { get; set; }
	public int TraversalIndentPerLevel { get; set; } = 2;
	public int LeadingSpaceCount { get; set; } = 0;

	internal static TreeLogOpt<T> Build(Action<TreeLogOpt<T>>? action)
	{
		var opt = new TreeLogOpt<T>();
		action?.Invoke(opt);
		return opt;
	}
}


public static class Algo_Logging
{
	public static string LogToString<T>(this TNod<T> root, Action<TreeLogOpt<T>>? optFun = null) => string.Join(Environment.NewLine, root.LogToStrings(optFun));
	public static string[] LogToStrings<T>(this TNod<T> root, Action<TreeLogOpt<T>>? optFun = null)
	{
		var opt = TreeLogOpt<T>.Build(optFun);
		static string FmtDefault(T v) => $"{v}";
		var strTree = root.Map(opt.FormatFun ?? FmtDefault);
		var lines = opt.Type switch
		{
			TreeLogType.Inline => strTree.LogInline(),
			TreeLogType.Traversal => strTree.LogTraversal(opt),
			_ => throw new ArgumentException()
		};
		var leadingStr = new string(' ', opt.LeadingSpaceCount);
		return lines.SelectToArray(e => $"{leadingStr}{e}");
	}



	private static string[] LogTraversal<T>(this TNod<string> root, TreeLogOpt<T> opt)
	{
		var lines = new List<string>();
		void Recurse(TNod<string> node, int level)
		{
			var padStr = new string(' ', level * opt.TraversalIndentPerLevel);
			lines.Add($"{padStr}{node.V}");
			foreach (var child in node.Kids)
				Recurse(child, level + 1);
		}
		Recurse(root, 0);
		return lines.ToArray();
	}



	private static string[] LogInline(this TNod<string> root)
	{
		var layout = root.Layout(e => new Sz(e.Length, 1), opt =>
		{
			opt.GutterSz = new Sz(3, 0);
			opt.AlignLevels = true;
		});
		var treeSz = layout.Values.Union().Size;
		var buffer = Enumerable.Range(0, treeSz.Height)
			.SelectToArray(_ => new string(' ', treeSz.Width));

		void Print(Pt pos, string s)
		{
			var line = buffer[pos.Y];
			var n = s.Length;
			buffer[pos.Y] = line[..pos.X] + s + line[(pos.X + n)..];
		}

		foreach (var (n, r) in layout)
			Print(r.Pos, n.V);

		DrawArrows(layout.GetRTree(), Print);

		return buffer;
	}


	private static void DrawArrows(TNod<R> root, Action<Pt, string> print)
	{
		//┌─┬─┐    ╭───╮
		//│ │ │    │   │
		//├─┼─┤    ╰───╯
		//└─┴─┘    

		var chHoriz = "─";
		var chVert = "│";
		var chCornerTop = "┌";
		var chCornerBottom = "└";
		var chCross = "┼";
		var chTUp = "┴";
		var chTDown = "┬";
		var chTLeft = "┤";
		var chTRight = "├";
		var chArrow = "►";

		root
			.Where(e => e.Kids.Count == 1).ForEach(n =>
			{
				var d = n.Kids[0].V.X - (n.V.X + n.V.Width);
				print(n.V.OnTheRight(), $"{new string(chHoriz[0], d - 1)}{chArrow}");
			});

		root
			.Where(e => e.Kids.Count > 1)
			.ForEach(n =>
			{
				var rp = n.V;
				var rcs = n.Kids.SelectToArray(e => e.V);
				var xMid = (rp.X + rp.Width + rcs[0].X - 1) / 2;
				var yMid = rp.YMid();
				var yMin = rcs.First().YMid();
				var yMax = rcs.Last().YMid();

				print(n.V.OnTheRight(), new string(chHoriz[0], xMid - (rp.X + rp.Width)));
				
				for (var i = yMin; i <= yMax; i++)
					print(new Pt(xMid, i), chVert);

				print(new Pt(xMid, rp.YMid()), chTLeft);

				for (var i = 0; i < rcs.Length; i++)
				{
					var r = rcs[i];
					var y = r.YMid();

					var ch = (i == 0, y == yMid, i == rcs.Length - 1) switch
					{
						(true, true, true) => chHoriz,
						(true, true, false) => chTDown,
						(true, false, false) => chCornerTop,
						(false, true, false) => chCross,
						(false, false, true) => chCornerBottom,
						(false, true, true) => chTUp,
						(false, false, false) => chTRight,
						_ => "V"
					};
					print(new Pt(xMid, y), $"{ch}{new string(chHoriz[0], r.X - xMid - 2)}{chArrow}");
				}
			});
	}

	private static Pt OnTheRight(this R r) => new(r.Right, r.YMid());
	private static int YMid(this R r) => r.Y + r.Height / 2;

	private static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (var elt in source)
			action(elt);
	}


	private static TNod<R> GetRTree<T>(this Dictionary<TNod<T>, R> layout) =>
		layout
			.Keys.Single(e => e.Parent == null)
			.MapN(e => layout[e]);
}
*/
