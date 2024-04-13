using PowWin32.Geom;
using FastForms.Utils.GdiUtils;
using FastForms.Docking.Utils.Btns_;
using FastForms.Docking.Logic.HolderWin_.Structs;
using Style = FastForms.Docking.Logic.HolderWin_.Painting.HolderWinPainterStyle;
using FastForms.Structs;

namespace FastForms.Docking.Logic.HolderWin_.Painting;


static class HolderWinPainter
{
    public static void Paint(
        Graphics gfx,
        R realR,
        R r,
        string title,
        bool active,
        bool isRoot,
        BtnSet<HolderBtnId> btns,
        string[] tabNames,
        int activeTabIndex,
        int? hoveredTabIndex,
        TabLabelLay? jerkLay
    )
    {
        AssMsg(tabNames.Length >= 1, "Invalid Pane number");

        PaintBorder(gfx, realR, isRoot);
        PaintTitleBar(gfx, r, title, active, btns.VisibleBtnCount);
        btns.Paint(gfx, active);
        PaintTabs(gfx, r, tabNames, activeTabIndex, hoveredTabIndex, jerkLay);
    }

    internal static void PaintBorder(Graphics gfx, R realR, bool isRoot) => gfx.DrawRect(realR, Style.Border, isRoot ? Side.Up : Side.All);

	internal static void PaintTitleBar(
        Graphics gfx,
        R r,
        string title,
        bool active,
        int btnCount
    )
    {
        gfx.FillRect(new R(r.X, r.Y, r.Width, HolderLayout.CaptionHeight), Style.CaptionBackBrush[active]);
        var titleSz = gfx.DrawTextMeasure(r.Pos + new Pt(4, 3), title, Style.Font, Style.TitleColor[active], Style.CaptionBackColor[active]);
        gfx.FillRectPattern(r.Pos + GetTileR(r.Width, titleSz, btnCount), Style.TilePatternBrush[active]);
    }


    internal static void PaintTabs(
        Graphics gfx,
        R r,
        string[] tabNames,
        int activeTabIndex,
        int? hoveredTabIndex,
        TabLabelLay? jerkLay
	)
    {
        if (tabNames.Length <= 1 && jerkLay == null) return;

        var rowR = HolderLayout.GetTabsRowR(r);
        gfx.FillRect(rowR, Style.HolderFrameBrush);

        var tabsVertBorders = HolderLayout.GetTabsVertBorders(r);
        gfx.DrawRect(tabsVertBorders, Style.Border, Side.Left | Side.Right);

        var lays = jerkLay switch
        {
	        null => HolderLayout.GetTabLabelRs(r, tabNames),
            not null => [jerkLay.Value]
        };
        for (var i = 0; i < tabNames.Length; i++)
        {
            var tabName = tabNames[i];
            var lay = lays[i];
            var isActive = i == activeTabIndex;
            var isHovered = i == hoveredTabIndex;
            var brush = (isActive, isHovered) switch
            {
                (true, _) => Style.HolderSelectedTabBrush,
                (false, true) => Style.HolderHoveredTabBrush,
                _ => Style.HolderFrameBrush
            };
            var textBackColor = (isActive, isHovered) switch
            {
                (true, _) => Style.HolderSelectedTabColor,
                (false, true) => Style.HolderHoveredTabColor,
                _ => Style.HolderFrameColor
            };
            var textForeColor = Style.TabLabelTextColor[isActive];

            gfx.FillRect(lay.R, brush);

            gfx.DrawRect(lay.R, Style.Border, isActive ? Side.Down | Side.Left | Side.Right : Side.Up);

            gfx.DrawText(lay.TextR, tabName, Style.Font, textForeColor, textBackColor);
        }

        var lineX1 = lays.Last().R.Right;
        var lineX2 = r.Right;
        var lineY = lays.Last().R.Y;
        gfx.DrawLine(Style.Border, lineX1, lineY, lineX2, lineY);
    }





    private static R GetTileR(int width, Sz titleSz, int btnCount)
    {
        var x1 = titleSz.Width + 12;
        var x2 = width - btnCount * 15 - 10;
        if (x2 <= x1) return R.Empty;
        return new R(x1, 8, x2 - x1, 5);
    }
}