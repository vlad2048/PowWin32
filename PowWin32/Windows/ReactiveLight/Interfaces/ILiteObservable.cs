namespace PowWin32.Windows.ReactiveLight.Interfaces;

public interface ILiteObservable<T>
{
    IDisposable Subscribe(ILiteObserver<T> observer);
}

