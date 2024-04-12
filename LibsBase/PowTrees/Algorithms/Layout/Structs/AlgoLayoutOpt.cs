using PowWin32.Geom;

// ReSharper disable once CheckNamespace
namespace PowTrees.Algorithms;


public sealed class AlgoLayoutOpt
{
	public Sz GutterSz { get; set; } = new(3, 1);
	public bool AlignLevels { get; set; } = true;

	private AlgoLayoutOpt() { }

	internal static AlgoLayoutOpt Make(Action<AlgoLayoutOpt>? optFun)
	{
		var opt = new AlgoLayoutOpt();
		optFun?.Invoke(opt);
		return opt;
	}
}