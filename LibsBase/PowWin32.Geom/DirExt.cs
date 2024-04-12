namespace PowWin32.Geom;

public static class DirExt
{
	public static Dir Neg(this Dir dir) => dir switch
	{
		Dir.Horz => Dir.Vert,
		Dir.Vert => Dir.Horz,
		_ => throw new ArgumentException()
	};
}