using PowWin32.Geom;
using FastForms.Docking.Utils.Btns_;
using FastForms.Docking.Logic.DockerWin_.Structs;
using FastForms.Utils.GdiUtils;
using Style = FastForms.Docking.Logic.DockerWin_.Painting.DockerWinPainterStyle;
using FastForms.Structs;

namespace FastForms.Docking.Logic.DockerWin_.Painting;

static class DockerWinPainter
{
	public static void Paint(
		Graphics gfx,
		R r,
		string title,
		bool active,
		BtnSet<DockerWinSysBtnId> btns
	)
	{
		gfx.DrawRect(r, Style.BorderPen[active], Side.Up);
		gfx.FillRect(new R(0, 1, r.Width, DockerLayout.CaptionHeight), Style.BackBrush);

		gfx.DrawImage(Style.Icon[active], 12, 7);

		gfx.DrawText(r.Pos + new Pt(44, 10), title, Style.Font, Style.TitleForeColor[active], Style.TitleBackColor);

		btns.Paint(gfx, active);
	}
}