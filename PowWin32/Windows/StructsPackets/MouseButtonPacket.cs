using PowWin32.Geom;
using PowWin32.Windows.StructsPInvoke;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct MouseButtonPacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public MouseInputKeyStateFlags InputState => (MouseInputKeyStateFlags)Message->WParam.ToSafeInt32().LowAsInt();
	// ReSharper disable PatternIsRedundant
	public MouseButton Button => (int)MsgId switch
	{
		>= 0x201 and <= 0x203 => MouseButton.Left,
		>= 0x204 and <= 0x206 => MouseButton.Right,
		>= 0x207 and <= 0x209 => MouseButton.Middle,
		_ => (MouseInputXButtonFlag)Message->WParam.ToSafeInt32().HighAsInt() == MouseInputXButtonFlag.XBUTTON1 ? MouseButton.XButton1 : MouseButton.XButton2
	};
	// ReSharper restore PatternIsRedundant
	public bool IsDown => MsgId is WM.WM_LBUTTONDOWN or WM.WM_RBUTTONDOWN or WM.WM_MBUTTONDOWN or WM.WM_XBUTTONDOWN;
	public bool IsDoubleClick => MsgId is WM.WM_LBUTTONDBLCLK or WM.WM_RBUTTONDBLCLK or WM.WM_MBUTTONDBLCLK or WM.WM_XBUTTONDBLCLK;
	public Pt Point => Message->LParam.ToPt();
}

public enum MouseButton
{
	Left = 0x1,
	Right = 0x2,
	Middle = 0x4,
	Other = 0x8,
	XButton1 = 0x10 | Other,
	XButton2 = 0x20 | Other
}