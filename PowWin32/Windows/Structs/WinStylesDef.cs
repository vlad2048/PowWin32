using static Vanara.PInvoke.User32;

namespace PowWin32.Windows.Structs;

public sealed record WinStylesDef(
	WindowStyles Styles,
	WindowStylesEx StylesEx
);