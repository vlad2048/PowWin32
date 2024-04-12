namespace PowWin32.Geom;

public readonly record struct Marg(int Up, int Right, int Bottom, int Left)
{
	public static Marg All(int v) => new(v, v, v, v);
	public static Marg MkUp(int v) => new(v, 0, 0, 0);
	public static Marg MkRight(int v) => new(0, v, 0, 0);
	public static Marg MkDown(int v) => new(0, 0, v, 0);
	public static Marg MkLeft(int v) => new(0, 0, 0, v);

	//public Marg MgUp(int top) => this with { Top = top };
	//public Marg MgRight(int right) => this with { Right = right };
	//public Marg MgDown(int bottom) => this with { Bottom = bottom };
	//public Marg MgLeft(int left) => this with { Left = left };

	public override string ToString() => (this == default) switch
	{
		true => string.Empty,
		false when Up == Right && Up == Bottom && Up == Left => $"mg({Up})",
		_ => $"mg({Up},{Right},{Bottom},{Left})"
	};

	public static readonly Marg Empty = default;

	//public Pt TopLeft => new(Left, Top);

	// Conversion operators
	// ====================
	//public static implicit operator Marg(NetCoreEx.Geometry.Margins e) => new(e.Top, e.Right, e.Bottom, e.Left);
	//public static implicit operator NetCoreEx.Geometry.Margins(Marg e) => new(e.Top, e.Right, e.Bottom, e.Left);
}


public static class MargUtils
{
	public static int Dir(this Marg m, Dir dir) => dir switch
	{
		Geom.Dir.Horz => m.Left + m.Right,
		Geom.Dir.Vert => m.Up + m.Bottom,
		_ => throw new ArgumentException()
	};

	public static Marg Mirror(this Marg m) => new(m.Left, m.Bottom, m.Right, m.Up);
}
