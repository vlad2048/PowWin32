using FastForms.Docking.Logic.DropZones_.Structs;
using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;
using PowTrees.Algorithms;
using PowWin32.Geom;

namespace FastForms.Docking.Logic.Tree_;

sealed record DockNod(
	SDir? SDir,
	HolderNode? Holder
)
{
	public static readonly DockNod Empty = new(null, null);
}

static class DockInterpreter
{
	public static IDrop Interpret(this DockNod dock, NodeType srcType, TNod<INode> root)
	{
		if (root.Kids.Count == 0)
			return new ToolRoot_Init_Drop();

		var defaultToolHolder = root.FirstOfTypeOrDefault<INode, ToolHolderNode>();
		var defaultDocHolder = root.FirstOfTypeOrDefault<INode, DocHolderNode>();
		var docRoot = root.FirstOrDefault(e => e.V is DocRootNode);
		var hasDocRoot = docRoot != null;

		return (srcType, dock.Holder, dock.SDir) switch
		{
			// TODO: sort Holder.NodeType vs Pane.NodeType
			(NodeType.Tool, { } holder, null) => new Holder_Over_Drop(holder),
			(NodeType.Tool, { } holder, not null) => new Holder_Side_Drop(holder, srcType, dock.SDir.Value),
			(NodeType.Tool, null, null) => defaultToolHolder switch
			{
				not null => new Holder_Over_Drop(defaultToolHolder),
				null => new ToolRoot_Side_Drop(SDir.Left),
			},
			(NodeType.Tool, null, not null) => new ToolRoot_Side_Drop(dock.SDir.Value),

			(NodeType.Doc, DocHolderNode docHolder, null) => new Holder_Over_Drop(docHolder),
			(NodeType.Doc, DocHolderNode docHolder, not null) => new Holder_Side_Drop(docHolder, srcType, dock.SDir.Value),
			(NodeType.Doc, null, _) => hasDocRoot switch
			{
				false => new Holder_Side_CreateDocRoot_Drop(defaultToolHolder ?? throw new ArgumentException("Should not be null"), dock.SDir ?? SDir.Right),
				true => defaultDocHolder switch
				{
					null => new DocRoot_Init_Drop(),
					not null => new Holder_Over_Drop(defaultDocHolder),
				}
			},

			_ => throw new ArgumentException("I didn't see that one comming"),
		};
	}
}