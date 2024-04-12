using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using FastForms.Docking.Logic.HolderWin_.Structs;
using FastForms.Docking.Utils.Btns_.Structs;

namespace FastForms.Docking.Logic.HolderWin_.Painting;

static class HolderWinPainterStyle
{
	public static readonly Font Font = SystemFonts.CaptionFont!;

	public static readonly Color HolderFrameColor = MkColor(0xEEEEF2);
	public static readonly Color HolderSelectedTabColor = MkColor(0xF5F5F5);
	public static readonly Color HolderHoveredTabColor = MkColor(0xC9DEF5);

	public static readonly Brush HolderFrameBrush = MkBrush(HolderFrameColor);
	public static readonly Brush HolderSelectedTabBrush = MkBrush(HolderSelectedTabColor);
	public static readonly Brush HolderHoveredTabBrush = MkBrush(HolderHoveredTabColor);

	public static readonly Pen Border = MkPen(0xCCCEDB);
	public static readonly Pen BorderFloatingAndActive = MkPen(0x9B9FB9);

	public static readonly IReadOnlyDictionary<bool, Color> CaptionBackColor = new Dictionary<bool, Color>
		{
			{ false, HolderFrameColor },
			{ true, MkColor(0x006CBE) },
		};

	public static readonly IReadOnlyDictionary<bool, Brush> CaptionBackBrush = new Dictionary<bool, Brush>
		{
			{ false, new SolidBrush(CaptionBackColor[false]) },
			{ true, new SolidBrush(CaptionBackColor[true]) },
		};

	public static readonly IReadOnlyDictionary<bool, Color> TitleColor = new Dictionary<bool, Color>
		{
			{ false, MkColor(0x1E1E1E, 224) },
			{ true, MkColor(0xFFFFFF) },
		};


	// ****************
	// * Tile Pattern *
	// ****************
	public static readonly IReadOnlyDictionary<bool, Brush> TilePatternBrush = new Dictionary<bool, Brush>
		{
			{ false, MkTilePatternBrush(false) },
			{ true, MkTilePatternBrush(true) },
		};

	private static Brush MkTilePatternBrush(bool active)
	{
		var attrs = new ImageAttributes();
		attrs.SetWrapMode(WrapMode.Tile);
		if (!active)
		{
			var colMat = new ColorMatrix([
				[0, 0, 0, 0, 0],
				[0, 0, 0, 0, 0],
				[0, 0, 0, 0, 0],
				[0, 0, 0, 1, 0],
				[0, 0, 0, 0, 0],
			]);
			attrs.SetColorMatrix(colMat);
		}
		var bmp = Resource.holderframe_tilepattern;
		var dstRect = new Rectangle(0, 0, bmp.Width, bmp.Height);
		return new TextureBrush(bmp, dstRect, attrs);
	}


	// ***************
	// * Sys Buttons *
	// ***************
	private static readonly float[][] SysBtnInactiveNormalColorMatVals = [
		[0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0],
		[0, 0, 0, 1, 0],
		[0, 0, 0, 0, 0],
	];

	private static readonly float[][] SysBtnInactiveHoverColorMatVals = [
		[.4f, 0, 0, 0, 0],
		[0, .4f, 0, 0, 0],
		[0, 0, .4f, 0, 0],
		[0, 0, 0, 1, 0],
		[0, 0, 0, 0, 0],
	];

	public static readonly BtnSetStyle BtnStyle = new(
		HolderBtnBmps.Bmps,
		BtnFun,
		1
	);


	private static BtnDrawRes BtnFun(Bitmap bmp, BtnMouseState state, bool active)
	{
		Brush? backBrush = (active, state) switch
		{
			(false, BtnMouseState.Normal) => null,
			(false, BtnMouseState.Hover) => MkBrush(0xF7F7F9),
			(false, BtnMouseState.Pressed) => MkBrush(0), // impossible
			(true, BtnMouseState.Normal) => MkBrush(0x006CBE),
			(true, BtnMouseState.Hover) => MkBrush(0x52B0EF),
			(true, BtnMouseState.Pressed) => MkBrush(0x0E6198),
			_ => throw new ArgumentException()
		};
		ImageAttributes? attrs = (active, state) switch
		{
			(false, BtnMouseState.Normal) => MkImgAttrs(false, new ColorMatrix(SysBtnInactiveNormalColorMatVals)),
			(false, BtnMouseState.Hover) => MkImgAttrs(false, new ColorMatrix(SysBtnInactiveHoverColorMatVals)),
			_ => null
		};
		return new BtnDrawRes(bmp, backBrush, attrs);
	}


	// ********
	// * Tabs *
	// ********
	public static readonly IReadOnlyDictionary<bool, Color> TabLabelTextColor = new Dictionary<bool, Color>
	{
		{ false, MkColor(0x1E1E1E, 224) },
		{ true, MkColor(0x0E70C0) },
	};
}
