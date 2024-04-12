using PowWin32.Windows.StructsPInvoke;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct MouseActivatePacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public nint TopLevelActiveParentHwnd => Message->WParam;
	public HitTestValues HitTestResult => (User32.HitTestValues)Message->LParam.ToSafeInt32().LowAsInt();
	public ushort MouseMessageId => (ushort)Message->LParam.ToSafeInt32().High();
	public MouseActivationResult Result { get => (MouseActivationResult)Message->Result; set => Message->Result = (int)value; }
}