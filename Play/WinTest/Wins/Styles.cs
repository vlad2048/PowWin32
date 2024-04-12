using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.Structs;
using Vanara.PInvoke;

namespace WinTest.Wins;

static class Styles
{
	public static readonly WinClass AppWin_Class = new(
		"AppWin",
		styles: User32.WindowClassStyles.CS_HREDRAW | User32.WindowClassStyles.CS_VREDRAW,
		hBrush: HBRUSH.NULL
	);
	public static readonly WinStylesDef AppWin_Styles = new(
		User32.WindowStyles.WS_VISIBLE | User32.WindowStyles.WS_CLIPSIBLINGS | //User32.WindowStyles.WS_CLIPCHILDREN |
		User32.WindowStyles.WS_MAXIMIZEBOX | User32.WindowStyles.WS_MINIMIZEBOX | User32.WindowStyles.WS_SIZEBOX | User32.WindowStyles.WS_SYSMENU | User32.WindowStyles.WS_DLGFRAME | User32.WindowStyles.WS_BORDER,
		User32.WindowStylesEx.WS_EX_WINDOWEDGE
	);


	public static readonly WinClass ToolWin_Class = new(
		"ToolWin",
		styles: User32.WindowClassStyles.CS_HREDRAW | User32.WindowClassStyles.CS_VREDRAW,
		hBrush: HBRUSH.NULL
	);
	public static readonly WinStylesDef ToolWin_StylesUndocked = new(
		User32.WindowStyles.WS_VISIBLE | User32.WindowStyles.WS_CLIPSIBLINGS | //User32.WindowStyles.WS_CLIPCHILDREN |
		User32.WindowStyles.WS_POPUP | User32.WindowStyles.WS_SYSMENU | User32.WindowStyles.WS_DLGFRAME | User32.WindowStyles.WS_BORDER | User32.WindowStyles.WS_SIZEBOX,
		User32.WindowStylesEx.WS_EX_TOOLWINDOW | User32.WindowStylesEx.WS_EX_WINDOWEDGE
	);
	public static readonly WinStylesDef ToolWin_StylesDocked = new(
		User32.WindowStyles.WS_VISIBLE | User32.WindowStyles.WS_CLIPSIBLINGS |
		User32.WindowStyles.WS_CHILD,
		User32.WindowStylesEx.WS_EX_TOOLWINDOW | User32.WindowStylesEx.WS_EX_WINDOWEDGE
	);

	public static readonly Marg FrameMargStd = new(0, 8, 8, 8);
	public static readonly Marg FrameMargMax = new(8, 8, 8, 8);
}