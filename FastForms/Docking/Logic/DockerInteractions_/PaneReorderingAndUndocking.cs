using System.Reactive.Linq;
using FastForms.Docking.Logic.HolderWin_.Painting;
using FastForms.Utils;
using FastForms.Utils.WinEventUtils;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.StructsPackets;
using PowBasics.CollectionsExt;
using PowMaybe;
using PowRxVar;
using PowWin32.Geom;
using PowWin32.Windows.Utils;
using FastForms.Docking.Logic.HolderWin_;

namespace FastForms.Docking.Logic.DockerInteractions_;


static class PaneReorderingAndUndocking
{
    private static readonly Marg TearoffMarg = new(23, 23, 23, 23);

    private sealed record TabMoveState(
        Pane[] Panes,
        int IdxSrc,
        TabLabelLay[] Lays,
        Pt GrabPosScr
    )
    {
        public int IdxDst { get; set; } = IdxSrc;
        public R RMax => Lays.Select(e => e.R).Union() + TearoffMarg;
    }


    public static void Setup(SysWin sys, HolderState state)
    {
        var isCaptured = sys.MouseCapture(
            MouseButton.Left,

            mouse =>
            {
                var lays = GetLays(sys, state);
                var idx = lays.IndexOf(f => f.R.Contains(mouse));
                if (idx == -1) return May.None<TabMoveState>();
                state.Panes.SetIdx(idx);
                state.HoveredTab.V = null;

                var isGripped = lays[idx].GripR.Contains(mouse);
                if (!isGripped) return May.None<TabMoveState>();

                return May.Some(new TabMoveState(
                    state.Panes.Arr.V,
                    idx,
                    lays,
                    sys.Client2Screen(mouse)
                ));
            },

            (mouse, st, stop) =>
            {
                if (st.RMax.Contains(mouse))
                {
                    var idxDst = Math.Min(st.Lays.Select(e => e.R).Count(e => mouse.X >= e.Right), st.Lays.Length - 1);
                    if (idxDst != st.IdxDst)
                    {
                        state.Panes.Move(st.IdxDst, idxDst);
                        st.IdxDst = idxDst;
                    }
                }
                else
                {
                    stop();

                    var pane = st.Panes[st.IdxSrc];
                    var jerkLay = st.Lays[st.IdxSrc];

                    state.Docker.UndockPane(pane, jerkLay, st.GrabPosScr);
                }
            },

            _ =>
            {
            }
        );



        sys.WhenMouseMoveOrLeave()
            .Where(_ => !isCaptured())
            .Select(pt => pt.GetIdxContaining(HolderLayout.GetTabLabelRs(sys.ClientR, [.. state.Panes.Arr.V.Select(e => e.Name)]).Select(e => e.R)))
            .Subscribe(e => state.HoveredTab.V = e).D(sys.D);
    }


    private static TabLabelLay[] GetLays(SysWin sys, HolderState state) => HolderLayout.GetTabLabelRs(sys.ClientR, [.. state.Panes.Arr.V.Select(e => e.Name)]);


    private static IObservable<Pt?> WhenMouseMoveOrLeave(this SysWin sys) =>
        sys.Evt.WhenMouseMove.Select(e => (Pt?)e.Point).Merge(
            sys.Evt.WhenMouseLeave.Select(_ => (Pt?)null)
        );

    private static int? GetIdxContaining(this Pt? pt, IEnumerable<R> rs) => pt switch
    {
        null => null,
        not null => rs.IndexOfOrNull(r => r.Contains(pt.Value))
    };
}