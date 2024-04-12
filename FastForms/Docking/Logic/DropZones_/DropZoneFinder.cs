/*
using FastForms.Docking.Logic.DropZones_.Structs;
using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;

namespace FastForms.Docking.Logic.DropZones_;

static class DropZoneFinder
{
	public static IIcons[] FindDropZones(this TNod<INode> root, NodeType srcType)
	{
		var nodes = root.ToArray();
		var hasDocRoot = nodes.Any(e => e.V is DocRootNode);
		return nodes
			.SelectMany(node => FindFor(node, srcType, hasDocRoot))
			.ToArray();
	}

	private static IIcons[] FindFor(TNod<INode> node, NodeType srcType, bool hasDocRoot) =>
		srcType switch
		{
			NodeType.Tool => node.V switch
			{
				DocRootNode => [Tool2DocRootIcons.Instance],
				ToolRootNode when hasDocRoot => Tool2ToolRootIcons.Instances,
				ToolHolderNode e => [new Tool2ToolHolderIcons(e)],
				DocHolderNode e => [new Tool2DocHolderIcons(e)],
				_ => [],
			},

			NodeType.Doc => node.V switch
			{
				DocRootNode => [Doc2DocRootIcons.Instance],
				ToolRootNode => [],
				ToolHolderNode e when !hasDocRoot => [new Doc2ToolHolderIcons(e)],
				DocHolderNode e => [new Doc2DocHolderIcons(e)],
				_ => [],
			},

			_ => throw new ArgumentException(),
		};
}
*/