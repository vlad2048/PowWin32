using PowWin32.Windows.ReactiveLight.Interfaces;

namespace PowWin32.Windows.ReactiveLight.Infra;

public abstract class LiteSubjectBase<T> : ILiteSubject<T>, IDisposable
{
    public abstract bool HasObservers { get; }
    public abstract bool IsDisposed { get; }
    public abstract void Dispose();
    public abstract void OnNext(ref T value);
    public abstract IDisposable Subscribe(ILiteObserver<T> observer);
}