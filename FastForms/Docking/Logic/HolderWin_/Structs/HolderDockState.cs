using FastForms.Docking.Enums;
using PowWin32.Geom;

namespace FastForms.Docking.Logic.HolderWin_.Structs;


public interface IAutohide;
sealed record AutohideOff : IAutohide;
sealed record AutohideOn(SDir Dir, bool Show) : IAutohide;


/*
public sealed record HolderDockState(
	bool IsMaximized,
	IAutohide Autohide
)
{
	public static readonly HolderDockState Empty = new(false, new AutohideOff());
}


static class HolderDockStateExt
{
	public static HolderBtnId[] GetBtns(this HolderDockState s, TreeType treeType) =>
		(type: treeType, s.IsMaximized, s.Autohide) switch
		{
			(TreeType.ToolSingle, false, _) => [HolderBtnId.Menu, HolderBtnId.Maximize, HolderBtnId.Close],
			(TreeType.ToolSingle, true, _) => [HolderBtnId.Menu, HolderBtnId.Restore, HolderBtnId.Close],

			(TreeType.Tool, _, _) => [HolderBtnId.Menu, HolderBtnId.Close],

			(TreeType.Doc, _, AutohideOff) => [HolderBtnId.Menu, HolderBtnId.AutohideOn, HolderBtnId.Close],
			(TreeType.Doc, _, AutohideOn) => [HolderBtnId.Menu, HolderBtnId.AutohideOff, HolderBtnId.Close],

			_ => throw new ArgumentException($"Invalid HolderDockState:{s}  TreeType:{treeType}")
		};
}
*/
