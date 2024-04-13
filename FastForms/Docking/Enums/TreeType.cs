using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;

namespace FastForms.Docking.Enums;


public enum TreeType
{
	/// <summary>
	/// Not the main window AND the root has a single ToolHolderNode
	/// </summary>
	ToolSingle,

	/// <summary>
	/// There is no DocRoot. Includes the case of an empty root
	/// </summary>
	Tool,

	/// <summary>
	/// There is a DocRoot and no ToolHolderNodes
	/// </summary>
	Doc,

	/// <summary>
	/// There is both a DocRoot and at least one ToolHolderNode
	/// </summary>
	Mixed,
}



static class TreeTypeExt
{
	public static TreeType ComputeTreeType(this TNod<INode> root, bool isMainWindow) =>
		(!isMainWindow && root.Kids is [{ V: HolderNode }]) switch
		{
			true => TreeType.ToolSingle,
			false => root.All(e => e.V is not DocRootNode) switch
			{
				true => TreeType.Tool,
				false => root.Any(e => e.V is HolderNode {Type: NodeType.Tool}) switch
				{
					true => TreeType.Mixed,
					false => TreeType.Doc,
				},
			},
		};


	public static int GetMargin(this TreeType type) => type is TreeType.Doc or TreeType.Mixed ? DockingConsts.MarginDoc : DockingConsts.MarginTool;
}
