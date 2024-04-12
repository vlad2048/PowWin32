using PowBasics.CollectionsExt;
using PowWin32.Geom;
using PowTrees.Algorithms.Layout.Exts;

// ReSharper disable once CheckNamespace
namespace PowTrees.Algorithms;


public static class Algo_Layout
{
	public static GraphLayout<T> BuildLayout<T>(
		this TNod<T> root,
		Func<T, Sz> szFun,
		Action<AlgoLayoutOpt>? optFun = null
	)
	{
		var dupRoot = root.Dup();
		return new GraphLayout<T>(
			dupRoot
				.Layout(e => szFun(e.Orig), optFun)
				.ZipMapWithTree(dupRoot, (dupNode, r) => new LayoutNode<T>(dupNode.Orig, r))
		);
	}


	public static Dictionary<TNod<T>, R> Layout<T>(
		this TNod<T> root,
		Func<T, Sz> szFun,
		Action<AlgoLayoutOpt>? optFun = null
	)
	{
		var opt = AlgoLayoutOpt.Make(optFun);

		var rootSz = root.Map(e => szFun(e).MakeBigger(opt.GutterSz));
		var mapBack = rootSz.Zip(root).ToDictionary(e => e.First, e => e.Second);

		var xs = rootSz.SolveXs(opt.AlignLevels);
		var ys = rootSz.SolveYs();

		return rootSz.ToDictionary(
			nodSz => mapBack[nodSz],
			nodSz => new R(
				new Pt(
					xs[nodSz],
					ys[nodSz]
				),
				nodSz.V.MakeSmaller(opt.GutterSz)
			)
		);
	}


	private static Dictionary<TNod<Sz>, int> SolveXs(this TNod<Sz> rootSz, bool alignLevels) =>
		rootSz
			.MapN(sz => sz.V.Width)
			.MapAlignIf(alignLevels, EnumExt.MaxOrZero)
			.MapBack(rootSz, out var mapBack)
			.FoldLDictN<int, int>(
				(n, w) => n + w
			)
			.ShiftTreeMapDown(0)
			.MapKey(e => mapBack[e]);




	private static Dictionary<TNod<Sz>, int> SolveYs(this TNod<Sz> rootSz)
	{
		var hcMap = rootSz.FoldRDictN<Sz, int>(
				(n, hcs) => Math.Max(n.Height, hcs.SumOrZero())
			)
			.ShiftTreeMapUp(hcs => hcs.SumOrZero());

		var ys = new Dictionary<TNod<Sz>, int>();

		int Recurse(TNod<Sz> nod, int y)
		{
			// "height of nod"
			var hn = nod.V.Height;

			// "height of children"
			var hc = hcMap[nod];

			// "total height" of nod and its children
			var ht = Math.Max(hn, hc);

			// Layout nod and its children within [y .. y + ht]
			// ================================================
			// (1) ht >= hn     if ht > hn => shift nod      to be in the center of ht
			// (2) ht >= hc     if ht > hc => shift children to be in the center of ht

			// (1)
			ys[nod] = y + (ht - hn) / 2;	// => RESULT

			// (2)
			y += (ht - hc) / 2;
			foreach (var c in nod.Kids)
				y += Recurse(c, y);

			return ht;
		}

		Recurse(rootSz, 0);

		return ys;
	}


	

	private static Dictionary<TNod<T>, U> ShiftTreeMapUp<T, U>(
		this Dictionary<TNod<T>, U> map,
		Func<IEnumerable<U>, U> combFun
	)
	{
		var resMap = new Dictionary<TNod<T>, U>();
		foreach (var (n, _) in map)
		{
			resMap[n] = combFun(n.Kids.Select(e => map[e]));
		}
		return resMap;
	}


	private static Dictionary<TNod<T>, U> ShiftTreeMapDown<T, U>(
		this Dictionary<TNod<T>, U> map,
		U rootVal
	)
	{
		var dadMap = map.Keys.GetDadMap();
		TNod<T>? GetDad(TNod<T> nod) => dadMap.TryGetValue(nod, out var dad) switch {
			true => dad,
			false => null
		};

		var resMap = new Dictionary<TNod<T>, U>();
		foreach (var (n, _) in map)
		{
			resMap[n] = GetDad(n) switch
			{
				null => rootVal,
				not null => map[GetDad(n)!],
			};
		}
		return resMap;
	}


	private static IReadOnlyDictionary<TNod<T>, TNod<T>?> GetDadMap<T>(this IEnumerable<TNod<T>> nodes)
	{
		var map = new Dictionary<TNod<T>, TNod<T>?>();
		foreach (var node in nodes)
		{
			foreach (var kid in node.Kids)
				map[kid] = node;
		}
		return map;
	}




	private static TNod<T> MapAlignIf<T>(
		this TNod<T> root,
		bool condition,
		Func<IEnumerable<T>, T> alignFun
	) => condition switch
	{
		false => root,
		true => root.MapAlign(alignFun)
	};

	private static TNod<T> MapAlign<T>(
		this TNod<T> root,
		Func<IEnumerable<T>, T> alignFun
	)
	{
		var levelArr = root.GetNodesByLevels();
		var levelValues = levelArr.SelectToArray(ns => alignFun(ns.Select(e => e.V)));
		var levelMap = GetNodeLevelMap(levelArr);
		return root
			.MapN(n => levelValues[levelMap[n]]);
	}

	private static Dictionary<TNod<T>, int> GetNodeLevelMap<T>(TNod<T>[][] levelArr)
	{
		var map = new Dictionary<TNod<T>, int>();
		for (var level = 0; level < levelArr.Length; level++)
		{
			var levelNods = levelArr[level];
			foreach (var nod in levelNods)
				map[nod] = level;
		}
		return map;
	}

	private static TNod<TDst> MapBack<TSrc, TDst>(this TNod<TDst> rootDst, TNod<TSrc> rootSrc, out Dictionary<TNod<TDst>, TNod<TSrc>> map)
	{
		map = rootDst.Zip(rootSrc).ToDictionary(e => e.First, e => e.Second);
		return rootDst;
	}

	private static Dictionary<L, V> MapKey<K, L, V>(this Dictionary<K, V> dict, Func<K, L> mapFun) where K : notnull where L : notnull
	{
		var dictRes = new Dictionary<L, V>();
		foreach (var (k, v) in dict)
			dictRes[mapFun(k)] = v;
		return dictRes;
	}



	private static Dictionary<TNod<T>, U> FoldLDictN<T, U>(
		this TNod<T> root,
		Func<T, U?, U> fun
	)
		=>
			root.Zip(root.FoldL(fun))
				.ToDictionary(
					e => e.First,
					e => e.Second.V
				);

	private static TNod<U> FoldL<T, U>(
		this TNod<T> root,
		Func<T, U?, U> fun
	)
	{
		TNod<U> Recurse(TNod<T> node, U? mayMappedParentVal)
		{
			var mappedNodeVal = fun(node.V, mayMappedParentVal);
			var mappedChildren = node.Kids.Select(child => Recurse(child, mappedNodeVal));
			var mappedNode = Nod.Make(mappedNodeVal, mappedChildren);
			return mappedNode;
		}

		return Recurse(root, default);		
	}
}