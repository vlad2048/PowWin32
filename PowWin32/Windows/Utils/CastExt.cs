using PowWin32.Geom;

namespace PowWin32.Windows.Utils;

public static class CastExt
{
	public static int GetLow(this nint v) => v.ToSafeInt32().LowAsInt();
	public static int GetHigh(this nint v) => v.ToSafeInt32().HighAsInt();




	public static bool ToBool(this nint v) => v.ToSafeInt32() != 0;
	public static nint FromBool(this bool v) => v ? 1 : 0;

	public static Pt ToPt(this nint v)
	{
		v.BreakSafeInt32To16Signed(out var y, out var x);
		return new Pt(x, y);
	}

	public static nint FromPt(this Pt v) => ((v.Y & 0xFFFF) << 16) + (v.X & 0xFFFF);

	public static Sz ToSz(this nint v)
	{
		v.BreakSafeInt32To16Signed(out var height, out var width);
		return new Sz(width, height);
	}

	private static void BreakSafeInt32To16Signed(this nint ptr, out int high16, out int low16)
	{
		int safeInt32 = ptr.ToSafeInt32();
		low16 = safeInt32.Low();
		high16 = safeInt32.High();
	}

	public static int LowAsInt(this int dword) => dword & 0xFFFF;
    public static int HighAsInt(this int dword) => dword >> 16 & 0xFFFF;

	public static int ToSafeInt32(this nint ptr) => nint.Size <= 4 ? ptr.ToInt32() : (int)ptr.ToInt64();
	public static uint ToSafeUInt32(this nint ptr) => nint.Size <= 4 ? (uint)ptr.ToInt32() : (uint)ptr.ToInt64();

	public static short Low(this int dword) => (short) dword;
    public static short High(this int dword) => (short) (dword >> 16);
    public static uint WithLow(this uint dword, ushort low16) => dword & 4294901760U | low16;
    public static ushort Low(this uint dword) => (ushort) dword;
}