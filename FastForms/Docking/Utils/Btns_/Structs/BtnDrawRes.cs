using PowWin32.Geom;
using System.Drawing.Imaging;
using FastForms.Utils.GdiUtils;

namespace FastForms.Docking.Utils.Btns_.Structs;

sealed record BtnDrawRes(
	Bitmap Bmp,
	Brush? BackBrush,
	ImageAttributes? Attrs
);


static class GfxBtnExt
{
	private static readonly ImageAttributes emptyAttrs = new();

	public static void PaintBtn(this Graphics gfx, BtnDrawRes res, Pt pos)
	{
		var r = new R(pos.X, pos.Y, res.Bmp.Width, res.Bmp.Height);
		if (res.BackBrush != null) gfx.FillRectangle(res.BackBrush, r.ToR());
		gfx.DrawImage(res.Bmp, r.ToR(), 0, 0, r.Width, r.Height, GraphicsUnit.Pixel, res.Attrs ?? emptyAttrs);
	}
}