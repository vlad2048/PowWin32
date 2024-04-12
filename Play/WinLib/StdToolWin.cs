using System.Drawing;
using PowWin32.Geom;
using PowWin32.Windows.StructsPackets;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;
using WinLib.Utils;

namespace WinLib;

public sealed class StdToolWin
{
	public SysWin Sys { get; } = new();
	public HWND Owner { get; }

	public bool IsDocked => Sys.Handle.HasStyle(User32.WindowStyles.WS_CHILD);

	public StdToolWin(HWND owner, R r, bool docked)
	{
		Owner = owner;
		Sys.Evt.WhenPaint.Subs((ref PaintPacket e) =>
		{
			var hdc = User32.BeginPaint(e.Hwnd, out var ps);
			var gfx = Graphics.FromHdc(hdc.DangerousGetHandle());

			gfx.FillDraw(Sys.Handle.GetClientR(), Consts.StdToolWin.DrawBrush, Consts.StdToolWin.DrawPen);

			User32.EndPaint(e.Hwnd, ps);
		});

		var styles = docked ? Consts.StdToolWin.StdToolWin_StylesDocked : Consts.StdToolWin.StdToolWin_StylesUndocked;
		Consts.StdToolWin.Class.CreateWindow(Sys, styles, r, Owner, "StdToolWin");
	}
}