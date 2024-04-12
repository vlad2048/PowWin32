using FastForms.Docking.Logic.DockerWin_.Structs;
using FastForms.Docking.Utils.Btns_.Structs;
using System.Drawing.Imaging;

namespace FastForms.Docking.Logic.DockerWin_.Painting;

static class DockerWinPainterStyle
{
	public static readonly Font Font = SystemFonts.CaptionFont!;

	public static readonly Brush BackBrush = MkBrush(0xEEEEF2);

	public static readonly IReadOnlyDictionary<bool, Pen> BorderPen = new Dictionary<bool, Pen>
	{
		{ false, MkPen(0xCCCEDB) },
		{ true, MkPen(0x9B9FB9) },
	};

	public static readonly IReadOnlyDictionary<bool, Bitmap> Icon = new Dictionary<bool, Bitmap>
	{
		{ false, Resource.appicon_inactive },
		{ true, Resource.appicon_active },
	};

	public static readonly IReadOnlyDictionary<bool, Color> TitleForeColor = new Dictionary<bool, Color>
	{
		{ false, MkColor(0x000000, 82) },
		{ true, MkColor(0x000000, 165) },
	};

	public static readonly Color TitleBackColor = MkColor(0xEEEEF2);


	public static readonly BtnSetStyle BtnStyle = new(
		DockerWinSysBtnBmps.Bmps,
		BtnFun,
		0
	);


	private static float[][] SysBtnInactiveNormalColorMatVals => [
		[0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0],
		[0, 0, 0, 1, 0],
		[0, 0, 0, 0, 0],
	];

	private static float[][] SysBtnInactiveHoverColorMatVals => [
		[.4f, 0, 0, 0, 0],
		[0, .4f, 0, 0, 0],
		[0, 0, .4f, 0, 0],
		[0, 0, 0, 1, 0],
		[0, 0, 0, 0, 0],
	];


	private static BtnDrawRes BtnFun(Bitmap bmp, BtnMouseState state, bool active)
	{
		Brush? backBrush = state switch
		{
			BtnMouseState.Normal => null,
			BtnMouseState.Hover => MkBrush(0xF7F7F9),
			BtnMouseState.Pressed => MkBrush(0x0E6198),
			_ => throw new ArgumentException()
		};
		ImageAttributes? attrs = state switch
		{
			BtnMouseState.Normal => MkImgAttrs(false, new ColorMatrix(SysBtnInactiveNormalColorMatVals)),
			BtnMouseState.Hover => MkImgAttrs(false, new ColorMatrix(SysBtnInactiveHoverColorMatVals)),
			_ => null
		};
		return new BtnDrawRes(bmp, backBrush, attrs);
	}
}