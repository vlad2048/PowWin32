using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

public readonly unsafe struct Packet(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }
}