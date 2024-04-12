using PowWin32.Windows.StructsPInvoke;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct AppCommandPacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public nint CommandHwnd => Message->WParam;
	public KeyboardInputState InputState => new((uint)Message->LParam.ToSafeInt32().LowAsInt());
	public AppCommand Command => (AppCommand)(Message->LParam.ToSafeInt32().HighAsInt() & ~(uint)AppCommandDevice.FAPPCOMMAND_MASK);
	public AppCommandDevice Device => (AppCommandDevice)(Message->LParam.ToSafeInt32().HighAsInt() & (uint)AppCommandDevice.FAPPCOMMAND_MASK);
	public AppCommandResult Result { get => (AppCommandResult)Message->Result; set => Message->Result = (int)value; }
}

public enum AppCommandResult
{
	Default = 0,
	Handled = 1
}