using FastForms.Utils.GdiUtils;
using PowWin32.Geom;
using Style = FastForms.Docking.Logic.HolderWin_.Painting.HolderWinPainterStyle;

namespace FastForms.Docking.Logic.HolderWin_.Painting;

public readonly record struct TabLabelLay(R R)
{
	public const int TabLabelHorzPad = 6;
	private static readonly Marg TextMarg = new(2, TabLabelHorzPad, 4, TabLabelHorzPad);
	private static readonly Marg GripMarg = new(2, TabLabelHorzPad - 1, 2, TabLabelHorzPad);

	public R TextR => R - TextMarg;
	public R GripR => R - GripMarg;
}

static class HolderLayout
{
    public const int CaptionHeight = 21;

    private static readonly Marg AdjMargRoot = new(1, 0, 0, 0);
    private static readonly Marg AdjMargNotRoot = new(1, 1, 1, 1);

    private static readonly Marg PaneMargSingleTab = new(CaptionHeight, 0, 0, 0);
    private static readonly Marg PaneMargMultipleTabs = new(CaptionHeight, 1, CaptionHeight, 1);


    public static readonly Marg WinBorderMarg = new(0, 1, 1, 1);

	public static R AdjustClientR(R r, bool isRoot) => r - (isRoot ? AdjMargRoot : AdjMargNotRoot);

	public static R GetPaneR(R r, int tabCount, TabLabelLay? jerkLay) => r - (tabCount > 1 || jerkLay != null ? PaneMargMultipleTabs : PaneMargSingleTab);


	public static R GetTabsVertBorders(R r) => (r.Height > 2 * CaptionHeight) switch {
		true => new R(r.X, r.Y + CaptionHeight, r.Width, r.Height - 2 * CaptionHeight),
		false => R.Empty
	};

	public static R GetTabsRowR(R r) => new(r.X, GetTabsRowY(r), r.Width, CaptionHeight);


    private const int MinTabLabelWidth = 10 + 2 * TabLabelLay.TabLabelHorzPad;

    public static TabLabelLay[] GetTabLabelRs(R r, string[] tabNames)
    {
        int[] xs = [.. tabNames.Select(e => Style.Font.MeasureText(e).Width + 2 * TabLabelLay.TabLabelHorzPad)];
        var space = Math.Max(0, r.Width);
        var labelSizes = TabShrinker.Shrink(space, MinTabLabelWidth, xs);
        var arr = new R[tabNames.Length];
        var x = r.X;
        var y = GetTabsRowY(r);
        for (var i = 0; i < labelSizes.Length; i++)
        {
            var labelSize = labelSizes[i];
            arr[i] = new R(x, y, labelSize, CaptionHeight);
            x += labelSize;
        }
        return [..arr.Select(e => new TabLabelLay(e))];
    }


    private static int GetTabsRowY(R r) => Math.Max(r.Y + CaptionHeight, r.Bottom - CaptionHeight);
}




file static class TabShrinker
{
    public static int[] Shrink(int space, int xMin, int[] xs)
    {
        var left = xs.Sum() - space;
        if (left <= 0) return xs;
        var steps = xs.ComputeSteps();
        var subs = new int[xs.Length];
        foreach (var step in steps)
        {
            var val = Math.Min(left, step.Total);
            var (valDiv, valMod) = (val / step.Cnt, val % step.Cnt);
            for (var i = 0; i < step.Indices.Length; i++)
                subs[step.Indices[i]] += valDiv + (i < valMod ? 1 : 0);
            left -= val;
            if (left < 0) throw new ArgumentException();
            if (left == 0) break;
        }
        return Subtract(xs, subs).CapMin(xMin);
	}

    private sealed record Step(int Delta, int[] Indices)
    {
        public static readonly Step Zero = new(0, []);
        public int Cnt => Indices.Length;
        public int Total => Delta * Cnt;
    }

    private static Step[] ComputeSteps(this int[] xs)
    {
        Step[] steps =
        [..xs
            .OrderDescending()
            .Distinct()
            .Select(e => new Step(e, xs.GetIndices(e)))
            .Append(Step.Zero)
        ];
        return
        [..steps.Zip(steps.Skip(1))
            .Select(t => t.First with { Delta = t.First.Delta - t.Second.Delta })
            .MergeIndices()
        ];
    }

    private static Step[] MergeIndices(this IEnumerable<Step> steps)
    {
        var list = new List<Step>();
        var cur = new List<int>();
        foreach (var step in steps)
        {
            cur.AddRange(step.Indices);
            list.Add(new Step(step.Delta, [.. cur]));
        }
        return list.ToArray();
    }

    private static int[] Subtract(int[] xs, int[] ys)
    {
        if (xs.Length != ys.Length) throw new ArgumentException();
        return [.. xs.Zip(ys).Select(e => e.First - e.Second)];
    }

    private static int[] CapMin(this int[] xs, int min) => [.. xs.Select(e => Math.Max(e, min))];

	private static int[] GetIndices(this int[] xs, int x) => [.. xs.Select((e, i) => (e, i)).Where(t => t.e == x).Select(t => t.i)];
}
