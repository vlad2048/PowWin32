using PowWin32.Windows.StructsPackets;
using System.Reactive.Disposables;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace FastForms.Utils.GdiUtils;

public static class GdiInitExt
{
	public static IDisposable Paint(this ref PaintPacket e, out Graphics gfx) => Tweaks.UseDoubleBuffering switch
	{
		false => e.PaintNormal(out gfx),
		true => e.PaintDoubleBuffered(out gfx),
	};

	private static IDisposable PaintDoubleBuffered(this ref PaintPacket e, out Graphics gfx)
	{
		var hwnd = e.Hwnd;
		var hdc = User32.BeginPaint(hwnd, out var ps);
		var sysGfx = Graphics.FromHdc(hdc.DangerousGetHandle());

		var clientR = hwnd.GetClientR();
		var bmp = new Bitmap(Math.Max(1, clientR.Width), Math.Max(1, clientR.Height));
		gfx = Graphics.FromImage(bmp);

		return Disposable.Create((hwnd, ps), t =>
		{
			sysGfx.DrawImage(bmp, 0, 0);

			User32.EndPaint(t.hwnd, t.ps);
		});
	}


	private static IDisposable PaintNormal(this ref PaintPacket e, out Graphics gfx)
	{
		var hwnd = e.Hwnd;
		var hdc = User32.BeginPaint(hwnd, out var ps);
		gfx = Graphics.FromHdc(hdc.DangerousGetHandle());
		return Disposable.Create((hwnd, ps), t => User32.EndPaint(t.hwnd, t.ps));
	}
}