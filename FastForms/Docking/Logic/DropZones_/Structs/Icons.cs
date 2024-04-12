/*
using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;
using PowWin32.Geom;

namespace FastForms.Docking.Logic.DropZones_.Structs;


interface IIcons
{
	IDrop[] Locs { get; }
}


sealed record Tool2DocRootIcons : IIcons
{
	private Tool2DocRootIcons() {}
	public override string ToString() => "Tool2DocRootIcons";

	public static readonly IIcons Instance = new Tool2DocRootIcons();

	public IDrop[] Locs => [new DocRoot_Init_Drop(), .. DocRoot_Side_Drop.All];
}


sealed record Tool2ToolRootIcons : IIcons
{
	public SDir SDir { get; }
	private Tool2ToolRootIcons(SDir sdir) => SDir = sdir;
	public override string ToString() => $"Tool2ToolRootIcons({SDir})";

	public static readonly IIcons[] Instances = [.. Enum.GetValues<SDir>().Select(sdir => new Tool2ToolRootIcons(sdir))];

	public IDrop[] Locs => [new ToolRoot_Side_Drop(SDir)];
}


sealed record Tool2ToolHolderIcons(ToolHolderNode Holder) : IIcons
{
	public override string ToString() => $"Tool2ToolHolderIcons({Holder})";

	public IDrop[] Locs => [new Holder_Over_Drop(Holder), .. Holder_Side_Drop.MakeAll(Holder, NodeType.Tool)];
}


sealed record Tool2DocHolderIcons(DocHolderNode Holder) : IIcons
{
	public override string ToString() => $"Tool2DocHolderIcons({Holder})";

	public IDrop[] Locs => [.. DocRoot_Side_Drop.All, new Holder_Over_Drop(Holder), .. Holder_Side_Drop.MakeAll(Holder, NodeType.Tool)];
}



sealed record Doc2DocRootIcons : IIcons
{
	private Doc2DocRootIcons() {}
	public override string ToString() => "Doc2DocRootIcons";

	public static readonly IIcons Instance = new Doc2DocRootIcons();

	public IDrop[] Locs => [new DocRoot_Init_Drop()];
}


sealed record Doc2ToolHolderIcons(ToolHolderNode Holder) : IIcons
{
	public override string ToString() => $"Doc2ToolHolderIcons({Holder})";

	public IDrop[] Locs => Holder_Side_CreateDocRoot_Drop.MakeAll(Holder);
}


sealed record Doc2DocHolderIcons(DocHolderNode Holder) : IIcons
{
	public override string ToString() => $"Doc2DocHolderIcons({Holder})";

	public IDrop[] Locs => [new Holder_Over_Drop(Holder), .. Holder_Side_Drop.MakeAll(Holder, NodeType.Doc)];
}
*/