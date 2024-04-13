using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;
using PowWin32.Geom;

namespace FastForms.Docking.Structs;


/*
    Merge(H)
   ========
       ..x ━━━ H           ->      ..x ━━━ (H ++ x⁺)

    Init(Type)
    ==========
        Rᵀ ━━━ ∅            ->      Rᵀ ━━━ (x⁺)             Init(Tool)

        Rᵀ ━━━ ∅            ->      Rᵀ ━━━ Rᴰ ━━━ (x⁺)      Init(Doc)
       ..x ━━━ Rᴰ ━━━ ∅     ->     ..x ━━━ Rᴰ ━━━ (x⁺)      Init(Doc)

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

sealed record MergeTarget(HolderNode Holder, int? InsertIndex = null) : ITarget
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
