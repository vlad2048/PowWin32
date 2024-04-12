using PowWin32.Geom;
using PowWin32.Windows.StructsPInvokeWM;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct NcHitTestPacket(WindowMessage* message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public Pt Point => Message->LParam.ToPt();
	public HitTestValues Result { get => (HitTestValues)Message->Result; set => Message->Result = (int)value; }

	public HitTestValues GetDefaultResult() => (HitTestValues)User32WM.DefWindowProcWM(Hwnd, MsgId, Message->WParam, Message->LParam);

	internal WindowMessage* Message => message;
}