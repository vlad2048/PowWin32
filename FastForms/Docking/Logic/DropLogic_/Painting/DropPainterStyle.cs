using System.Drawing.Imaging;
using FastForms.Docking.Logic.DropLogic_.Structs;

namespace FastForms.Docking.Logic.DropLogic_.Painting;


static class DropPainterStyle
{
	public const int GeomTabWidth = 21;
	public const int GeomMarg = 5;
	public const int InsertTabLng = 100;
	public static readonly Brush GeomInner = MkBrush(0x006CBE, 102);
	public static readonly Brush GeomOuter = MkBrush(0xFFFFFF, 102);


	private static readonly Bitmap bmpZoneSingle = Resource.dock_zone_single;
	private static readonly Bitmap bmpZoneSmall = Resource.dock_zone_small;
	private static readonly Bitmap bmpZoneBig = Resource.dock_zone_big;

	public static Bitmap? GetZoneBmp(this ZoneBmp bmp) => bmp switch
	{
		ZoneBmp.None => null,
		ZoneBmp.Single => bmpZoneSingle,
		ZoneBmp.Small => bmpZoneSmall,
		ZoneBmp.Big => bmpZoneBig,
		_ => throw new ArgumentException()
	};


	public static Brush? GetBtnBrush(this IDropBmp bmp, bool on) =>
		bmp switch
		{
			NoneDropBmp => null,
			MergeDropBmp => Btns.Merge[on],
			ToolSplitDropBmp { Dir: var dir } => Btns.ToolSplit[(int)dir][on],
			DocSplitDropBmp { Dir: var dir } => Btns.DocSplit[(int)dir][on],
			_ => throw new ArgumentException()
		};



	private static BtnBmpSet Btns => btnBmpSet.Value;

	private static readonly Lazy<BtnBmpSet> btnBmpSet = new(MkBtnBmpSet);

	private sealed record Bmp(Brush BmpOn, Brush BmpOff)
	{
		public Brush this[bool isOn] => isOn ? BmpOn : BmpOff;
	}

	private sealed record BtnBmpSet(
		Bmp Merge,
		Bmp[] ToolSplit,
		Bmp[] DocSplit
	);


	private static BtnBmpSet MkBtnBmpSet() => new(
		MkBmp(Resource.dock_hint_over),
		[
			MkBmp(Resource.dock_hint_up),
			MkBmp(Resource.dock_hint_down),
			MkBmp(Resource.dock_hint_left),
			MkBmp(Resource.dock_hint_right),
		],
		[
			MkBmp(Resource.dock_hint_doc_up),
			MkBmp(Resource.dock_hint_doc_down),
			MkBmp(Resource.dock_hint_doc_left),
			MkBmp(Resource.dock_hint_doc_right),
		]
	);

	private static Bmp MkBmp(Bitmap bmp) => new(
		new TextureBrush(bmp, new Rectangle(0, 0, 32, 32)),
		new TextureBrush(bmp, new Rectangle(0, 0, 32, 32), AttrsOff)
	);




	private static ImageAttributes AttrsOff => attrsOff.Value;



	private static readonly float[][] attrsOffColorMat = [
		[1, 0, 0, 0, 0],
		[0, 1, 0, 0, 0],
		[0, 0, 1, 0, 0],
		[0, 0, 0, 0.659f, 0],
		[0, 0, 0, 0, 0],
	];
	private static readonly Lazy<ImageAttributes> attrsOff = new(MkAttrsOff);
	private static ImageAttributes MkAttrsOff()
	{
		var attrs = new ImageAttributes();
		attrs.SetColorMatrix(new ColorMatrix(attrsOffColorMat));
		return attrs;
	}
}