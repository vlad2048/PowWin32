using PowWin32.Windows.Utils;
using System.Runtime.InteropServices;
using PowWin32.Windows.StructsPInvoke;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

//@formatter:off

public unsafe struct NcCalcSizePacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public bool ShouldCalcValidRects => Message->WParam.ToBool();
	public ref NcCalcSizeParams Params => ref ((NcCalcSizeParamsWrapper*)Message->LParam)->Value;
	public WindowViewRegionFlags Result { get => (WindowViewRegionFlags)Message->Result.ToSafeInt32(); set => Message->Result = (int)value; }
}


[StructLayout(LayoutKind.Sequential)]
struct NcCalcSizeParamsWrapper
{
	public NcCalcSizeParams Value;
}
