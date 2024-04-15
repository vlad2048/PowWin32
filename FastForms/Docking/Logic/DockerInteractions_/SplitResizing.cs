using FastForms.Docking.Enums;
using FastForms.Docking.Logic.Layout_;
using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Docking.Structs;
using FastForms.Utils.WinEventUtils;
using PowMaybe;
using PowTrees.Algorithms;
using PowWin32.Geom;
using PowWin32.Windows.StructsPackets;
using Vanara.PInvoke;

namespace FastForms.Docking.Logic.DockerInteractions_;

static class SplitResizing
{
	private static readonly User32.SafeHCURSOR defaultCursor = User32.LoadCursor(default, User32.IDC_ARROW);
	private static readonly IReadOnlyDictionary<Dir, User32.SafeHCURSOR> curMap = new Dictionary<Dir, User32.SafeHCURSOR>
	{
		{ Dir.Horz, User32.LoadCursor(default, User32.IDC_SIZEWE) },
		{ Dir.Vert, User32.LoadCursor(default, User32.IDC_SIZENS) },
	};

	private sealed record St(
		SplitBar Bar,
		int Ofs,
		int PosMin,
		int PosMax
	)
	{
		public Dir Dir => Bar.Dir;
	}

	public static void Setup(Docker docker)
	{
		docker.Sys.MouseCaptureCursor(
			MouseButton.Left,

			mouse =>
			{
				var mg = docker.TreeType.V.GetMargin();
				var bars = docker.Root.GetBars(mg);
				var bar = bars.FirstOrDefault(e => e.R.Contains(mouse));
				return bar switch
				{
					null => May.None<User32.SafeHCURSOR>(),
					not null => May.Some(curMap[bar.Split.Dir])
				};
			},

			mouse =>
			{
				var mg = docker.TreeType.V.GetMargin();
				var bars = docker.Root.GetBars(mg);
				var bar = bars.FirstOrDefault(e => e.R.Contains(mouse));

				if (bar == null) throw new ArgumentException("Shouldn't be possible");

				User32.SetCursor(curMap[bar.Split.Dir]);

				var ofs = bar.Split.Pos;
				var (min, max) = bar.Split.GetPosBounds(mg);

				var mouseLoc = mouse.Dir(bar.Dir) - bar.R.Pos.Dir(bar.Dir);

				return new St(
					bar,
					ofs - mouseLoc,
					min,
					max
				);
			},

			(mouse, st) =>
			{
				var mouseLoc = mouse.Dir(st.Dir) - st.Bar.R.Pos.Dir(st.Dir);

				var posNext = mouseLoc + st.Ofs;
				var posNextClamped = posNext.Clamp(st.PosMin, st.PosMax);
				st.Bar.Split.Pos = posNextClamped;
				docker.TriggerTreeMod(new SplitResizeTreeMod());
			}
		);
	}
}



sealed record SplitBar(R R, SplitNode Split)
{
	public Dir Dir => Split.Dir;
}

file static class SplitBarGetter
{
	/*public static int GetMid(this SplitNode split) =>
		split.Dir switch
		{
			Dir.Horz => split.R.X + split.Pos,
			Dir.Vert => split.R.Y + split.Pos,
			_ => throw new ArgumentException()
		};*/

	public static SplitBar[] GetBars(this TNod<INode> root, int mg)
	{
		mg /= 2;
		return [..root.OfTypeNod<INode, SplitNode>().Select(e => e.Get(mg))];
	}

	public static SplitBar Get(this SplitNode split, int mg)
	{
		var (r, p) = (split.R, split.Pos);
		return split.Dir switch
		{
			Dir.Horz => new SplitBar(new R(
				r.X + p - mg,
				r.Y,
				2 * mg,
				r.Height
			), split),
			Dir.Vert => new SplitBar(new R(
				r.X,
				r.Y + p - mg,
				r.Width,
				2 * mg
			), split),
			_ => throw new ArgumentException()
		};
	}
}