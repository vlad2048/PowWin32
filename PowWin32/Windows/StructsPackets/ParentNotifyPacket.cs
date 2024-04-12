using PowWin32.Geom;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

public readonly unsafe struct ParentNotifyPacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	/// <summary>
	/// Can be WM_CREATE, WM_DESTROY, WM_LBUTTONDOWN, WM_MBUTTONDOWN, WM_RBUTTONDOWN, WM_XBUTTONDOWN, WM_POINTERDOWN
	/// </summary>
	public WM ChildMsgId => (WM)Message->WParam.GetLow();

	/// <summary>
	/// For WM_CREATE, WM_DESTROY, this is the identifier of the child window
	/// For WM_XBUTTONDOWN, this can be either XBUTTON1 or XBUTTON2
	/// For WM_POINTERDOWN, this is the identifier of the pointer that generated the event
	/// For WM_LBUTTONDOWN, WM_MBUTTONDOWN, WM_RBUTTONDOWN, this is undefined
	/// </summary>
	public int ChildWParam => Message->WParam.GetHigh();

	/// <summary>
	/// For WM_CREATE, WM_DESTROY, this is the handle of the child window
	/// </summary>
	public HWND ChildHwnd => Message->LParam;

	/// <summary>
	/// For WM_LBUTTONDOWN, WM_MBUTTONDOWN, WM_RBUTTONDOWN, WM_XBUTTONDOWN, this is the position of the mouse
	/// For WM_POINTERDOWN, this is the point location of the pointer
	/// </summary>
	public Pt MousePos => Message->LParam.ToPt();
}