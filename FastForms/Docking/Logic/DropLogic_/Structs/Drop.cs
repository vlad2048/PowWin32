using FastForms.Docking.Structs;
using PowWin32.Geom;

namespace FastForms.Docking.Logic.DropLogic_.Structs;


interface IDropBmp;
sealed record NoneDropBmp : IDropBmp;
sealed record MergeDropBmp : IDropBmp;
sealed record ToolSplitDropBmp(SDir Dir) : IDropBmp;
sealed record DocSplitDropBmp(SDir Dir) : IDropBmp;


sealed record Drop(
	Docker Docker,
	string Id,
	R R,
	IDropBmp Bmp,
	Geom Geom,
	ITarget Target
)
{
	public override string ToString() => Id;
}