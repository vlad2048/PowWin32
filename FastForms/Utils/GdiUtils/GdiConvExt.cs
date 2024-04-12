using PowWin32.Geom;

namespace FastForms.Utils.GdiUtils;

public static class GdiConvExt
{
    public static Point ToPt(this Pt e) => new(e.X, e.Y);
    public static Pt ToPt(this Point e) => new(e.X, e.Y);

    public static Rectangle ToR(this R e) => new(e.X, e.Y, e.Width, e.Height);
    public static R ToR(this Rectangle e) => new(e.X, e.Y, e.Width, e.Height);

    public static Size ToSz(this Sz e) => new(e.Width, e.Height);
    public static Sz ToSz(this Size e) => new(e.Width, e.Height);
    public static Sz ToSz(this SizeF e) => new((int)e.Width, (int)e.Height);
}