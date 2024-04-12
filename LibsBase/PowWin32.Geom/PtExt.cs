namespace PowWin32.Geom;

public static class PtExt
{
	public static int Dir(this Pt pt, Dir dir) => dir switch
	{
		Geom.Dir.Horz => pt.X,
		Geom.Dir.Vert => pt.Y,
		_ => throw new ArgumentException()
	};
}
