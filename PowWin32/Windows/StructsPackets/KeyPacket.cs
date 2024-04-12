using PowWin32.Windows.StructsPInvoke;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct KeyPacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public VirtualKey VirtualKey => (VirtualKey)Message->WParam.ToSafeInt32();
	public KeyboardInputState InputState => new(Message->LParam.ToSafeUInt32());
	public bool IsKeyDown => MsgId is WM.WM_KEYDOWN or WM.WM_SYSKEYDOWN;
	public bool IsSystemContext => MsgId is WM.WM_SYSKEYDOWN or WM.WM_SYSKEYUP;
}