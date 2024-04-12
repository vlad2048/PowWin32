using System.Runtime.InteropServices;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPInvokeWM;

public static class User32WM
{
	[PInvokeData("winuser.h")]
	[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "DefWindowProc")]
	public static extern nint DefWindowProcWM(HWND hWnd, WM Msg, nint wParam, nint lParam);

	[PInvokeData("winuser.h", MSDNShortId = "")]
	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, EntryPoint = "RegisterClassEx")]
	public static extern ushort RegisterClassExWM(in WNDCLASSEXWM Arg1);
}