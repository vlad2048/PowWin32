/*
using PowBasics.CollectionsExt;
using PowRxVar;

namespace FastForms.Utils;

static class ArrExt
{
	public static int ArrAdd<T>(this IRwVar<T[]> arr, params T[] elts)
	{
		var list = arr.V.ToList();
		list.AddRange(elts);
		arr.V = [..list];
		return arr.V.Length - 1;
	}

	public static int ArrDel<T>(this IRwVar<T[]> arr, T elt)
	{
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
		var idx = arr.V.IndexOf(elt);
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
		arr.V = arr.V.ArrDel(elt);
		return idx.Cap(arr);
	}

	public static int ArrInsert<T>(this IRwVar<T[]> arr, T elt, int? idx)
	{
		var idxVal = idx ?? arr.V.Length;
		arr.V = arr.V.ArrInsert(elt, idxVal);
		return idxVal;
	}

	
	public static T[] ArrMove<T>(this T[] arr, int idxSrc, int idxDst)
	{
		if (idxDst == idxSrc) return arr;
		Ass(idxSrc >= 0 && idxDst >= 0 && idxSrc <= arr.Length && idxDst <= arr.Length, "Wrong indices");
		var list = arr.ToList();
		var elt = list[idxSrc];
		list.RemoveAt(idxSrc);
		list.Insert(idxDst, elt);
		return [.. list];
	}




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

	
	private static int Cap<T>(this int idx, IRoVar<T[]> arr) => Math.Max(0, Math.Min(arr.V.Length - 1, idx));
}
*/