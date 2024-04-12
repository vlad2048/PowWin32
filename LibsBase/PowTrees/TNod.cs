using System.Collections;

// ReSharper disable once CheckNamespace
public static class Nod
{
	public static TNod<T> Make<T>(T v, IEnumerable<TNod<T>>? children = null) => new(v, children);
	public static TNod<T> Make<T>(T v, params TNod<T>[] children) => new(v, children);
}


public sealed class TNod<T> : IEnumerable<TNod<T>>
{
	public T V { get; }
	public List<TNod<T>> Kids { get; } = new();

	internal TNod(T v, IEnumerable<TNod<T>>? kids)
	{
		V = v;
		if (kids != null)
			Kids.AddRange(kids);
	}

	public override string ToString()
	{
		try
		{
			var str = $"{V}";
			return str;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}


	public IEnumerator<TNod<T>> GetEnumerator() => Enumerate();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private IEnumerator<TNod<T>> Enumerate()
	{
		IEnumerable<TNod<T>> Recurse(TNod<T> node)
		{
			yield return node;
			foreach (var child in node.Kids)
			foreach (var childRes in Recurse(child))
				yield return childRes;
		}
		foreach (var res in Recurse(this))
			yield return res;
	}
}