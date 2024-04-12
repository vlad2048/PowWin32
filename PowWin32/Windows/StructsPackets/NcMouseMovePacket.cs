using PowWin32.Geom;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct NcMouseMovePacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public User32.HitTestValues HitTestValue => (User32.HitTestValues)Message->WParam;
	public Pt Point => Message->LParam.ToPt();
}