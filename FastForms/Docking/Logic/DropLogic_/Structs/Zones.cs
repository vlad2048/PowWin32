using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;
using PowWin32.Geom;
using System.Windows.Forms.VisualStyles;

namespace FastForms.Docking.Logic.DropLogic_.Structs;


sealed record RSet(R[] Rs)
{
	public static implicit operator RSet(R r) => new([r]);
}
sealed record TabGeom(SDir Dir, int Pos, int Lng);
sealed record Geom(R R, TabGeom? Tab)
{
	public static Geom ForNode(R r) => new(r, null);
	public static Geom ForNodeSide(R r, SDir dir) => new(r.GetSubR(dir), null);
}


interface IDropBmp;
sealed record NoneDropBmp : IDropBmp;
sealed record MergeDropBmp : IDropBmp;
sealed record ToolSplitDropBmp(SDir Dir) : IDropBmp;
sealed record DocSplitDropBmp(SDir Dir) : IDropBmp;

enum ZoneBmp
{
	None,
	Single,
	Small,
	Big,
}


/*
    Merge(H)
   ========
       ..x ━━━ H           ->      ..x ━━━ (H ++ x⁺)

    Init
    ====
        Rᵀ ━━━ ∅            ->      Rᵀ ━━━ (x⁺)             Init(Tool)

        ..x ━━━ Rᴰ ━━━ ∅    ->      ..x ━━━ Rᴰ ━━━ (x⁺)     Init(Doc)
    
    Split(H)
    ========
                                                ┏━ H
        ..x ━━━ H           ->      ..x ━━━ s' ━┫
                                                ┗━ (x⁺)

    SplitRoot
    =========
                                               ┏━ x..
        Rᵀ ━━━ x..          ->      Rᵀ ━━━ s' ━┫            SplitRoot(Tool)
                                               ┗━ (x⁺)

                                                ┏━ Rᴰ..
        ..x ━━━ Rᴰ..        ->      ..x ━━━ s' ━┫           SplitRoot(Doc)
                                                ┗━ (x⁺)

    SplitCreateDocRoot(H)
    =====================
                                                ┏━ H
        ..x ━━━ H           ->      ..x ━━━ s' ━┫
                                                ┗━ Rᴰ ━━━ (x⁺)
*/




interface ITarget
{
	NodeType DstType { get; }
}

interface ISplitTarget : ITarget
{
	SDir Dir { get; }
}

sealed record MergeTarget(HolderNode Holder) : ITarget
{
	public NodeType DstType => Holder.Type;
}

sealed record InitTarget(NodeType Type) : ITarget
{
	public NodeType DstType => Type;
}

sealed record SplitTarget(HolderNode Holder, SDir Dir) : ISplitTarget
{
	public NodeType DstType => Holder.Type;
}

sealed record SplitRootTarget(NodeType Type, SDir Dir) : ISplitTarget
{
	public NodeType DstType => NodeType.Tool;
}

sealed record SplitCreateDocRootTarget(ToolHolderNode Holder, SDir Dir) : ISplitTarget
{
	public NodeType DstType => NodeType.Doc;
}



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


sealed record Zone(
	Docker Docker,
	string Id,
	RSet ZoneR,
	R BmpR,
	ZoneBmp Bmp,
	Drop[] Drops
)
{
	public override string ToString() => Id;
}



file static class GeomFileExt
{
	public static R GetSubR(this R r, SDir dir) =>
		dir switch
		{
			SDir.Up => r - Marg.MkDown(r.Height / 2),
			SDir.Down => r - Marg.MkUp(r.Height / 2),
			SDir.Left => r - Marg.MkRight(r.Width / 2),
			SDir.Right => r - Marg.MkLeft(r.Width / 2),
			_ => throw new ArgumentException()
		};
}
