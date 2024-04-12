using System.Runtime.InteropServices;
using PowWin32.Windows.StructsPInvoke;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct NcCreatePacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public ref CreateStruct CreateParams => ref ((CreateStructWrapper*)Message->LParam)->Value;
	/// <summary>
	/// true to continue
	/// </summary>
	public bool Result { get => Message->Result.ToBool(); set => Message->Result = value.FromBool(); }
}


[StructLayout(LayoutKind.Sequential)]
struct CreateStructWrapper
{
	public CreateStruct Value;
}