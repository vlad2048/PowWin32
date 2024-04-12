using System.Reactive;
using PowRxVar;
using PowWin32.Windows.ReactiveLight.Interfaces;
using PowWin32.Windows.ReactiveLight.Infra;

namespace PowWin32.Windows.ReactiveLight;

public static class ObsLite
{
	public static IObservable<T> Merge<T>(params ILiteObservable<T>[] sources) => Obs.Create<T>(obs =>
	{
		var d = new Disp("RxLiteExt.Merge");
		foreach (var source in sources)
			source.Subs((ref T e) => obs.OnNext(e)).D(d);
		return d;
	});
}

public static class RxLiteExt
{
	public static IDisposable Subs<T>(this ILiteObservable<T> source, ActionRef<T> onNext) => source.Subscribe(new AnonymousLiteObserver<T>(onNext));


	public static ILiteObservable<T> Where<T>(this ILiteObservable<T> source, Func<T, bool> predicate) => Create<T>(obs => source.Subs((ref T e) => {
		if (predicate(e))
			obs.OnNext(ref e);
	}));


	public static IObservable<Unit> ToUnit<T>(this ILiteObservable<T> source) => source.Select(_ => Unit.Default);

	public static IObservable<U> Select<T, U>(this ILiteObservable<T> source, Func<T, U> fun) => Obs.Create<U>(obs => source.Subs((ref T e) =>
	{
		var f = fun(e);
		obs.OnNext(f);
	}));

	public static IObservable<T> ToObs<T>(this ILiteObservable<T> source) => source.Select(e => e);


	private static ILiteObservable<T> Create<T>(Func<ILiteObserver<T>, IDisposable> obsFun) => new AnonymousLiteObservable<T>(obsFun);

	private sealed class AnonymousLiteObservable<T>(Func<ILiteObserver<T>, IDisposable> fun) : ILiteObservable<T>
	{
		public IDisposable Subscribe(ILiteObserver<T> observer) => fun(observer);
	}

	private sealed class AnonymousLiteObserver<T> : LiteObserverBase<T>
	{
		private readonly ActionRef<T> _onNext;
		public AnonymousLiteObserver(ActionRef<T> onNext) => _onNext = onNext;
		protected override void OnNextCore(ref T value) => _onNext(ref value);
	}
}