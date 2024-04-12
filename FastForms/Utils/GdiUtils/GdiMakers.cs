using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Vanara.PInvoke;

namespace FastForms.Utils.GdiUtils;

public static class GdiMakers
{
    public static Gdi32.SafeHBRUSH MkBrushGdi(uint v)
    {
        var r = v >> 16 & 0xFF;
        var g = v >> 8 & 0xFF;
        var b = v & 0xFF;
        var vGdi = r + (g << 8) + (b << 16);
        return Gdi32.CreateSolidBrush(new COLORREF(vGdi));
    }

    public static Brush MkBrush(uint v, int alpha = 255) => new SolidBrush(MkColor(v, alpha));
    public static Brush MkBrush(Color v) => new SolidBrush(v);
    public static Pen MkPen(uint v, int alpha = 255) => new(MkColor(v, alpha));
    public static Color MkColor(uint v, int alpha = 255) => Color.FromArgb(alpha, Color.FromArgb(unchecked((int)v)));

    public static ImageAttributes MkImgAttrs(bool tiled, ColorMatrix? colorMatrix)
    {
        var attrs = new ImageAttributes();
        if (tiled) attrs.SetWrapMode(WrapMode.Tile);
        if (colorMatrix != null) attrs.SetColorMatrix(colorMatrix);
        return attrs;
    }
}