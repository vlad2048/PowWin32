using System.Reactive;

namespace PowRxVar;

public interface IWhenChanged
{
	IObservable<Unit> WhenChanged { get; }
}


/*
public sealed class RoDisp(Disp d)
{
	public void Add(IDisposable disposable) => d.Add(disposable);
}
*/

public interface IRoVar<out T> : IObservable<T>, IWhenChanged
{
	//RoDisp RoD { get; }
	T V { get; }
}

public interface IHasDisp : IDisposable
{
	Disp D { get; }
}

public interface IRwVar<T> : IRoVar<T>, IHasDisp
{
	new T V { get; set; }
}

/*								IObservable		WhenOuter	WhenInner
	-----------------------------------------------------------------
	x = Var.MakeBound(1, d);	1
	x.V = 2;					2				2
	x.SetInner(3);				3							3
*/
public interface IBoundVar<T> : IRwVar<T>
{
	IObservable<T> WhenOuter { get; }
	IObservable<T> WhenInner { get; }
	void SetInner(T v);
}