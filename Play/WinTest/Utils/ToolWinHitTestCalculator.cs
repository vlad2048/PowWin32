using PowWin32.Geom;
using Vanara.PInvoke;

namespace WinTest.Utils;

static class ToolWinHitTestCalculator
{
	private const int TopBorderResizeOverride = 6;
	private const int CloseThreshold = 5;

	public static bool ComputeResizeHitTestResult(Sz ncSize, R clientR, Pt mouse, out User32.HitTestValues hit)
	{
		hit = ComputeHitTestResult(ncSize, clientR, mouse);
		return hit != User32.HitTestValues.HTNOWHERE;
	}

	private static User32.HitTestValues ComputeHitTestResult(Sz ncSize, R clientR, Pt mouse)
	{
		//if (clientR.Contains(mouse)) return User32.HitTestValues.HTCLIENT;

		var left = mouse.X <= 0;
		var leftClose = mouse.X <= CloseThreshold;

		var top = mouse.Y <= 0 + TopBorderResizeOverride;
		var topClose = mouse.Y <= 0 + CloseThreshold;

		var right = mouse.X >= ncSize.Width - 1;
		var rightClose = mouse.X >= ncSize.Width - CloseThreshold - 1;

		var bottom = mouse.Y >= ncSize.Height - 1;
		var bottomClose = mouse.Y >= ncSize.Height - CloseThreshold - 1;


		if (left && topClose || leftClose && top) return User32.HitTestValues.HTTOPLEFT;
		if (right && topClose || rightClose && top) return User32.HitTestValues.HTTOPRIGHT;

		if (left && bottomClose || leftClose && bottom) return User32.HitTestValues.HTBOTTOMLEFT;
		if (right && bottomClose || rightClose && bottom) return User32.HitTestValues.HTBOTTOMRIGHT;


		return (left.b(), right.b(), top.b(), bottom.b()) switch
		{
			(1, 0, 1, 0) => User32.HitTestValues.HTTOPLEFT,
			(0, 1, 1, 0) => User32.HitTestValues.HTTOPRIGHT,
			(1, 0, 0, 1) => User32.HitTestValues.HTBOTTOMLEFT,
			(0, 1, 0, 1) => User32.HitTestValues.HTBOTTOMRIGHT,

			(1, 0, 0, 0) => User32.HitTestValues.HTLEFT,
			(0, 1, 0, 0) => User32.HitTestValues.HTRIGHT,
			(0, 0, 1, 0) => User32.HitTestValues.HTTOP,
			(0, 0, 0, 1) => User32.HitTestValues.HTBOTTOM,

			_ => User32.HitTestValues.HTNOWHERE,
		};
	}

	private static int b(this bool v) => v ? 1 : 0;
}