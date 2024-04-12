using System.Drawing;
using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.StructsPackets;
using Vanara.PInvoke;
using WinTest.Utils;

namespace WinTest.Wins;

sealed class AppWin
{
	public SysWin Sys { get; } = new();

	public AppWin(R r)
	{
		Sys.Evt.WhenPaint.Subs((ref PaintPacket e) =>
		{
			var hdc = User32.BeginPaint(e.Hwnd, out var ps);
			var gfx = Graphics.FromHdc(hdc.DangerousGetHandle());

			gfx.FillDraw(Sys.GetClientR(), brush, pen);

			User32.EndPaint(e.Hwnd, ps);
		});

		Styles.AppWin_Class.CreateWindow(Sys, Styles.AppWin_Styles, r, 0, "AppWin");
	}


	private static readonly Brush brush = new SolidBrush(Col.Make(0xFF8CBDFF));
	private static readonly Pen pen = new(Col.Make(0xFFFF2865));
}