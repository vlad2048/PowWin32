using PowWin32.Geom;

namespace FastForms.Docking.Utils.Btns_.Structs;

sealed class BtnSetStyle
{
	public Sz[] BmpSizes { get; }
    public IReadOnlyDictionary<(int, BtnMouseState, bool), BtnDrawRes> Map { get; }
    public int Gutter { get; }


    public BtnSetStyle(
	    Bitmap[] bmps,
	    Func<Bitmap, BtnMouseState, bool, BtnDrawRes> gen,
		int gutter
	)
    {
	    BmpSizes = [..bmps.Select(e => new Sz(e.Width, e.Height))];
		Map = (
			    from bmpIdx in bmps.Select((bmp, idx) => (bmp, idx))
			    from mouseState in Enum.GetValues<BtnMouseState>()
			    from active in Bools
			    select (
				    (bmpIdx.idx, mouseState, active),
				    gen(bmpIdx.bmp, mouseState, active)
			    )
		    )
		    .ToDictionary(e => e.Item1, e => e.Item2);

		Gutter = gutter;
    }



	private static readonly bool[] Bools = [false, true];
}
