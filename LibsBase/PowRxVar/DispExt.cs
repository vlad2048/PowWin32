using System.Reactive.Disposables;

namespace PowRxVar;

public static class DispExt
{
	public static T D<T>(this T v, Disp d) where T : class, IDisposable
	{
		d.Add(v);
		return v;
	}

	public static Dictionary<K, V> D<K, V>(this Dictionary<K, V> dict, Disp d) where K : notnull where V : IDisposable
	{
		Disposable.Create(dict, dict_ =>
		{
			foreach (var v in dict_.Values)
				v.Dispose();
		}).D(d);
		return dict;
	}
}