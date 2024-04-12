using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct NcActivatePacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	/// <summary>
	/// If true, the Result is ignored
	/// </summary>
	public bool IsActive => Message->WParam.ToBool();
	public nint UpdateRegion { get => Message->LParam; set => Message->LParam = value; }
	public NcActivateResult Result { get => (NcActivateResult)Message->Result; set => Message->Result = (int)value; }
}

public enum NcActivateResult
{
	/// <summary>
	/// Default processing.
	/// </summary>
	Default = 1,

	/// <summary>
	/// Prevent changes.
	/// </summary>
	PreventDefault = 0
}