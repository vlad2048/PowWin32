using PowWin32.Geom;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct SysCommandPacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public User32.SysCommand Command => (User32.SysCommand)Message->WParam.ToSafeInt32();
	public Pt Point => Message->LParam.ToPt();
	public bool IsAccelerator => Point.Y == -1;
	public bool IsMnemonic => Point.Y == 0;
}