namespace PowWin32.Windows.ReactiveLight.Interfaces;

public interface ILiteObserver<T>
{
    void OnNext(ref T value);
}