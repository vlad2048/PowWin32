using FastForms.Docking.Logic.Layout_.Enums;
using PowWin32.Geom;

namespace FastForms.Docking.Logic.Layout_.Nodes;

public abstract class RootNode(NodeType type) : INode
{
    public NodeType Type { get; } = type;
    public R R { get; set; }

    public override string ToString() => $"[{Type}]-Root  r:{R}";
}

public sealed class ToolRootNode() : RootNode(NodeType.Tool);
public sealed class DocRootNode() : RootNode(NodeType.Doc);
