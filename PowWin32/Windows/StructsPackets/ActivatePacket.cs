using PowWin32.Windows.StructsPInvoke;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct ActivatePacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public WindowActivateFlag Flag => (WindowActivateFlag)Message->WParam.ToSafeInt32().LowAsInt();
	public bool IsMinimized => Message->WParam.ToSafeInt32().High() != 0;
	public nint ConverseHwnd => Message->LParam;
}