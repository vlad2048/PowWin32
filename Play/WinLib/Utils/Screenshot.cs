using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace WinLib.Utils;

public static class Screenshot
{
	public static Bitmap Take(HWND hwnd)
	{
		var r = hwnd.GetWinR();
		var bmp = new Bitmap(r.Width, r.Height, PixelFormat.Format32bppArgb);
		var gfxBmp = Graphics.FromImage(bmp);
		gfxBmp.CopyFromScreen(r.X, r.Y, 0, 0, new Size(r.Width, r.Height));
		gfxBmp.Dispose();
		return bmp;
	}


	public static bool AreSame(Bitmap? b1, Bitmap b2)
	{
		if (b1 == null) return false;
		if (b1.Size != b2.Size) return false;
		var bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
		var bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
		try
		{
			var bd1scan0 = bd1.Scan0;
			var bd2scan0 = bd2.Scan0;
			var stride = bd1.Stride;
			var len = stride * b1.Height;
			return memcmp(bd1scan0, bd2scan0, len) == 0;
		}
		finally
		{
			b1.UnlockBits(bd1);
			b2.UnlockBits(bd2);
		}
	}

	[DllImport("msvcrt.dll")] private static extern int memcmp(IntPtr b1, IntPtr b2, long count);
}