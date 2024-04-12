using FastForms.Docking.Logic.Layout_.Nodes;
using Vanara.PInvoke;

namespace FastForms.Docking.Logic.Layout_;

static class LayoutApplier
{
	private const User32.SetWindowPosFlags WinPosFlags =
		User32.SetWindowPosFlags.SWP_NOACTIVATE |
		User32.SetWindowPosFlags.SWP_NOOWNERZORDER | User32.SetWindowPosFlags.SWP_NOZORDER |
		User32.SetWindowPosFlags.SWP_SHOWWINDOW;

	public static void Apply(TNod<INode> root)
	{
		var holders = root.Select(e => e.V).OfType<HolderNode>().ToArray();
		var hdwp = User32.BeginDeferWindowPos(holders.Length);
		foreach (var holder in holders)
		{
			var r = holder.R;
			User32.DeferWindowPos(hdwp, holder.State.Sys.Handle, 0, r.X, r.Y, r.Width, r.Height, WinPosFlags);
		}
		User32.EndDeferWindowPos(hdwp.DangerousGetHandle());
	}
}