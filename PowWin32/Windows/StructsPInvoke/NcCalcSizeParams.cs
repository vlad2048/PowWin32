using System.Drawing;
using System.Runtime.InteropServices;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPInvoke;


[StructLayout(LayoutKind.Sequential)]
public struct WindowPosition
{
	public nint Hwnd;
	public nint HwndZOrderInsertAfter;
	public int X;
	public int Y;
	public int Width;
	public int Height;
	public User32.SetWindowPosFlags Flags;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct NcCalcSizeParams
{
	public NcCalcSizeRegionUnion Region;
	public WindowPosition* Position;
}

[StructLayout(LayoutKind.Explicit)]
public struct NcCalcSizeRegionUnion
{
	[FieldOffset(0)] public NcCalcSizeInput Input;
	[FieldOffset(0)] public NcCalcSizeOutput Output;
}

[StructLayout(LayoutKind.Sequential)]
public struct NcCalcSizeInput
{
	public RECT TargetWindowRect;
	public RECT CurrentWindowRect;
	public RECT CurrentClientRect;
}

[StructLayout(LayoutKind.Sequential)]
public struct NcCalcSizeOutput
{
	public RECT TargetClientRect;
	public RECT DestRect;
	public RECT SrcRect;
}