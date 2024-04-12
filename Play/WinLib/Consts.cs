using System.Drawing;
using PowWin32.Windows;
using PowWin32.Windows.Structs;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace WinLib;

public static class Consts
{
	public static class AppWin
	{
		public static readonly WinClass Class = new(
			"AppWin",
			styles: WindowClassStyles.CS_HREDRAW | WindowClassStyles.CS_VREDRAW,
			hBrush: HBRUSH.NULL
		);
		public static readonly WinStylesDef Styles = new(
			WindowStyles.WS_VISIBLE | WindowStyles.WS_CLIPSIBLINGS | //User32.WindowStyles.WS_CLIPCHILDREN |
			WindowStyles.WS_MAXIMIZEBOX | WindowStyles.WS_MINIMIZEBOX | WindowStyles.WS_SIZEBOX | WindowStyles.WS_SYSMENU | WindowStyles.WS_DLGFRAME | WindowStyles.WS_BORDER,
			WindowStylesEx.WS_EX_WINDOWEDGE
		);

		public static readonly Brush DrawBrush = MkBrush(0x8CBDFF);
		public static readonly Pen DrawPen = MkPen(0xFF2865);
	}


	public static class StdToolWin
	{
		public static readonly WinClass Class = new(
			"StdToolWin",
			styles: WindowClassStyles.CS_HREDRAW | WindowClassStyles.CS_VREDRAW,
			hBrush: HBRUSH.NULL
		);
		public static readonly WinStylesDef StdToolWin_StylesUndocked = new(
			WindowStyles.WS_VISIBLE | WindowStyles.WS_CLIPSIBLINGS |
			WindowStyles.WS_POPUP | WindowStyles.WS_SYSMENU | WindowStyles.WS_DLGFRAME | WindowStyles.WS_BORDER | WindowStyles.WS_SIZEBOX,
			WindowStylesEx.WS_EX_TOOLWINDOW //| WindowStylesEx.WS_EX_WINDOWEDGE
		);
		public static readonly WinStylesDef StdToolWin_StylesDocked = new(
			WindowStyles.WS_VISIBLE | WindowStyles.WS_CLIPSIBLINGS |
			WindowStyles.WS_CHILD,
			WindowStylesEx.WS_EX_TOOLWINDOW
		);

		public static readonly Brush DrawBrush = MkBrush(0xE1E359);
		public static readonly Pen DrawPen = MkPen(0x000000);
	}
}