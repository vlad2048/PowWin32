using PowWin32.Windows.ReactiveLight.Interfaces;

namespace PowWin32.Windows.ReactiveLight.Infra;

public abstract class LiteObserverBase<T> : ILiteObserver<T>, IDisposable
{
    private int _isStopped;
    public void OnNext(ref T value) { if (Volatile.Read(ref _isStopped) == 0) OnNextCore(ref value); }
    protected abstract void OnNextCore(ref T value);
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    protected virtual void Dispose(bool disposing) { if (!disposing) return; Volatile.Write(ref _isStopped, 1); }
}