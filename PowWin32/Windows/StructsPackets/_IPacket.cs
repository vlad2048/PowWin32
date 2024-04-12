using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

public interface IPacket
{
	WM MsgId { get; }
	HWND Hwnd { get; }
	bool Handled { get; set; }
}