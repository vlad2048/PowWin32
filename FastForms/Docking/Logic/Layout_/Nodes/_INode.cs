using FastForms.Docking.Logic.Layout_.Enums;
using PowBasics.CollectionsExt;
using PowBasics.StringsExt;
using PowTrees.Algorithms;
using PowWin32.Geom;

namespace FastForms.Docking.Logic.Layout_.Nodes;


public interface INode
{
	NodeType Type { get; }
	R R { get; set; }
}



public static class NodeExt
{
	public static void Log(this TNod<INode> node) => L(
		node.Log(opt =>
		{
			opt.GutterSz = new(5, 2);
			opt.FmtFun = e => $" [{e}] ";
		}).JoinLines()
	);
}