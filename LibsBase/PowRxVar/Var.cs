using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace PowRxVar;

public static class Var
{
	public static IRwVar<T> Make<T>(T init, Disp d) => new RwVar<T>(init, d);
	public static IRoVar<T> Make<T>(T init, IObservable<T> obs, Disp d)
	{
		var rxVar = new RwVar<T>(init, d);
		obs.Subscribe(v => rxVar.V = v).D(d);
		return rxVar;
	}

	public static IRoVar<T> MakeConst<T>(T val) => Obs.Return(val).ToVar();
	public static IBoundVar<T> MakeBound<T>(T init, Disp d) => new BoundVar<T>(init, d);



	public static IRoVar<T> ToVar<T>(this IObservable<T> obs, Disp d) => new RoVar<T>(obs.MakeReplay(d));


	public static IObservable<T> MakeReplay<T>(this IObservable<T> src, Disp d)
	{
		var srcConn = src.Replay(1);
		srcConn.Connect().D(d);
		return srcConn;
	}

	public static IObservable<T> MakeHot<T>(this IObservable<T> src, Disp d)
	{
		var srcConn = src.Publish();
		srcConn.Connect().D(d);
		return srcConn;
	}

	public static IObservable<Unit> Merge(params IWhenChanged[] srcs) => srcs.Select(e => e.WhenChanged).Merge();



	private static IRoVar<T> ToVar<T>(this IObservable<T> obs) => new RoVar<T>(obs.Replay(1).RefCount());



	private sealed class RoVar<T>(IObservable<T> obs) : IRoVar<T>
	{
		public IDisposable Subscribe(IObserver<T> observer) => obs.Subscribe(observer);
		public T V => Task.Run(async () => await obs.FirstAsync()).Result;

		public IObservable<Unit> WhenChanged => this.ToUnit();
	}


	private sealed class RwVar<T>(T init, Disp d) : IRwVar<T>
	{
		private readonly BehaviorSubject<T> subj = new BehaviorSubject<T>(init).D(d);

		public Disp D { get; } = d;
		public void Dispose() => D.Dispose();
		public IDisposable Subscribe(IObserver<T> observer) => subj.Subscribe(observer);

		public T V
		{
			get => subj.Value;
			set
			{
				if (value != null && value.Equals(subj.Value)) return;
				subj.OnNext(value);
			}
		}

		public IObservable<Unit> WhenChanged => this.ToUnit();
	}


	private sealed class BoundVar<T> : IBoundVar<T>
	{
		private enum UpdateType { Inner, Outer };
		private sealed record Update(UpdateType Type, T Val);

		public Disp D { get; }
		public void Dispose() => D.Dispose();

		private readonly BehaviorSubject<T> subj;
		private readonly Subject<Update> whenUpdate;
		private IObservable<Update> WhenUpdate { get; }

		// IRoVar<T>
		// =========
		public IDisposable Subscribe(IObserver<T> observer) => subj.Subscribe(observer);

		// IRwVar<T>
		// =========
		public T V
		{
			get => subj.Value;
			set => SetOuter(value);
		}

		// IBoundVar<T>
		// ============
		public IObservable<T> WhenOuter => WhenUpdate.Where(e => e.Type == UpdateType.Outer).Select(e => e.Val);
		public IObservable<T> WhenInner => WhenUpdate.Where(e => e.Type == UpdateType.Inner).Select(e => e.Val);
		public void SetInner(T v) => whenUpdate.OnNext(new Update(UpdateType.Inner, v));
		private void SetOuter(T v) => whenUpdate.OnNext(new Update(UpdateType.Outer, v));

		public IObservable<Unit> WhenChanged => this.ToUnit();

		public BoundVar(T init, Disp d)
		{
			D = d;
			subj = new BehaviorSubject<T>(init).D(d);
			whenUpdate = new Subject<Update>().D(d);
			WhenUpdate = whenUpdate.AsObservable();
			WhenUpdate.Subscribe(e => subj.OnNext(e.Val)).D(d);
		}
	}
}