using PowWin32.Geom;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace FastForms.Utils.Win32;

static class WindowFinder
{
	public static T? GetWindowAt<T>(Pt mouse, string propName, HWND exclude) where T : class
	{
		(HWND, T)[] ws =
		[
			..
			from win in GetTopWindowsInThread()
			where win != exclude
			where win.HasProp(propName)
			let winObj = win.GetProp<T>(propName)
			where win.GetDwmR().Contains(mouse)
			select (win, winObj)
		];

		var zs = GetZOrder([.. ws.Select(e => e.Item1)]);

		return (
				from t in ws.Zip(zs)
				let winObj = t.First.Item2
				let zOrder = t.Second
				orderby zOrder
				select winObj
			)
			.FirstOrDefault();
	}


	private static HWND[] GetTopWindowsInThread()
	{
		var threadId = Kernel32.GetCurrentThreadId();
		var list = new List<HWND>();
		User32.EnumThreadWindows(threadId, (hwnd, _) =>
		{
			list.Add(hwnd);
			return true;
		}, 0);
		return [.. list];
	}


	private static int[] GetZOrder(HWND[] hwnds)
	{
		var z = new int[hwnds.Length];
		for (var i = 0; i < hwnds.Length; i++) z[i] = -1;

		var index = 0;
		var numRemaining = hwnds.Length;
		User32.EnumWindows((wnd, _) =>
		{
			var searchIndex = Array.IndexOf(hwnds, wnd);
			if (searchIndex != -1)
			{
				z[searchIndex] = index;
				numRemaining--;
				if (numRemaining == 0) return false;
			}
			index++;
			return true;
		}, 0);

		return z;
	}
}