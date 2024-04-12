using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.StructsPackets;
using Vanara.PInvoke;

namespace FastForms.Docking.Utils;

public static class NcCalcTweaker
{
	public static void Tweak(SysWin win, Marg marg) => TweakIf(win, () => marg, () => true);

	public static void Tweak(SysWin win, Func<Marg> margFun) => TweakIf(win, margFun, () => true);

	public static void TweakIf(SysWin win, Func<Marg> margFun, Func<bool> predicate)
	{
		win.Evt.WhenNcCalcSize.Subs((ref NcCalcSizePacket e) =>
		{
			//Console.WriteLine("WhenNcCalcSize");
			if (e.ShouldCalcValidRects)
			{
				if (predicate())
				{
					var marg = margFun();
					e.Params.Region.Output.TargetClientRect.Left += marg.Left;
					e.Params.Region.Output.TargetClientRect.Top += marg.Up;
					e.Params.Region.Output.TargetClientRect.Right -= marg.Right;
					e.Params.Region.Output.TargetClientRect.Bottom -= marg.Bottom;
					e.Handled = true;
				}
			}
		});

		win.Evt.WhenCreate.Subs((ref CreatePacket _) => User32.SetWindowPos(
			win.Handle,
			nint.Zero,
			0, 0, 0, 0,
			User32.SetWindowPosFlags.SWP_FRAMECHANGED | User32.SetWindowPosFlags.SWP_NOMOVE |
			User32.SetWindowPosFlags.SWP_NOSIZE | User32.SetWindowPosFlags.SWP_NOOWNERZORDER |
			User32.SetWindowPosFlags.SWP_NOZORDER
		));
	}
}