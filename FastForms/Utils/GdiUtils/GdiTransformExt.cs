using System.Reactive.Disposables;
using PowWin32.Geom;

namespace FastForms.Utils.GdiUtils;

public static class GdiTransformExt
{
	public static IDisposable PushOffset(this Graphics gfx, Pt ofs)
	{
		gfx.TranslateTransform(ofs.X, ofs.Y);
		return Disposable.Create(ofs, ofs_ => gfx.TranslateTransform(-ofs_.X, -ofs_.Y));
	}
}