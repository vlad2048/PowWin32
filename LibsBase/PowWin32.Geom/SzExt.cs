namespace PowWin32.Geom;

public static class SzExt
{
	public static int Dir(this Sz sz, Dir dir) => dir switch
	{
		Geom.Dir.Horz => sz.Width,
		Geom.Dir.Vert => sz.Height,
		_ => throw new ArgumentException()
	};
}