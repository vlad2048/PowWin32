using System.Reactive;
using PowRxVar;

namespace FastForms.Utils.RxUtils;

public sealed class RxArr<T>(Disp d)
{
	private readonly IRwVar<T[]> arr = Var.Make<T[]>([], d);
	private readonly IRwVar<int> idx = Var.Make(0, d);

	public IRoVar<T[]> Arr => arr;
	public IRoVar<int> Idx => idx;
	public IObservable<Unit> WhenChanged => Var.Merge(Arr, Idx);

	public int Count => Arr.V.Length;
	public void SetIdx(int index)
	{
		Ass(index >= 0 && index < Arr.V.Length, "Wrong index");
		idx.V = index;
	}
	public void Add(params T[] items) => idx.V = arr.ArrAdd(items);
	public void Del(T item) => idx.V = arr.ArrDel(item, Idx.V);
	public void Insert(T item, int index) => idx.V = arr.ArrInsert(item, index);

	public T[] GetAndRemove()
	{
		var res = arr.V;
		arr.V = [];
		idx.V = 0;
		return res;
	}

	//public void Move(T[] itemsCopy, int idxSrc, int idxDst) => idx.V = arr.ArrMove(itemsCopy, idxSrc, idxDst);

	public void Move(int idxSrc, int idxDst) => idx.V = arr.ArrMove(idxSrc, idxDst);
}




file static class RxArrFileExt
{
	public static int ArrAdd<T>(this IRwVar<T[]> arr, T[] elts)
	{
		var list = arr.V.ToList();
		list.AddRange(elts);
		arr.V = [.. list];
		return arr.V.Length - 1;
	}

	public static int ArrDel<T>(this IRwVar<T[]> arr, T elt, int idx)
	{
		arr.V = arr.V.ArrDel(elt);
		return idx.Cap(arr);
	}

	public static int ArrInsert<T>(this IRwVar<T[]> arr, T elt, int idx)
	{
		arr.V = arr.V.ArrInsert(elt, idx);
		return idx;
	}

	public static int ArrMove<T>(this IRwVar<T[]> arr, int idxSrc, int idxDst)
	{
		arr.V = arr.V.ArrMove(idxSrc, idxDst);
		return idxDst;
	}

	/*public static int ArrMove<T>(this IRwVar<T[]> arr, T[] itemsCopy, int idxSrc, int idxDst)
	{
		arr.V = itemsCopy.ArrMove(idxSrc, idxDst);
		return idxDst;
	}*/


	private static T[] ArrDel<T>(this T[] arr, T elt)
	{
		Ass(arr.Contains(elt), "Elt not in array");
		var list = arr.ToList();
		list.Remove(elt);
		return [.. list];
	}

	private static T[] ArrInsert<T>(this T[] arr, T elt, int idx)
	{
		Ass(!arr.Contains(elt), "Elt already in array");
		var list = arr.ToList();
		list.Insert(idx, elt);
		return [.. list];
	}

	private static T[] ArrMove<T>(this T[] arr, int idxSrc, int idxDst)
	{
		if (idxDst == idxSrc) return arr;
		Ass(idxSrc >= 0 && idxDst >= 0 && idxSrc <= arr.Length && idxDst <= arr.Length, "Wrong indices");
		var list = arr.ToList();
		var elt = list[idxSrc];
		list.RemoveAt(idxSrc);
		list.Insert(idxDst, elt);
		return [.. list];
	}


	private static int Cap<T>(this int idx, IRoVar<T[]> arr) => Math.Max(0, Math.Min(arr.V.Length - 1, idx));
}
