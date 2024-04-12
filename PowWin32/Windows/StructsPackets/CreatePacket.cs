using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct CreatePacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public CreateWindowResult Result { get => (CreateWindowResult)Message->Result.ToSafeInt32(); set => Message->Result = (int)value; }
}

public enum CreateWindowResult
{
	Default = 0,
	PreventCreation = -1
}