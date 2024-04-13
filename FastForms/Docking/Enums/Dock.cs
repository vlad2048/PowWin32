using PowWin32.Geom;

namespace FastForms.Docking.Enums;


public sealed record Dock
{
	public SDir? Dir { get; }
	public Pane? Pane { get; }
	internal Dock(SDir? dir, Pane? pane)
	{
		Dir = dir;
		Pane = pane;
	}

	public static readonly Dock Empty = new(null, null);

	public static readonly Dock Up = Make(SDir.Up);
	public static readonly Dock Down = Make(SDir.Down);
	public static readonly Dock Left = Make(SDir.Left);
	public static readonly Dock Right = Make(SDir.Right);

	public static Dock Make(SDir? sdir = null) => new(sdir, null);
	public static Dock Make(Pane pane, SDir? sdir = null) => new(sdir, pane);
}
