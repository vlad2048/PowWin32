using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct EraseBkgndPacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public nint Hdc => Message->WParam;
	public EraseBackgroundResult Result { get => (EraseBackgroundResult)Message->Result.ToSafeInt32(); set => Message->Result = (int)value; }
}

public enum EraseBackgroundResult
{
	/// <summary>
	/// Let DefWndProc erase the background with the window class's brush.
	/// </summary>
	Default = 0,

	/// <summary>
	/// prevent default erase.
	/// </summary>
	DisableDefaultErase = 1
}