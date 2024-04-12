using PowBasics.CollectionsExt;
using PowWin32.Geom;
using System.Text;
using PowTrees.Geom;

// ReSharper disable once CheckNamespace
namespace PowTrees.Algorithms;

public static class SvgArrowMaker
{
	public static SvgArrowNfo Make<T>(TNod<T> tRoot, Dictionary<TNod<T>, R> layout)
	{
		var root = MakeRTree(tRoot, layout);

		var r = root.Select(e => e.V).Union();
		var orig = r.Pos;
		var sz = r.Size;
		
		var sb = new StringBuilder();
		
		void AddSvg(string str) => sb.AppendLine(str);

		void AddSvgLine(VecPt src, VecPt dst, string? markerEnd) => AddSvg($"""
			<line
				x1="{src.X.hAdj()}"
				y1="{src.Y.vAdj()}"
				x2="{dst.X.hAdj()}"
				y2="{dst.Y.vAdj()}"
				stroke={TreeArrowLineStroke}
				stroke-width={TreeArrowLineStrokeWidth}
				{(markerEnd == null ? "" : $"""marker-end="url(#{markerEnd})" """)}
			/>
		""");
		
		void DrawSingleArrow(R srcR, R dstR)
		{
			srcR -= orig;
			dstR -= orig;

			var src = srcR.ToVec();
			var dst = dstR.ToVec();
			var ptSrc = src.OnTheRight();
			var ptDstAct = dst.OnTheLeft();
			var ptDst = new VecPt(ptDstAct.X, ptSrc.Y);
			AddSvgLine(ptSrc, ptDst, ArrowName);
		}

		void DrawMultipleArrows(R srcR, R[] dstRs)
		{
			srcR -= orig;
			dstRs = dstRs.SelectToArray(e => e - orig);

			var src = srcR.ToVec();
			var dsts = dstRs.Select(e => e.ToVec()).ToArray();
			var ptSrc = src.OnTheRight();
			var ptMid = new VecPt((ptSrc.X + dsts[0].Min.X) / 2, ptSrc.Y);
			var ptDsts = dsts.Select(e => e.OnTheLeft()).ToArray();
			AddSvgLine(ptSrc, ptMid, null);
			var ptConTop = new VecPt(ptMid.X, ptDsts[0].Y);
			var ptConBottom = new VecPt(ptMid.X, ptDsts[^1].Y);
			AddSvgLine(ptConTop, ptConBottom, null);
			foreach (var ptDst in ptDsts)
			{
				var ptCon = new VecPt(ptMid.X, ptDst.Y);
				AddSvgLine(ptCon, ptDst, ArrowName);
			}
		}
		
		root.Where(e => e.Kids.Count == 1).ForEach(e => DrawSingleArrow(e.V, e.Kids[0].V));
		root.Where(e => e.Kids.Count > 1).ForEach(e => DrawMultipleArrows(e.V, e.Kids.Select(f => f.V).ToArray()));

		return new SvgArrowNfo(
			$$"""
			<defs>
				{{ArrowDef}}
			</defs>
			{{sb}}
			""",

			$"0 0 {sz.Width.h()} {sz.Height.v()}",

			$$"""
				position:		absolute;
				left:			0;
				top:			0;
				width:			{{sz.Width.h()}};
				height:			{{sz.Height.v()}};
				pointer-events:	none;
			"""
		);
	}

	private static TNod<R> MakeRTree<T>(TNod<T> root, Dictionary<TNod<T>, R> layout) => root.MapN(e => layout[e]);



	private static readonly string TreeArrowLineStroke = "#145d99";
	private static readonly string TreeArrowLineStrokeWidth = "1px";
	
	private const string ArrowName = "arrowhead";
	private static readonly string TreeArrowHeadStroke = "#145d99";
	private static readonly string TreeArrowHeadStrokeWidth = "1px";
	private static readonly string TreeArrowHeadFill = "#178ceb";
	private static readonly string ArrowDef = $"""
	<marker
		id="{ArrowName}"
		markerWidth ="10"
		markerHeight="10"
		refX="10"
		refY="5"
		orient="auto"
	>
		<polygon
			points = "4 3, 10 5, 4 7"
			stroke = {TreeArrowHeadStroke}
			stroke-width = {TreeArrowHeadStrokeWidth}
			fill = {TreeArrowHeadFill}
		/>
	</marker>
	""";
	
	private static string hAdj(this double v) => (v - 0.25).h();
	private static string vAdj(this double v) => (v - 0.25).v();
	private static string h(this int v) => $"{v}ch";
	private static string v(this int v) => $"{v}em";
	private static string h(this double v) => $"{v}ch";
	private static string v(this double v) => $"{v}em";
	private static VecR ToVec(this R r) => new(r.Pos.ToVec(), new VecPt(r.Right + 1, r.Bottom + 1));
	private static VecPt ToVec(this Pt pt) => new(pt.X, pt.Y);
	private static VecPt OnTheRight(this VecR r) => new(r.Min.X + r.Width, r.Min.Y + r.Height / 2);
	private static VecPt OnTheLeft(this VecR r) => new(r.Min.X, r.Min.Y + r.Height / 2);
}