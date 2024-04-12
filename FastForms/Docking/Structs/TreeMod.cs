using FastForms.Docking.Logic.Layout_.Nodes;
using PowWin32.Geom;

namespace FastForms.Docking.Structs;

enum TreeModType
{
    Layout,
    PaintOnly
}

sealed record TreeMod(
	TreeModType Type,
	TNod<INode> Nod,
	R? NodR,
	HolderNode[] HolderAdds
)
{
	public static TreeMod MakeInit(TNod<INode> root, R clientR) => new(TreeModType.Layout, root, clientR, [.. root.Select(e => e.V).OfType<HolderNode>()]);

	public static TreeMod Make(TNod<INode> nod) => new(TreeModType.Layout, nod, null, []);
}