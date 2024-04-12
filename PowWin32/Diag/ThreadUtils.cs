using Thread = System.Threading.Thread;

namespace PowWin32.Diag;

static class ThreadUtils
{
	private static int? uiThreadId;

	public static void EnsureUIThread()
	{
		if (uiThreadId is null)
		{
			uiThreadId = Thread.CurrentThread.ManagedThreadId;
		}
		else
		{
			if (Thread.CurrentThread.ManagedThreadId != uiThreadId.Value)
				throw new ArgumentException("We expect to be in the UI thread here");
		}
	}
}