using PowWin32.Diag;
using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace FastForms.Utils.Win32;

static class LayeredWindowUtils
{
	private const byte AC_SRC_ALPHA = 0x01;
	private const byte AC_SRC_OVER = 0x00;


	public static void Paint(
		HWND handle,
		R winR,
		Action<Graphics> gfxFun
	)
	{
		var hdc = User32.GetDC(handle);

		var memDC = Gdi32.CreateCompatibleDC(hdc);
		var memBmp = Gdi32.CreateCompatibleBitmap(hdc, winR.Width, winR.Height);

		Gdi32.SelectObject(memDC, memBmp);
		var gfx = Graphics.FromHdc(memDC.DangerousGetHandle());
		gfxFun(gfx);

		// @formatter:off
		User32.UpdateLayeredWindow(
			hWnd: handle,
			hdcDst:		0,
			pptDst:		winR.Pos,
			psize:		winR.Size,
			hdcSrc:		memDC,
			pptSrc:		new Pt(0, 0),
			crKey:		COLORREF.None,
			pblend:		new Gdi32.BLENDFUNCTION
						{				
							AlphaFormat = AC_SRC_ALPHA,
							BlendFlags = 0,
							BlendOp = AC_SRC_OVER,
							SourceConstantAlpha = 255,
						},
			dwFlags:	User32.UpdateLayeredWindowFlags.ULW_ALPHA
		).Check();
		// @formatter:on

		Gdi32.DeleteDC(memDC);
		Gdi32.DeleteObject(memBmp);
	}



	public static void PaintDrop(
		HWND handle,
		R winR,
		Action<Graphics> gfxFun
	)
	{
		var hdc = User32.GetDC(handle);

		var memDC = Gdi32.CreateCompatibleDC(hdc);
		var memBmp = Gdi32.CreateCompatibleBitmap(hdc, winR.Width, winR.Height);

		Gdi32.SelectObject(memDC, memBmp);
		var gfx = Graphics.FromHdc(memDC.DangerousGetHandle());
		gfxFun(gfx);

		// @formatter:off
		User32.UpdateLayeredWindow(
			hWnd: handle,
			hdcDst: 0,
			pptDst: winR.Pos,
			psize: winR.Size,
			hdcSrc: memDC,
			pptSrc: new Pt(0, 0),
			crKey: COLORREF.None,
			pblend: new Gdi32.BLENDFUNCTION
			{
				AlphaFormat = AC_SRC_ALPHA,
				BlendFlags = 0,
				BlendOp = AC_SRC_OVER,
				SourceConstantAlpha = 255,
			},
			dwFlags: User32.UpdateLayeredWindowFlags.ULW_ALPHA
		).Check();
		// @formatter:on

		Gdi32.DeleteDC(memDC);
		Gdi32.DeleteObject(memBmp);
	}

}