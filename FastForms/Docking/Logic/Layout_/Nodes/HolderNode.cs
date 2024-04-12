using DynamicData;
using FastForms.Docking.Logic.HolderWin_;
using FastForms.Docking.Logic.HolderWin_.Painting;
using FastForms.Docking.Logic.HolderWin_.Structs;
using FastForms.Docking.Logic.Layout_.Enums;
using PowRxVar;
using PowWin32.Geom;

namespace FastForms.Docking.Logic.Layout_.Nodes;

public abstract class HolderNode : INode
{
	public Disp D { get; } = new(nameof(HolderNode));

	public NodeType Type { get; }
	public R R { get; set; }
	public HolderState State { get; }

	protected HolderNode(NodeType type, Pane[] panes, TabLabelLay? jerkLay)
	{
		Ass(!(type is NodeType.Tool && panes.Any(e => e.Type == NodeType.Doc)), "Cannot put a DocPane in a ToolHolder");
		Type = type;
		State = new HolderState(this, panes, jerkLay, D);
	}

	public override string ToString() => $"Holder  r:{R}";



	public static HolderNode Make(NodeType type, Pane[] panes, TabLabelLay? jerkLay = null) =>
		type switch
		{
			NodeType.Tool => new ToolHolderNode(panes, jerkLay),
			NodeType.Doc => new DocHolderNode(panes),
			_ => throw new ArgumentException()
		};
}

sealed class ToolHolderNode(Pane[] panes, TabLabelLay? jerkLay) : HolderNode(NodeType.Tool, panes, jerkLay);
sealed class DocHolderNode(Pane[] panes) : HolderNode(NodeType.Doc, panes, null);




/*
public abstract class HolderNode : INode, IDisposable
{
	public Disp D { get; } = new(nameof(HolderNode));
	public void Dispose() => D.Dispose();

	public NodeType Type { get; }
	public R R { get; set; }
	public HolderState State { get; }
	public HolderWin Win { get; }

	public override string ToString() => $"Holder  r:{R}";

	protected HolderNode(NodeType type, Pane[] panes, TabLabelLay? jerkLay)
	{
		Ass(!(type is NodeType.Tool && panes.Any(e => e.Type == NodeType.Doc)), "Cannot put a DocPane in a ToolHolder");
		Type = type;

		State = new HolderState(panes, jerkLay, D);
		Win = new HolderWin(Type, State);
	}

	public static HolderNode Make(NodeType type, Pane[] panes, TabLabelLay? jerkLay = null) =>
		type switch
		{
			NodeType.Tool => new ToolHolderNode(panes, jerkLay),
			NodeType.Doc => new DocHolderNode(panes),
			_ => throw new ArgumentException()
		};

	public void Realize(Docker docker) => Win.Realize(docker, R);
}

sealed class ToolHolderNode(Pane[] panes, TabLabelLay? jerkLay) : HolderNode(NodeType.Tool, panes, jerkLay);
sealed class DocHolderNode(Pane[] panes) : HolderNode(NodeType.Doc, panes, null);
*/