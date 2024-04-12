namespace PowWin32.Geom;

public static class RExt
{
	public static int Dir(this R r, Dir dir) => dir switch
	{
		Geom.Dir.Horz => r.Width,
		Geom.Dir.Vert => r.Height,
		_ => throw new ArgumentException()
	};

	public static R ToR(this Sz sz) => new(Pt.Empty, sz);

	public static bool IsDegenerate(this R r) => r.Width <= 0 || r.Height <= 0;

	public static R SetPos(this R r, Pt p) => r with { X = p.X, Y = p.Y };

	public static bool Contains(this R r, Pt pt) => pt.X >= r.X && pt.X < r.X + r.Width && pt.Y >= r.Y && pt.Y < r.Y + r.Height;

	public static bool Contains(this R a, R b) =>
		b.X >= a.X &&
		b.Y >= a.Y &&
		(b.X + b.Width) <= (a.X + a.Width) &&
		(b.Y + b.Height) <= (a.Y + a.Height);

	public static R Union(this IEnumerable<R> listE)
	{
		var list = listE.Where(e => e != R.Empty).ToArray();
		if (list.Length == 0) return R.Empty;
		var minX = list.Min(e => e.X);
		var minY = list.Min(e => e.Y);
		var maxX = list.Max(e => e.X + e.Width);
		var maxY = list.Max(e => e.Y + e.Height);
		return new R(minX, minY, maxX - minX, maxY - minY);
	}

	public static R Intersection(this R a, R b)
	{
		var x = Math.Max(a.X, b.X);
		var num1 = Math.Min(a.X + a.Width, b.X + b.Width);
		var y = Math.Max(a.Y, b.Y);
		var num2 = Math.Min(a.Y + a.Height, b.Y + b.Height);
		/*
            In WinDX (for Pop nodes), it's important for to have the intersection of a rectangle with
            zero size to be a rectangle with zero size at the correct location.

            This is why we have:
            num1 >= x && num2 >= y
            and not:
            num1 > x && num2 > y
        */
		return num1 >= x && num2 >= y ? new R(x, y, num1 - x, num2 - y) : R.Empty;
	}

	public static R Intersection(this IEnumerable<R> source)
	{
		var arr = source.ToArray();
		if (arr.Length == 0) return R.Empty;
		var curR = arr[0];
		for (var i = 1; i < arr.Length; i++)
			curR = curR.Intersection(arr[i]);
		return curR;
	}

	public static R CapToMin(this R r, int minWidth, int minHeight) => r with { Width = Math.Max(r.Width, minWidth), Height = Math.Max(r.Height, minHeight) };
	public static R WithZeroPos(this R r) => new(Pt.Empty, r.Size);
	public static R SzDec(this R r) => r with { Width = Math.Max(0, r.Width - 1), Height = Math.Max(0, r.Height - 1) };
	public static R WithSize(this R r, Sz sz) => new(r.Pos, sz);

	public static R Enlarge(this R r, int v)
	{
		if (v < 0) throw new ArgumentException();
		return r.EnlargeShrinkSigned(v);
	}

	public static R Shrink(this R r, int v)
	{
		if (v < 0) throw new ArgumentException();
		return r.EnlargeShrinkSigned(-v);
	}

	private static R EnlargeShrinkSigned(this R r, int v)
	{
		if (v >= 0)
		{
			return new R(r.X - v, r.Y - v, r.Width + v * 2, r.Height + v * 2);
		}
		else
		{
			v = -v;
			var left = r.X + v;
			var top = r.Y + v;
			var right = r.Right - v;
			var bottom = r.Bottom - v;
			if (left > right)
				left = right = (left + right) / 2;
			if (top > bottom)
				top = bottom = (top + bottom) / 2;
			return new R(left, top, right - left, bottom - top);
		}
	}
}
