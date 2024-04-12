using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.StructsPackets;
using System.Drawing;
using Vanara.PInvoke;
using WinTest.Utils;

namespace WinTest.Wins;

sealed class ToolWin
{
	internal HWND Owner { get; }
	public bool IsDocked { get; internal set; }

	public SysWin Sys { get; } = new();

	public ToolWin(R r, HWND owner, bool isDocked)
	{
		Owner = owner;
		IsDocked = isDocked;

		NcCalcTweaker.TweakIf(
			Sys,
			() => Sys.IsMaximized() ? Styles.FrameMargMax : Styles.FrameMargStd,
			() => !IsDocked
		);
		ToolWinFileExt.SetupToolWinHitTest(Sys, () => IsDocked);
		//Sys.Evt.WhenNcActivate.Subs((ref NcActivatePacket e) =>
		//{
		//	e.UpdateRegion = -1;
		//	e.Handled = true;
		//});

		Sys.Evt.WhenPaint.Subs((ref PaintPacket e) =>
		{
			var hdc = User32.BeginPaint(e.Hwnd, out var ps);
			var gfx = Graphics.FromHdc(hdc.DangerousGetHandle());

			var sz = Sys.GetClientR().Size;
			const int captionHeight = 31;
			var captionR = new R(0, 0, sz.Width, captionHeight);
			var clientR = new R(0, captionHeight, sz.Width, sz.Height - captionHeight);
			gfx.FillDraw(captionR, ncBrush, ncPen);
			gfx.FillDraw(clientR, brush, pen);

			User32.EndPaint(e.Hwnd, ps);
		});

		Styles.ToolWin_Class.CreateWindow(Sys, IsDocked ? Styles.ToolWin_StylesDocked : Styles.ToolWin_StylesUndocked, r, owner, "ToolWin");
	}

	private static readonly Brush ncBrush = new SolidBrush(Col.Make(0xFFe1e359));
	private static readonly Pen ncPen = new(Col.Make(0xFF000000));

	private static readonly Brush brush = new SolidBrush(Col.Make(0xFFD18FF2));
	private static readonly Pen pen = new(Col.Make(0xFF551973));
}



file static class ToolWinFileExt
{
	public static bool IsMaximized(this SysWin win) => WinUtils.GetShowCmd(win) == ShowWindowCommand.SW_MAXIMIZE;


	public static void SetupToolWinHitTest(SysWin win, Func<bool> isDockedFun)
	{
		win.Evt.WhenNcHitTest.Subs((ref NcHitTestPacket e) =>
		{
			var mouse = win.Screen2Client(e.Point);

			var winR = win.GetWinR();
			var clientR = win.GetClientR();

			var clientRScr = win.Client2Screen(clientR);

			var ncSize = clientR.Size;
			var clientRAdj = clientRScr - winR.Pos;

			var result = ToolWinHitTestCalculator.ComputeResizeHitTestResult(
					ncSize,
					clientRAdj,
					mouse,
					out var resizeHit
				) switch
				{
					true => resizeHit,
					//false => win.NcHitTest(ncSize, mouse)
					false => User32.HitTestValues.HTCAPTION
				};

			Console.WriteLine($"HitTest  isDocked:{isDockedFun()}  result:{result}");

			e.Result = isDockedFun() switch
			{
				false => result,
				true => result switch
				{
					User32.HitTestValues.HTCAPTION or User32.HitTestValues.HTTOP or User32.HitTestValues.HTTOPRIGHT or User32.HitTestValues.HTRIGHT or User32.HitTestValues.HTBOTTOMRIGHT or
						User32.HitTestValues.HTBOTTOM or User32.HitTestValues.HTBOTTOMLEFT or User32.HitTestValues.HTLEFT or User32.HitTestValues.HTTOPLEFT
						=> User32.HitTestValues.HTBORDER,
					_ => result
				}
			};

			e.Handled = true;
		});
	}
}