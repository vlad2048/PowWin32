using PowWin32.Geom;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct NcMouseButtonPacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public User32.HitTestValues HitTestValue => (User32.HitTestValues)Message->WParam;
	// ReSharper disable PatternIsRedundant
	public MouseButton Button => (int)MsgId switch
	{
		>= 0xA1 and <= 0xA3 => MouseButton.Left,
		>= 0xA4 and <= 0xA6 => MouseButton.Right,
		>= 0xA7 and <= 0xA9 => MouseButton.Middle,
		_ => Message->WParam.ToSafeInt32().HighAsInt() == 1 ? MouseButton.XButton1 : MouseButton.XButton2
	};
	// ReSharper restore PatternIsRedundant
	public bool IsDown => MsgId is WM.WM_NCLBUTTONDOWN or WM.WM_NCRBUTTONDOWN or WM.WM_NCMBUTTONDOWN or WM.WM_NCXBUTTONDOWN;
	public bool IsDoubleClick => MsgId is WM.WM_NCLBUTTONDBLCLK or WM.WM_NCRBUTTONDBLCLK or WM.WM_NCMBUTTONDBLCLK or WM.WM_NCXBUTTONDBLCLK;
	public Pt Point => Message->LParam.ToPt();
}