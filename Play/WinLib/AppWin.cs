using System.Drawing;
using PowWin32.Geom;
using PowWin32.Windows.StructsPackets;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;
using WinLib.Utils;

namespace WinLib;

public sealed class AppWin
{
	public SysWin Sys { get; } = new();

	public AppWin(R r)
	{
		Sys.Evt.WhenPaint.Subs((ref PaintPacket e) =>
		{
			var hdc = User32.BeginPaint(e.Hwnd, out var ps);
			var gfx = Graphics.FromHdc(hdc.DangerousGetHandle());

			gfx.FillDraw(Sys.Handle.GetClientR(), Consts.AppWin.DrawBrush, Consts.AppWin.DrawPen);

			User32.EndPaint(e.Hwnd, ps);
		});

		Consts.AppWin.Class.CreateWindow(Sys, Consts.AppWin.Styles, r, 0, "AppWin");
	}
}