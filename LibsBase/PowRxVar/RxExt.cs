using System.Reactive;
using System.Reactive.Linq;

namespace PowRxVar;

public static class RxExt
{
	public static IObservable<Unit> ToUnit<T>(this IObservable<T> obs) => obs.Select(_ => Unit.Default);
}