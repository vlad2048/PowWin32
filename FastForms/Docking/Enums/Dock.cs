using PowWin32.Geom;

namespace FastForms.Docking.Enums;


public sealed record Dock
{
	public SDir? SDir { get; }
	public Pane? Pane { get; }
	internal Dock(SDir? sdir, Pane? pane)
	{
		SDir = sdir;
		Pane = pane;
	}

	public static readonly Dock Up = Make(PowWin32.Geom.SDir.Up);
	public static readonly Dock Down = Make(PowWin32.Geom.SDir.Down);
	public static readonly Dock Left = Make(PowWin32.Geom.SDir.Left);
	public static readonly Dock Right = Make(PowWin32.Geom.SDir.Right);

	public static Dock Make(SDir? sdir = null) => new(sdir, null);
	public static Dock Make(Pane pane, SDir? sdir = null) => new(sdir, pane);
}



/*
static class DockPosExt
{
	public static (Dir, bool) GetSplitDirIsSecond(this DockRelType pos) => (
		pos switch
		{
			DockRelType.Left or DockRelType.Right => Dir.Horz,
			DockRelType.Top or DockRelType.Bottom => Dir.Vert,
			_ => throw new ArgumentException()
		},
		pos switch
		{
			DockRelType.Left or DockRelType.Top => false,
			DockRelType.Right or DockRelType.Bottom => true,
			_ => throw new ArgumentException()
		}
	);
}
*/