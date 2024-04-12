using PowWin32.Windows.StructsPInvoke;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct HotKeyPacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public ScreenshotHotKey ScreenshotHotKey => (ScreenshotHotKey)Message->WParam.ToSafeInt32();
	public HotKeyInputState KeyState => (HotKeyInputState)Message->LParam.ToSafeInt32().Low();
	public VirtualKey Key => (VirtualKey)Message->LParam.ToSafeInt32().High();
}