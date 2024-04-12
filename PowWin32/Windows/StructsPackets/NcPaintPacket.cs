using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct NcPaintPacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public nint UpdateRegion
	{
		get => Message->WParam;
		set => Message->WParam = value;
	}
}