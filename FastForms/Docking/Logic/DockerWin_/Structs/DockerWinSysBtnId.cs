using FastForms.Docking.Enums;
using PowWin32.Windows;
using Vanara.PInvoke;

namespace FastForms.Docking.Logic.DockerWin_.Structs;

enum DockerWinSysBtnId
{
	Minimize,
	Maximize,
	Restore,
	Close,
}

static class DockerWinSysBtnBmps
{
	public static readonly Bitmap[] Bmps = [
		Resource.dockerwin_btn_minimize,
		Resource.dockerwin_btn_maximize,
		Resource.dockerwin_btn_restore,
		Resource.dockerwin_btn_close,
	];
}



static class DockerWinSysBtnUtils
{
	public static DockerWinSysBtnId[] GetBtns(TreeType treeType, bool isHolderFrame, bool isMaximized) =>
		isHolderFrame switch
		{
			true => [],
			false => (treeType, isMaximized) switch
			{
				(TreeType.Empty or TreeType.Tool, false) => [DockerWinSysBtnId.Maximize, DockerWinSysBtnId.Close],
				(TreeType.Empty or TreeType.Tool, true) => [DockerWinSysBtnId.Restore, DockerWinSysBtnId.Close],
				(TreeType.Doc or TreeType.Mixed, false) => [DockerWinSysBtnId.Minimize, DockerWinSysBtnId.Maximize, DockerWinSysBtnId.Close],
				(TreeType.Doc or TreeType.Mixed, true) => [DockerWinSysBtnId.Minimize, DockerWinSysBtnId.Restore, DockerWinSysBtnId.Close],
				_ => throw new ArgumentException()
			},
		};

	/*public static DockerWinSysBtnId[] GetBtns(TreeType type, bool isMaximized) =>
		(type, isMaximized) switch
		{
			(TreeType.ToolSingle, _) => [],
			(TreeType.Tool, false) => [DockerWinSysBtnId.Maximize, DockerWinSysBtnId.Close],
			(TreeType.Tool, true) => [DockerWinSysBtnId.Restore, DockerWinSysBtnId.Close],
			(TreeType.Doc or TreeType.Mixed, false) => [DockerWinSysBtnId.Minimize, DockerWinSysBtnId.Maximize, DockerWinSysBtnId.Close],
			(TreeType.Doc or TreeType.Mixed, true) => [DockerWinSysBtnId.Minimize, DockerWinSysBtnId.Restore, DockerWinSysBtnId.Close],
			_ => throw new ArgumentException()
		};*/

	public static void Execute(this DockerWinSysBtnId btn, SysWin sys)
	{
		switch (btn)
		{
			case DockerWinSysBtnId.Minimize:
				User32.ShowWindow(sys.Handle, ShowWindowCommand.SW_MINIMIZE);
				break;

			case DockerWinSysBtnId.Maximize:
				User32.ShowWindow(sys.Handle, ShowWindowCommand.SW_MAXIMIZE);
				break;

			case DockerWinSysBtnId.Restore:
				User32.ShowWindow(sys.Handle, ShowWindowCommand.SW_NORMAL);
				break;

			case DockerWinSysBtnId.Close:
				sys.Destroy();
				//sys.Dispose();
				break;

			default:
				throw new ArgumentException();
		}
	}
}