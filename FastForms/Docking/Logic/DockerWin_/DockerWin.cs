using FastForms.Docking.Enums;
using FastForms.Docking.Logic.DockerWin_.Painting;
using FastForms.Docking.Logic.DockerWin_.Structs;
using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Docking.Utils;
using FastForms.Docking.Utils.Btns_;
using FastForms.Utils.GdiUtils;
using FastForms.Utils.Win32;
using PowRxVar;
using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.Structs;
using PowWin32.Windows.StructsPackets;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace FastForms.Docking.Logic.DockerWin_;

/*
public sealed class DockerWin
{
	private static readonly Gdi32.SafeHBRUSH BkgBrush = MkBrushGdi(0xEEEEF2);
	private static readonly WinClass Class = new(
        "DockerWin",
        styles: User32.WindowClassStyles.CS_HREDRAW | User32.WindowClassStyles.CS_VREDRAW,
		//hBrush: Gdi32.GetStockObject(Gdi32.StockObjectType.NULL_BRUSH)
		hBrush: BkgBrush
    );
    private static readonly WinStylesDef Styles = new(
        User32.WindowStyles.WS_CLIPSIBLINGS | User32.WindowStyles.WS_CLIPCHILDREN |
        User32.WindowStyles.WS_POPUP |
        User32.WindowStyles.WS_SYSMENU | User32.WindowStyles.WS_MAXIMIZEBOX | User32.WindowStyles.WS_MINIMIZEBOX | User32.WindowStyles.WS_SIZEBOX | User32.WindowStyles.WS_DLGFRAME | User32.WindowStyles.WS_BORDER,
        User32.WindowStylesEx.WS_EX_WINDOWEDGE
    );

    public Disp D { get; } = new(nameof(DockerWin));
	public SysWin Sys { get; }
	public Docker Docker { get; }


	public DockerWin(R r, TNod<INode> root)
	{
		var treeType = Var.Make(root.ComputeTreeType(false), D);
		Sys = new SysWin(D, clientR =>
		{
			var res = Layout.AdjustClientR(clientR, treeType.V is TreeType.ToolSingle);
			return res;
		});

		Sys.EnableMouseTracking();
		var isMaximized = Sys.GetIsMaximized();
        NcCalcTweaker.Tweak(Sys, () => isMaximized.V ? Layout.WinMargMax : Layout.WinMargStd);

		var btns = new BtnSet<DockerWinSysBtnId>(
			Sys,
			DockerWinPainterStyle.BtnStyle,
			() => new Pt(0, 1)
		);

		btns.WhenClicked.Subscribe(btn => btn.Execute(Sys)).D(Sys.D);

		HitTester.ForFrame(Sys, Layout.CaptionHeight + 1, btns.IsOverAnyButton);

		Sys.Evt.WhenPaint.Subs((ref PaintPacket e) =>
		{
			using var _ = e.Paint(out var gfx);
			if (treeType.V is not TreeType.ToolSingle)
			{
				DockerWinPainter.Paint(
					gfx,
					Sys.GetClientR(),
					"FastForms",
					Sys.IsActive(),
					btns
				);
			}
		});

		Class.CreateWindow(Sys, Styles, r, 0, "DockerWin");
		Docker = new Docker(Sys, root, false, treeType);

		Var.Merge(treeType, isMaximized).Subscribe(_ => btns.ShowButtons(DockerWinSysBtnUtils.GetBtns(treeType.V, isMaximized.V))).D(Sys.D);


		Sys.Show();
    }
}
*/