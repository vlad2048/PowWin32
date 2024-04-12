using FastForms.Docking.Logic.Layout_.Enums;
using PowWin32.Geom;

namespace FastForms.Docking.Logic.Layout_.Nodes;

public sealed class SplitNode(NodeType type, Dir dir) : INode
{
	public NodeType Type { get; } = type;
	public R R { get; set; } //= r;
	public Dir Dir { get; } = dir;
	public int Pos { get; internal set; } //= r.Dir(dir) / 2;

	public override string ToString() => $"Split({Dir}@{Pos})  r:{R}";
}