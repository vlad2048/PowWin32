using System.Reactive.Linq;
using PowRxVar;
using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.StructsPackets;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace FastForms.Utils.Win32;

static class WinMoveInitiator
{
	public static void Start(
		SysWin sys,
		Pt grabPosScr,
		Action onFinish
	)
	{
		var grabOfs = grabPosScr - sys.GetWinR().Pos;

		User32.PostMessage(sys.Handle, (uint)WM.WM_SYSCOMMAND, 0xF010 | 0x0002);

		var tearD = new Disp("TearD");

		sys.Evt.WhenWindowPosChanging.Subs((ref WindowPosPacket e) =>
		{
			e.R = e.R.SetPos(MouseTracking.GetCursorPos() - grabOfs);

		}).D(tearD);

		sys.Evt.WhenMessage.ToObs().Where(e => e.Id == WM.WM_EXITSIZEMOVE).Take(1).Subscribe(_ =>
		{
			onFinish();
			tearD.Dispose();
		});
	}
}