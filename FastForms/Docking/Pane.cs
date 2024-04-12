using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Utils.GdiUtils;
using PowWin32.Diag;
using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.Structs;
using PowWin32.Windows.StructsPackets;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace FastForms.Docking;

public abstract class Pane
{
	private static readonly WinClass Class = new(
		"Pane",
		styles: User32.WindowClassStyles.CS_HREDRAW | User32.WindowClassStyles.CS_VREDRAW,
		hBrush: Gdi32.GetStockObject(Gdi32.StockObjectType.NULL_BRUSH)
	);

	private static readonly WinStylesDef Styles = new(
		User32.WindowStyles.WS_VISIBLE |
		User32.WindowStyles.WS_CLIPSIBLINGS | User32.WindowStyles.WS_CLIPCHILDREN |
		User32.WindowStyles.WS_CHILD,
		0
	);

	public NodeType Type { get; }
	public SysWin Sys { get; } = new();

	public string Name { get; }

	protected Pane(NodeType type, string name)
	{
		Type = type;
		Name = name;
	}

	internal void SetParent(SysWin holderWin, R r) => Sys.CreateMove(holderWin.Handle, r, Class, Styles);

	internal void SetR(R r) => Sys.SetWindowPos_MoveSize(r);

	internal void SetVisibility(bool visible)
	{
		User32.EnableWindow(Sys.Handle, visible);
		User32.ShowWindow(Sys.Handle, visible ? ShowWindowCommand.SW_SHOW : ShowWindowCommand.SW_HIDE);
	}
}

public class ToolPane : Pane
{
	public ToolPane(string name) : base(NodeType.Tool, name) {}
}

public class DocPane : Pane
{
	public DocPane(string name) : base(NodeType.Doc, name) {}
}



public sealed class SimplePane : ToolPane
{
	public SimplePane(string name, uint colorVal) : base(name)
	{
		var color = MkColor(colorVal);
		var textBrush = MkBrush(0);

		Sys.Evt.WhenPaint.Subs((ref PaintPacket e) =>
		{
			using var _ = e.Paint(out var gfx);
			gfx.Clear(color);
			gfx.DrawString(Name, SystemFonts.MessageBoxFont!, textBrush, 5, 5);
		});
	}
}