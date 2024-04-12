using FastForms.Docking.Enums;
using FastForms.Docking.Logic.HolderWin_.Painting;
using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Utils.Win32;
using FastForms.Utils.WinEventUtils;
using PowMaybe;
using PowWin32.Geom;
using PowWin32.Windows.StructsPackets;
using PowWin32.Windows.Utils;

namespace FastForms.Docking.Logic.HolderWin_.Logic;

static class HolderJerker
{
	private const int JerkThreshold = 5;

	public static void Setup(
		HolderNode holderNode,
		Func<Pt, bool> isOverBtn
	)
	{
		var (state, sys, dockerSrc) = (holderNode.State, holderNode.State.Sys, holderNode.State.Docker);

		sys.MouseCapture(
			MouseButton.Left,

			mouse =>
			{
				if (state.TreeType.V == TreeType.ToolSingle) return May.None<Pt>();
				var inCaption = mouse.Y <= HolderLayout.CaptionHeight + 1;
				if (!inCaption || isOverBtn(mouse)) return May.None<Pt>();
				return May.Some(sys.Client2Screen(mouse));
			},

			(mouse, st, stop) =>
			{
				if ((mouse - st).IsFurtherThan(JerkThreshold))
				{
					stop();
					var dockerDst = dockerSrc.UndockHolder(holderNode);
					WinMoveInitiator.Start(dockerDst.Sys, st, () => { });
				}
			},

			_ =>
			{

			}
		);
	}
}


file static class HolderJerkerFileExt
{
	public static bool IsFurtherThan(this Pt v, int threshold) => Math.Abs(v.X) >= threshold || Math.Abs(v.Y) >= threshold;
}