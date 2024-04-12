using PowWin32.Windows;
using PowWin32.Windows.Utils;

namespace PowWin32.Diag;

public static class ConsoleMemChecker
{
	private static readonly TimeSpan Period = TimeSpan.FromSeconds(0.1);
	private const int ValuesPerRow = 20;

	public static void Init(SysWin sys)
	{
		var memPrev = 0L;
		var cnt = -1;

		sys.Schedule(0, Period, true, () =>
		{
			var mem = GC.GetAllocatedBytesForCurrentThread();
			var delta = mem - memPrev;
			memPrev = mem;
			var mod = cnt % ValuesPerRow;
			if (mod == 0)
			{
				if (cnt > 0)
					Console.WriteLine();
				Console.Write("mem: ");
			}
			if (cnt >= 0)
			{
				Console.Write(delta);
				if (mod < ValuesPerRow - 1)
					Console.Write(", ");
			}
			cnt++;
		});
	}
}