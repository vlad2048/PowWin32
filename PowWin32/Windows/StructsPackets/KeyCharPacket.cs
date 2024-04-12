using PowWin32.Windows.StructsPInvoke;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct KeyCharPacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public char Key => (char)Message->WParam.ToSafeInt32();
	public KeyboardInputState InputState => new(Message->LParam.ToSafeUInt32());
	public bool IsDeadChar => MsgId is WM.WM_DEADCHAR or WM.WM_SYSDEADCHAR;
	public bool IsSystemContext => MsgId is WM.WM_SYSCHAR or WM.WM_SYSDEADCHAR;
}