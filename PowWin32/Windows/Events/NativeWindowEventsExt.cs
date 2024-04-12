using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.StructsPackets;

namespace PowWin32.Windows.Events;

public static class NativeWindowEventsExt
{
	public static void DisableEraseBkgnd(this NativeWindowEvents evt) =>
		evt.WhenEraseBkgnd.Subs((ref EraseBkgndPacket e) =>
		{
			e.Result = EraseBackgroundResult.DisableDefaultErase;
			e.Handled = true;
		});
}