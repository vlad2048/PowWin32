using System.Runtime.InteropServices;
using PowWin32.Geom;
using PowWin32.Windows.StructsPInvoke;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
// WM.WINDOWPOSCHANGED, WM.WINDOWPOSCHANGING
public readonly unsafe struct WindowPosPacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public ref WindowPosition Position => ref ((WindowPositionWrapper*)Message->LParam)->Value;

	public R R
	{
		get => new(Position.X, Position.Y, Position.Width, Position.Height);
		set
		{
			Position.X = value.X;
			Position.Y = value.Y;
			Position.Width = value.Width;
			Position.Height = value.Height;
		}
	}
}

[StructLayout(LayoutKind.Sequential)]
struct WindowPositionWrapper
{
	public WindowPosition Value;
}
