using FastForms.Docking.Logic.HolderWin_.Painting;
using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;
using PowWin32.Geom;

namespace FastForms.Docking.Logic.Layout_;

public static class NodeMaker
{
	public static TNod<INode> RootTool(TNod<INode>? kid = null) => Nod.Make<INode>(new ToolRootNode(), kid != null ? [kid] : []);
	public static TNod<INode> RootDoc(TNod<INode>? kid = null) => Nod.Make<INode>(new DocRootNode(), kid != null ? [kid] : []);

	public static TNod<INode> Split(NodeType type, Dir dir, TNod<INode> kid1, TNod<INode> kid2) => Nod.Make<INode>(new SplitNode(type, dir), [kid1, kid2]);

	public static TNod<INode> Holder(NodeType type, params Pane[] panes) => Nod.Make<INode>(HolderNode.Make(type, panes));
	public static TNod<INode> HolderJerk(NodeType type, Pane[] panes, TabLabelLay jerkLay) => Nod.Make<INode>(HolderNode.Make(type, panes, jerkLay));
}