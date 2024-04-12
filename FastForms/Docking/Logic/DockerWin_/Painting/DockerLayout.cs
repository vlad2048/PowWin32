using PowWin32.Geom;

namespace FastForms.Docking.Logic.DockerWin_.Painting;

public static class DockerLayout
{
    public static readonly Marg WinMargStd = new(0, 6, 6, 6);
    public static readonly Marg WinMargMax = new(8, 8, 8, 8);

    public const int CaptionHeight = 32;

    private static readonly Marg FrameMarg = new(CaptionHeight + 1, 0, 0, 0);

    public static R AdjustClientR(R r, bool isToolSingle) => isToolSingle switch
    {
	    true => r,
	    false => r - FrameMarg,
    };
}
