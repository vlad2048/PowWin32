using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace PowWin32.Utils;

public static class RxSchedUtils
{
	public static IScheduler Sched
	{
		get
		{
			InitIFN();
			return sched ?? throw new ArgumentException("Should not be null");
		}
	}

	public static IObservable<T> ObserveOnUIThread<T>(this IObservable<T> source) => source.ObserveOn(Sched);

	public static void DisposeOnUIThread(this IDisposable innerD)
	{
		new ScheduledDisposable(
			Sched,
			innerD
		).Dispose();
	}




	private static IScheduler? sched;

	private static void InitIFN()
	{
		if (SynchronizationContext.Current == null)
		{
			var ctx = new WindowsFormsSynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(ctx);
			sched = new SynchronizationContextScheduler(ctx);
		}

		AssMsg(sched != null, "Wrong");
	}
}
