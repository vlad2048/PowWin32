using FastForms.Docking.Logic.HolderWin_.Painting;
using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Utils.WinEventUtils;
using PowMaybe;
using PowWin32.Geom;
using PowWin32.Windows.StructsPackets;
using PowWin32.Windows.Utils;

namespace FastForms.Docking.Logic.DockerInteractions_;

static class HolderUndocking
{
    private const int JerkThreshold = 5;

    public static void Setup(
        HolderNode holderNode,
        Func<Pt, bool> isOverBtn
    )
    {
        var (state, sys) = (holderNode.State, holderNode.State.Sys);

        sys.MouseCapture(
            MouseButton.Left,

            mouse =>
            {
                if (state.IsHolderFrame.V) return May.None<Pt>();
                var inCaption = mouse.Y <= HolderLayout.CaptionHeight + 1;
                if (!inCaption || isOverBtn(mouse)) return May.None<Pt>();

                return May.Some(sys.Client2Screen(mouse));
            },

            (mouse, st, stop) =>
            {
                if ((mouse - st).IsFurtherThan(JerkThreshold))
                {
                    stop();
                    state.Docker.UndockHolder(holderNode, st);
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