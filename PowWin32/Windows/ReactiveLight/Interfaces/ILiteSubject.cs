namespace PowWin32.Windows.ReactiveLight.Interfaces;

public interface ILiteSubject<TSource, TResult> : ILiteObserver<TSource>, ILiteObservable<TResult>;

public interface ILiteSubject<T> : ILiteSubject<T, T>;
