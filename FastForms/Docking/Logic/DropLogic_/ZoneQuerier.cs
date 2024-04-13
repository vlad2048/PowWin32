using FastForms.Docking.Enums;
using FastForms.Docking.Logic.DropLogic_.Structs;
using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Docking.Structs;
using PowWin32.Geom;
using PowWin32.Windows.Utils;

namespace FastForms.Docking.Logic.DropLogic_;


static class ZoneQuerier
{
	public static Zone[] QueryZones(this Docker dockerDst, Docker dockerSrc) =>
		Query(dockerDst, dockerSrc)
			.ToScreen();


	private static Zone[] Query(
		Docker dockerDst,
		Docker dockerSrc
	)
	{
		if (dockerSrc.Root.Count(e => e.V is HolderNode) == 0)
			return [];

		return dockerSrc.TreeType.V switch
		{
			TreeType.Tool or TreeType.ToolSingle =>
				[.. dockerDst.Root.SelectMany(node => QueryForTool(dockerDst, node))],

			TreeType.Doc =>
				[.. dockerDst.Root.SelectMany(node => QueryForDoc(dockerDst, node))],

			TreeType.Mixed =>
				[],

			_ => throw new ArgumentException()
		};
	}


	// ***************
	// ***************
	// ** For Tools **
	// ***************
	// ***************
	private static Zone[] QueryForTool(Docker dockerDst, TNod<INode> node) =>
		node.V switch
		{
			ToolRootNode when dockerDst.TreeType.V is TreeType.Tool && node.Kids.Count == 0 =>
				[MakeToolRootInitZone(dockerDst, node.V.R)],

			ToolRootNode when dockerDst.TreeType.V is TreeType.Doc or TreeType.Mixed =>
				[],
				//MakeToolRootZones(dockerDst, node.V.R),

			ToolHolderNode holder =>
				[MakeToolHolderZone(dockerDst, holder)],

			DocRootNode when node.Kids.Count == 0 =>
				[],
				//[MakeToolDocRootInitZone(dockerDst, node.V.R)],

			DocHolderNode holder =>
				[],
				//[MakeToolInDocHolderZone(dockerDst, holder, rootDst)],

			_ => [],
		};

	// ToolRootInit Zone
	// =================
	private static Zone MakeToolRootInitZone(Docker docker, R zoneR)
	{
		var bmpR = ZoneSmall.MakeBmpR(zoneR);
		return new Zone(
			docker,
			"ToolRootInit",
			zoneR,
			bmpR,
			ZoneBmp.Small,
			[
				new Drop(
					docker,
					"ToolRootInit.Init",
					ZoneSmall.MakeCenterR(bmpR),
					new MergeDropBmp(),
					Geom.ForNode(zoneR),
					new InitTarget(NodeType.Tool)
				)
			]
		);
	}

	// ToolRoot Zone
	// =============
	private static Zone[] MakeToolRootZones(Docker docker, R zoneR) =>
	[
		.. Enum.GetValues<SDir>().Select(dir =>
		{
			var bmpR = ZoneSingle.MakeSideR(zoneR, dir);
			return new Zone(
				docker,
				$"ToolRoot({dir})",
				zoneR,
				bmpR,
				ZoneBmp.Single,
				[
					new Drop(
						docker,
						$"ToolRoot({dir}).SplitRoot",
						bmpR - Marg.All(4),
						new ToolSplitDropBmp(dir),
						Geom.ForNodeSide(zoneR, dir),
						new SplitRootTarget(NodeType.Tool, dir)
					)
				]
			);
		})
	];


	// ToolHolder Zone
	// ===============
	private static Zone MakeToolHolderZone(Docker docker, ToolHolderNode holder)
	{
		var zoneR = holder.R;
		var bmpR = ZoneSmall.MakeBmpR(zoneR);
		return new Zone(
			docker,
			$"ToolHolder({zoneR})",
			zoneR,
			bmpR,
			ZoneBmp.Small,
			[
				new Drop(
					docker,
					$"ToolHolder({zoneR}).Merge",
					ZoneSmall.MakeCenterR(bmpR),
					new MergeDropBmp(),
					Geom.ForNode(zoneR),
					new MergeTarget(holder)
				),
				..Enum.GetValues<SDir>()
					.Select(dir => new Drop(
						docker,
						$"ToolHolder({zoneR}).Split({dir})",
						ZoneSmall.MakeSideR(bmpR, dir),
						new ToolSplitDropBmp(dir),
						Geom.ForNodeSide(zoneR, dir),
						new SplitTarget(holder, dir)
					)),
			]
		);
	}


	// ToolDocRootInit Zone
	// ====================
	private static Zone MakeToolDocRootInitZone(Docker dockerDst, R zoneR)
	{
		var bmpR = ZoneBig.MakeBmpR(zoneR);
		return new Zone(
			dockerDst,
			"DocRootInit",
			zoneR,
			bmpR,
			ZoneBmp.Big,
			[
				new Drop(
					dockerDst,
					"DocRootInit.Init",
					ZoneBig.MakeCenterR(bmpR),
					new MergeDropBmp(),
					Geom.ForNode(zoneR),
					new InitTarget(NodeType.Doc)
				),
				..Enum.GetValues<SDir>()
					.Select(dir => new Drop(
						dockerDst,
						$"DocRootInit.SplitRoot({dir})",
						ZoneBig.MakeFarSideR(bmpR, dir),
						new ToolSplitDropBmp(dir),
						Geom.ForNodeSide(zoneR, dir),
						new SplitRootTarget(NodeType.Doc, dir)
					)),
			]
		);
	}



	// ToolInDocHolder Zone
	// ====================
	private static Zone MakeToolInDocHolderZone(Docker dockerDst, DocHolderNode holder)
	{
		var docRoot = dockerDst.Root.First(e => e.V is DocRootNode);
		var splits = docRoot.FindSplitDirs(holder);
		SDir[] none = [];
		SDir[] dirs =
		[
			..splits.Contains(SDir.Down) ? none : [SDir.Up],
			..splits.Contains(SDir.Up) ? none : [SDir.Down],
			..splits.Contains(SDir.Left) ? none : [SDir.Right],
			..splits.Contains(SDir.Right) ? none : [SDir.Left],
		];
		var zoneR = holder.R;
		var bmpR = ZoneBig.MakeBmpR(zoneR);
		return new Zone(
			dockerDst,
			$"ToolInDoc({zoneR})",
			zoneR,
			bmpR,
			ZoneBmp.Big,
			[
				new Drop(
					dockerDst,
					$"ToolInDoc({zoneR}).Merge",
					ZoneBig.MakeCenterR(bmpR),
					new MergeDropBmp(),
					Geom.ForNode(zoneR),
					new MergeTarget(holder)
				),
				..Enum.GetValues<SDir>()
					.Select(dir => new Drop(
						dockerDst,
						$"ToolInDoc({zoneR}).Split({dir})",
						ZoneBig.MakeSideR(bmpR, dir),
						new DocSplitDropBmp(dir),
						Geom.ForNodeSide(zoneR, dir),
						new SplitTarget(holder, dir)
					)),
				..dirs.Select(dir => new Drop(
					dockerDst,
					$"ToolInDoc({zoneR}).SplitRoot({dir})",
					ZoneBig.MakeFarSideR(bmpR, dir),
					new DocSplitDropBmp(dir),
					Geom.ForNodeSide(docRoot.V.R, dir),
					new SplitRootTarget(NodeType.Doc, dir)
				))
			]
		);
	}






	// **************
	// **************
	// ** For Docs **
	// **************
	// **************
	private static Zone[] QueryForDoc(Docker dockerDst, TNod<INode> node) =>
		node.V switch
		{
			_ => []
		};

}








file static class ZoneSingle
{
	private const int Size = 40;
	private const int Marg = 4;

	public static R MakeSideR(R zoneR, SDir dir)
	{
		var (sz, mg) = (Size, Marg);
		return dir switch
		{
			SDir.Up => new(zoneR.X + (zoneR.Width - sz) / 2, zoneR.Y + mg, sz, sz),
			SDir.Down => new(zoneR.X + (zoneR.Width - sz) / 2, zoneR.Bottom - 1 - mg - sz, sz, sz),
			SDir.Left => new(zoneR.X + mg, zoneR.Y + (zoneR.Height - sz) / 2, sz, sz),
			SDir.Right => new(zoneR.Right - 1 - mg - sz, zoneR.Y + (zoneR.Height - sz) / 2, sz, sz),
			_ => throw new ArgumentException()
		};
	}
}

file static class ZoneSmall
{
	private const int Size = 112;

	public static R MakeBmpR(R zoneR) => new(zoneR.X + (zoneR.Width - Size) / 2, zoneR.Y + (zoneR.Height - Size) / 2, Size, Size);
	public static R MakeCenterR(R bmpR) => bmpR.BtnAt(40, 40);
	public static R MakeSideR(R bmpR, SDir dir) =>
		dir switch
		{
			SDir.Up => bmpR.BtnAt(40, 4),
			SDir.Down => bmpR.BtnAt(40, 76),
			SDir.Left => bmpR.BtnAt(4, 40),
			SDir.Right => bmpR.BtnAt(76, 40),
			_ => throw new ArgumentException()
		};
}

file static class ZoneBig
{
	private const int Size = 184;

	public static R MakeBmpR(R zoneR) => new(zoneR.X + (zoneR.Width - Size) / 2, zoneR.Y + (zoneR.Height - Size) / 2, Size, Size);
	public static R MakeCenterR(R bmpR) => bmpR.BtnAt(76, 76);
	public static R MakeSideR(R bmpR, SDir dir) =>
		dir switch
		{
			SDir.Up => bmpR.BtnAt(76, 40),
			SDir.Down => bmpR.BtnAt(76, 112),
			SDir.Left => bmpR.BtnAt(40, 76),
			SDir.Right => bmpR.BtnAt(112, 76),
			_ => throw new ArgumentException()
		};
	public static R MakeFarSideR(R bmpR, SDir dir) =>
		dir switch
		{
			SDir.Up => bmpR.BtnAt(76, 4),
			SDir.Down => bmpR.BtnAt(76, 148),
			SDir.Left => bmpR.BtnAt(4, 76),
			SDir.Right => bmpR.BtnAt(148, 76),
			_ => throw new ArgumentException()
		};
}


file static class ZoneQuerierFileExt
{
	private const int BtnSize = 32;

	public static R BtnAt(this R r, int x, int y) => new(r.X + x, r.Y + y, BtnSize, BtnSize);
}



file static class ZoneFileExt
{
	public static Zone[] ToScreen(this Zone[] zones) => [..zones.Select(zone => zone.ToScreen(zone.Docker))];

	private static Zone ToScreen(this Zone zone, Docker docker) =>
		zone with {
			ZoneR = zone.ZoneR.ToScreen(docker),
			BmpR = zone.BmpR.ToScreen(docker),
			Drops = [..zone.Drops.Select(drop =>
				drop with
				{
					R = drop.R.ToScreen(docker),
					Geom = drop.Geom.ToScreen(docker),
				}
			)]
		};


	/*private static Zone ToScreen(this Zone zone, Docker docker) => new(
		zone.Docker,
		zone.Id,
		zone.ZoneR.ToScreen(docker),
		zone.BmpR.ToScreen(docker),
		zone.Bmp,
		[..zone.Drops.Select(drop =>
			drop with
			{
				R = drop.R.ToScreen(docker),
				Geom = drop.Geom.ToScreen(docker),
			}
		)]
	);*/



	private static Geom ToScreen(this Geom e, Docker docker) => e with { R = e.R.ToScreen(docker) };
	private static RSet ToScreen(this RSet r, Docker docker) => new([.. r.Rs.Select(e => e.ToScreen(docker))]);
	private static R ToScreen(this R r, Docker docker) => docker.Sys.Client2Screen(r);
}



file static class TreeExt
{
	public static HashSet<SDir> FindSplitDirs<T>(this TNod<T> root, T node)
	{
		if (root.V!.Equals(node)) return [];

		var set = new HashSet<SDir>();
		var cur = node;

		while (true)
		{
			var curDad = root.FindDad(cur);
			if (curDad.V!.Equals(root.V)) break;

			if (curDad is not SplitNode split) throw new ArgumentException("Should be a SplitNode");

			var isFirst = curDad.Kids[0].V!.Equals(cur);

			var dir = split.Dir switch
			{
				Dir.Horz => isFirst ? SDir.Left : SDir.Right,
				Dir.Vert => isFirst ? SDir.Up : SDir.Down,
				_ => throw new ArgumentException()
			};
			set.Add(dir);
			
			cur = curDad.V;
		}

		return set;
	}

	private static TNod<T> FindDad<T>(this TNod<T> root, T kid) => root.First(e => e.Kids.Any(f => f.V!.Equals(kid)));
}
