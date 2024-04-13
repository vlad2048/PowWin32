using FastForms.Docking.Enums;
using Vanara.PInvoke;

namespace FastForms.Docking.Logic.HolderWin_.Structs;

enum HolderBtnId
{
    Menu,
    Maximize,
    Restore,
    AutohideOn,
    AutohideOff,
    Close,
}

static class HolderBtnBmps
{
    public static readonly Bitmap[] Bmps = [
        Resource.holderframe_btn_menu,
        Resource.holderframe_btn_maximize,
        Resource.holderframe_btn_restore,
        Resource.holderframe_btn_autohideon,
        Resource.holderframe_btn_autohideoff,
        Resource.holderframe_btn_close,
    ];
}


static class HolderBtnUtils
{
	public static HolderBtnId[] GetBtns(TreeType treeType, bool isHolderFrame, bool isMaximized, IAutohide autohide) =>
		(isHolderFrame, isMaximized) switch
		{
			(true, false) => [HolderBtnId.Menu, HolderBtnId.Maximize, HolderBtnId.Close],
			(true, true) => [HolderBtnId.Menu, HolderBtnId.Restore, HolderBtnId.Close],

			(false, _) => (treeType, autohide) switch
			{
				(TreeType.Empty or TreeType.Tool, _) => [HolderBtnId.Menu, HolderBtnId.Close],
				(TreeType.Doc or TreeType.Mixed, AutohideOff) => [HolderBtnId.Menu, HolderBtnId.AutohideOn, HolderBtnId.Close],
				(TreeType.Doc or TreeType.Mixed, AutohideOn) => [HolderBtnId.Menu, HolderBtnId.AutohideOff, HolderBtnId.Close],
				_ => throw new ArgumentException("Invalid combination"),
			},
		};

	/*public static HolderBtnId[] GetBtns(TreeType treeType, bool isMaximized, IAutohide autohide) =>
		(treeType, isMaximized, autohide) switch
		{
			(TreeType.ToolSingle, false, _) => [HolderBtnId.Menu, HolderBtnId.Maximize, HolderBtnId.Close],
			(TreeType.ToolSingle, true, _) => [HolderBtnId.Menu, HolderBtnId.Restore, HolderBtnId.Close],

			(TreeType.Tool, _, _) => [HolderBtnId.Menu, HolderBtnId.Close],

			(TreeType.Doc or TreeType.Mixed, _, AutohideOff) => [HolderBtnId.Menu, HolderBtnId.AutohideOn, HolderBtnId.Close],
			(TreeType.Doc or TreeType.Mixed, _, AutohideOn) => [HolderBtnId.Menu, HolderBtnId.AutohideOff, HolderBtnId.Close],

			_ => throw new ArgumentException("Invalid combination")
		};*/



	public static void Execute(this HolderBtnId btn, Docker docker)
	{
		switch (btn)
		{
			case HolderBtnId.Menu:
	            break;

			case HolderBtnId.Maximize:
				User32.ShowWindow(docker.Sys.Handle, ShowWindowCommand.SW_MAXIMIZE);
				break;

			case HolderBtnId.Restore:
				User32.ShowWindow(docker.Sys.Handle, ShowWindowCommand.SW_NORMAL);
				break;

			case HolderBtnId.AutohideOn:
				break;

			case HolderBtnId.AutohideOff:
				break;

			case HolderBtnId.Close when docker.IsHolderFrame.V:
				docker.Sys.Destroy();
				break;
		}
	}
}