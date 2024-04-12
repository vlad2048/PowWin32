using PowWin32.Geom;
using PowWin32.Windows.StructsPInvoke;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct SizePacket(WindowMessage* Message)
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public WindowSizeFlag Flag => (WindowSizeFlag)Message->WParam.ToSafeInt32();
	public Sz Size => Message->LParam.ToSz();
}