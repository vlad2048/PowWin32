using PowWin32.Windows.StructsPInvoke;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct CommandPacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public short Id => Message->WParam.ToSafeInt32().Low();
	public CommandSource CommandSource => (CommandSource)Message->WParam.ToSafeInt32().HighAsInt();
	public nint CommandHwnd => Message->LParam;
}