using System.Runtime.InteropServices;
using PowWin32.Windows.StructsPInvoke;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct GetMinMaxInfoPacket(WindowMessage* Message)
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public ref MinMaxInfo Info => ref ((MinMaxInfoWrapper*)Message->LParam)->Value;
}

[StructLayout(LayoutKind.Sequential)]
struct MinMaxInfoWrapper
{
	public MinMaxInfo Value;
}
