using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.StructsPackets;
using PowWin32.Windows.Utils;
using static Vanara.PInvoke.User32;
// ReSharper disable InconsistentNaming

namespace FastForms.Docking.Utils;


static class HitTester
{
	private const int m = 6;
	private const int M = 11;
	private const int mTop = 8;

	public static void ForFrame(
		SysWin sys,
		int captionHeight,
		Func<Pt, bool> isOverBtn
	)
	{
		sys.Evt.WhenNcHitTest.Subs((ref NcHitTestPacket e) =>
		{
			var p = sys.Screen2Client(e.Point);

			var r = sys.GetClientR();
			var clientR = r - new Marg(captionHeight, 0, 0, 0);

			if (sys.IsMaximized())
			{
				if (clientR.Contains(p) || isOverBtn(p))
				{
					e.Result = HitTestValues.HTCLIENT;
					e.Handled = true;
				}
				else if (p.Y <= captionHeight)
				{
					e.Result = HitTestValues.HTCAPTION;
					e.Handled = true;
				}
				else
				{
					e.Result = HitTestValues.HTNOWHERE;
					e.Handled = true;
				}
			}
			else
			{
				if (clientR.Contains(p) || isOverBtn(p))
				{
					e.Result = HitTestValues.HTCLIENT;
					e.Handled = true;
				}
				else
				{
					if (p.Y <= mTop)
					{
						var isLeft = p.X <= r.X + m;
						var isRight = p.X >= r.Right - m;
						var val = (isLeft, isRight) switch
						{
							(false, false) => HitTestValues.HTTOP,
							(true, false) => HitTestValues.HTTOPLEFT,
							(false, true) => HitTestValues.HTTOPRIGHT,
							(true, true) => Math.Abs(p.X) < Math.Abs(p.X - r.Right) ? HitTestValues.HTTOPLEFT : HitTestValues.HTTOPRIGHT,
						};
						e.Result = val;
						e.Handled = true;
					}
					else if (p.Y <= captionHeight)
					{
						e.Result = HitTestValues.HTCAPTION;
						e.Handled = true;
					}
					else if (p.Y >= r.Bottom - 1)
					{
						var isLeft = p.X <= r.X + M;
						var isRight = p.X >= r.Right - M;
						var val = (isLeft, isRight) switch
						{
							(false, false) => HitTestValues.HTBOTTOM,
							(true, false) => HitTestValues.HTBOTTOMLEFT,
							(false, true) => HitTestValues.HTBOTTOMRIGHT,
							(true, true) => Math.Abs(p.X) < Math.Abs(p.X - r.Right) ? HitTestValues.HTBOTTOMLEFT : HitTestValues.HTBOTTOMRIGHT,
						};
						e.Result = val;
						e.Handled = true;
					}
					else
					{
						e.Result = Math.Abs(p.X) < Math.Abs(p.X - r.Right) ? HitTestValues.HTLEFT : HitTestValues.HTRIGHT;
						e.Handled = true;
					}
				}
			}

			//L($"left:{p.X}  top:{p.Y}  right:{r.Right - p.X}  bottom:{r.Bottom - p.Y} -> {e.Result}  (max:{sys.IsMaximized()})");
		});
	}


	public static void ForPassthrough(
		SysWin sys,
		int captionHeight,
		Func<Pt, bool> isOverBtn,
		Func<bool> enableFun
	)
	{
		sys.Evt.WhenNcHitTest.Subs((ref NcHitTestPacket e) =>
		{
			if (!enableFun()) return;

			var p = sys.Screen2Client(e.Point);

			var isBtn = isOverBtn(p);

			if (!isBtn && p.Y <= captionHeight)
			{
				e.Result = HitTestValues.HTTRANSPARENT;
				e.Handled = true;
			}
		});
	}
}



/*
static class HitTester
{
	private const int TopResizeMargin = 7;
	private const int HorzResizeThreshold = 7;

	public static void Setup(SysWin win, int captionHeight)
	{
		win.Evt.WhenNcHitTest.Subs((ref NcHitTestPacket e) =>
		{
			var p = e.Point;
			var sysHit = e.GetDefaultResult();

			e.Result = sysHit.IsResize() switch
			{
				true => sysHit,
				false => win.IsMaximized() switch

				{
					//false => ()
				}
			};

			e.Handled = true;
		});
	}

	private static HitTestValues HitNormal(SysWin win, Pt p, int captionHeight)
	{
		var r = win.GetWinR() - DockerLayout.WinMargStd;
		p -= r.Pos;
		return (p.Y < TopResizeMargin) switch {
			true => (p.X < HorzResizeThreshold, p.X > r.Width - HorzResizeThreshold) switch {
				(true, false) => HitTestValues.HTTOPLEFT,
				(false, true) => HitTestValues.HTTOPRIGHT,
				_ => HitTestValues.HTTOP
			},
			false => (p.Y < captionHeight) switch
			{
				true => HitTestValues.HTCAPTION,
				false => HitTestValues.HTCLIENT
			}
		};
	}

	private static bool IsResize(this HitTestValues e) => e is HitTestValues.HTTOPLEFT or HitTestValues.HTTOP or HitTestValues.HTTOPRIGHT or HitTestValues.HTRIGHT or HitTestValues.HTBOTTOMRIGHT or HitTestValues.HTBOTTOM or HitTestValues.HTBOTTOMLEFT or HitTestValues.HTLEFT;
}
*/
