using FastForms.Docking.Logic.Layout_.Nodes;
using PowWin32.Windows;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace FastForms.Docking.Logic.Layout_;

static class LayoutApplier
{
	private const User32.SetWindowPosFlags ApplyFlags =
		User32.SetWindowPosFlags.SWP_NOACTIVATE |
		User32.SetWindowPosFlags.SWP_NOOWNERZORDER | User32.SetWindowPosFlags.SWP_NOZORDER |
		User32.SetWindowPosFlags.SWP_SHOWWINDOW | User32.SetWindowPosFlags.SWP_DRAWFRAME;

	public static void Apply(TNod<INode> root)
	{
		var holders = root.Select(e => e.V).OfType<HolderNode>().ToArray();
		var hdwp = User32.BeginDeferWindowPos(holders.Length);
		foreach (var holder in holders)
		{
			var r = holder.R;
			User32.DeferWindowPos(hdwp, holder.State.Sys.Handle, 0, r.X, r.Y, r.Width, r.Height, ApplyFlags);
		}
		User32.EndDeferWindowPos(hdwp.DangerousGetHandle());
	}



	private const User32.SetWindowPosFlags ApplyRedrawFlags =
		User32.SetWindowPosFlags.SWP_NOACTIVATE |
		User32.SetWindowPosFlags.SWP_NOOWNERZORDER | User32.SetWindowPosFlags.SWP_NOZORDER;

	private const User32.RedrawWindowFlags RedrawFlags =
		User32.RedrawWindowFlags.RDW_INVALIDATE |
		User32.RedrawWindowFlags.RDW_UPDATENOW |
		User32.RedrawWindowFlags.RDW_ALLCHILDREN;

	public static void ApplyRedraw(TNod<INode> root, SysWin sys)
	{
		var holders = root.Select(e => e.V).OfType<HolderNode>().ToArray();
		var hdwp = User32.BeginDeferWindowPos(holders.Length);
		foreach (var holder in holders)
		{
			var r = holder.R;
			User32.DeferWindowPos(hdwp, holder.State.Sys.Handle, 0, r.X, r.Y, r.Width, r.Height, ApplyRedrawFlags);
		}
		User32.EndDeferWindowPos(hdwp.DangerousGetHandle());

		sys.Redraw(RedrawFlags);
	}
}