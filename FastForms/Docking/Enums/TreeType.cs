namespace FastForms.Docking.Enums;


public enum TreeType
{
	/// <summary>
	/// Contains no Holders
	/// So the tree can be either:
	///   Rᵀ ━━━ ∅
	///   Rᵀ ━━━ Rᴰ ━━━ ∅
	/// </summary>
	Empty,

	/// <summary>
	/// Contains only ToolHolders
	/// </summary>
	Tool,

	/// <summary>
	/// Contains only DocHolders
	/// </summary>
	Doc,

	/// <summary>
	/// Contains both ToolHolders and DocHolders
	/// </summary>
	Mixed,
}


/*
public enum HolderFrameState
{
	None,

	ToolFrameNormal,

	ToolFrameMaximized,
}


static class HolderFrameStateExt
{
	public static bool IsToolFrame(this HolderFrameState state) => state is HolderFrameState.ToolFrameNormal or HolderFrameState.ToolFrameMaximized;
}
*/



/*
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
*/