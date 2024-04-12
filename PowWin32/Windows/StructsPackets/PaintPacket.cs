using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct PaintPacket(WindowMessage* Message)
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	/// <summary>
	/// Only some common controls fill in WParam
	/// </summary>
	public nint Hdc => Message->WParam;
}