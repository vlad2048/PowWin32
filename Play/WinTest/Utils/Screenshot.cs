using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.Structs;
using Vanara.PInvoke;

namespace WinTest.Utils;

public static class Screenshot
{
	private static readonly TimeSpan Duration = TimeSpan.FromSeconds(5);
	private const string Folder = @"C:\tmp\screenshots";

	public static void SaveScreenshots(this SysWin win)
	{
		var files = Directory.GetFiles(Folder);
		foreach (var file in files) File.Delete(file);

		var startTime = DateTime.Now;
		Bitmap? bmpLast = null;
		var idx = 0;
		var enabled = true;

		void PaintStop()
		{
			//var hdc = User32.GetDCEx(win.Handle, default, User32.DCX.DCX_WINDOW);
			//var res = Gdi32.ExcludeClipRect(hdc, 0, 0, 50, 100);
			//Console.WriteLine($"exclude: {res}");
			User32.SetWindowRgn(win.Handle, HRGN.NULL, false).L("SetWindowRgn");
		}

		void PaintStart()
		{

		}

		PaintStop();

		bool IsEnabled()
		{
			if (!enabled) return false;
			if (DateTime.Now - startTime > Duration)
			{
				PaintStart();
				enabled = false;
				return false;
			}
			return true;
		}


		var cntMap = new Dictionary<WM, int> {
			{ WM.WM_PAINT, 0 }
		};


		var level = 0;

		void Log(WM id, string? extra = null)
		{
			var sb = new StringBuilder(new string(' ', level * 4));
			sb.Append($"{id}".PadRight(32));
			sb.Append($"{extra}");
			Console.WriteLine(sb.ToString());
		}


		win.Evt.WhenMessage.Subs((ref WindowMessage e) =>
		{
			if (!IsEnabled()) return;
			Log(e.Id);
			level++;
		});

		win.Evt.WhenMessagePost.Subs((ref WindowMessage e) =>
		{
			if (!IsEnabled()) return;
			if (cntMap.TryGetValue(e.Id, out var cnt) && cnt > 0)
			{
				cntMap[e.Id]--;
				e.Handled = true;
				return;
			}


			var bmp = Take(win);
			var areSame = AreSame(bmpLast, bmp);
			if (!areSame)
			{
				var file = Path.Combine(Folder, $"scr_{idx++}_{e.Id}.png");
				bmp.Save(file);
				bmpLast = bmp;
			}

			level--;
			Log(e.Id, areSame ? "" : "BMP");
		});
	}


	private static void L<T>(this T obj, string? str = null)
	{
		if (str == null)
			Console.WriteLine($"{obj}");
		else
			Console.WriteLine($"[{str}] - {obj}");
	}


	private static Bitmap Take(SysWin hwnd)
	{
		var r = WinUtils.GetWinR(hwnd);
		var bmp = new Bitmap(r.Width, r.Height, PixelFormat.Format32bppArgb);
		var gfxBmp = Graphics.FromImage(bmp);
		gfxBmp.CopyFromScreen(r.X, r.Y, 0, 0, new Size(r.Width, r.Height));
		gfxBmp.Dispose();
		return bmp;
	}


	private static bool AreSame(Bitmap? b1, Bitmap b2)
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