using System.Drawing;
using PowWin32.Geom;

// ReSharper disable once CheckNamespace
namespace PowTrees.Algorithms;

public sealed class TreeLogColoredOpt
{
	public Sz GutterSz { get; set; } = new(3, 1);
	public bool AlignLevels { get; set; } = true;
	public Color ArrowColor { get; set; } = Color.DarkGray;

	private TreeLogColoredOpt() { }

	internal static TreeLogColoredOpt Make(Action<TreeLogColoredOpt>? optFun)
	{
		var opt = new TreeLogColoredOpt();
		optFun?.Invoke(opt);
		return opt;
	}
}