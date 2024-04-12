using System.Runtime.InteropServices;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace PowWin32.Windows.StructsPInvoke;

[StructLayout(LayoutKind.Sequential)]
public struct CreateStruct
{
	public nint CreateParams;
	public nint InstanceHandle;
	public nint MenuHandle;
	public nint ParentHwnd;
	public int Height;
	public int Width;
	public int Y;
	public int X;
	public User32.WindowStyles Styles;
	public nint Name;
	public nint ClassName;
	public WindowStylesEx ExStyles;
}
