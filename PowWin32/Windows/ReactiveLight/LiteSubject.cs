using System.Reactive.Disposables;
using PowWin32.Windows.ReactiveLight.Infra;
using PowWin32.Windows.ReactiveLight.Interfaces;

namespace PowWin32.Windows.ReactiveLight;

public sealed class LiteSubject<T> : LiteSubjectBase<T>
{
	private SubjectDisposable[] _observers = Array.Empty<SubjectDisposable>();
	// ReSharper disable once UseArrayEmptyMethod
	private static readonly SubjectDisposable[] Disposed = new SubjectDisposable[0];
	public override bool HasObservers => Volatile.Read(ref _observers).Length != 0;
	public override bool IsDisposed => Volatile.Read(ref _observers) == Disposed;
	public override void Dispose() => Interlocked.Exchange(ref _observers, Disposed);
	public override void OnNext(ref T value)
	{
		var observers = Volatile.Read(ref _observers);
		if (observers == Disposed) { ThrowDisposed(); return; }
		foreach (var observer in observers)
			observer.Observer?.OnNext(ref value);
	}

	public override IDisposable Subscribe(ILiteObserver<T> observer)
	{
		if (observer == null) throw new ArgumentNullException(nameof(observer));
		var disposable = default(SubjectDisposable);
		for (; ; )
		{
			var observers = Volatile.Read(ref _observers);
			if (observers == Disposed) { ThrowDisposed(); break; }
			disposable ??= new SubjectDisposable(this, observer);
			var n = observers.Length;
			var b = new SubjectDisposable[n + 1];
			Array.Copy(observers, 0, b, 0, n);
			b[n] = disposable;
			if (Interlocked.CompareExchange(ref _observers, b, observers) == observers)
				return disposable;
		}

		return Disposable.Empty;
	}

	private void Unsubscribe(SubjectDisposable observer)
	{
		for (; ; )
		{
			var a = Volatile.Read(ref _observers);
			var n = a.Length;
			if (n == 0) break;
			var j = Array.IndexOf(a, observer);
			if (j < 0) break;
			SubjectDisposable[] b;
			if (n == 1)
			{
				b = Array.Empty<SubjectDisposable>();
			}
			else
			{
				b = new SubjectDisposable[n - 1];
				Array.Copy(a, 0, b, 0, j);
				Array.Copy(a, j + 1, b, j, n - j - 1);
			}
			if (Interlocked.CompareExchange(ref _observers, b, a) == a) break;
		}
	}

	private static void ThrowDisposed() => throw new ObjectDisposedException(string.Empty);

	private sealed class SubjectDisposable : IDisposable
	{
		private LiteSubject<T> _subject;
		private volatile ILiteObserver<T>? _observer;

		public SubjectDisposable(LiteSubject<T> subject, ILiteObserver<T> observer)
		{
			_subject = subject;
			_observer = observer;
		}

		public ILiteObserver<T>? Observer => _observer;

		public void Dispose()
		{
			var observer = Interlocked.Exchange(ref _observer, null);
			if (observer == null)
			{
				return;
			}

			_subject.Unsubscribe(this);
			_subject = null!;
		}
	}

}