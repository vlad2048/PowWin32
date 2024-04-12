using Vanara.PInvoke;

namespace PowWin32.Windows;

public static class MsgPump
{
	public static int Run(SysWin? win = null)
	{
		static void OnDestroy() => User32.PostQuitMessage();

		if (win != null)
			win.Destroyed += OnDestroy;

		try
		{
			return RunLoop();
		}
		finally
		{
			if (win != null)
				win.Destroyed -= OnDestroy;
		}
	}

	private static int RunLoop()
	{
		int bRet;
		while ((bRet = User32.GetMessage(out MSG msg)) != 0)
		{
			if (bRet == -1)
				Win32Error.ThrowLastError();
			User32.TranslateMessage(msg);

			if (msg.message == (uint)WM.WM_CAPTURECHANGED)
			{
				Console.WriteLine($"Capture <- {msg.lParam:X}");
			}

			User32.DispatchMessage(msg);
		}
		return bRet;
	}
}