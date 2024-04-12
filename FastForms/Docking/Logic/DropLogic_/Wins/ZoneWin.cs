using FastForms.Docking.Logic.DropLogic_.Painting;
using FastForms.Docking.Logic.DropLogic_.Structs;
using FastForms.Utils.GdiUtils;
using FastForms.Utils.Win32;
using PowMaybe;
using PowRxVar;
using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.Structs;
using PowWin32.Windows.StructsPackets;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace FastForms.Docking.Logic.DropLogic_.Wins;


sealed class ZoneWin : IDisposable
{
	private static readonly WinClass Class = new(
		"ZoneWin",
		styles: User32.WindowClassStyles.CS_HREDRAW | User32.WindowClassStyles.CS_VREDRAW,
		hBrush: Gdi32.GetStockObject(Gdi32.StockObjectType.NULL_BRUSH)
	);
	private static readonly WinStylesDef Styles = new(
		User32.WindowStyles.WS_POPUP |
		User32.WindowStyles.WS_CLIPSIBLINGS | User32.WindowStyles.WS_CLIPCHILDREN,
		User32.WindowStylesEx.WS_EX_LAYERED
	);


	public void Dispose() => sys.Destroy();

	private readonly SysWin sys = new();
	private readonly Bitmap? bmp;

	public Zone Zone { get; }

	public ZoneWin(Zone zone, IRoVar<Maybe<Drop>> drop)
	{
		Zone = zone;
		bmp = zone.Bmp.GetZoneBmp();

		var dropSet = Zone.Drops.Select(e => e.Id).ToHashSet();
		var dropZone = drop.SelectVar(mayDrop => mayDrop.IsSome(out var drop_) switch
		{
			true => dropSet.Contains(drop_.Id) switch
			{
				false => May.None<Drop>(),
				true => May.Some(drop_)
			},
			false => May.None<Drop>(),
		}, sys.D);


		Class.CreateWindow(sys, Styles, zone.BmpR, 0, "ZoneWin");

		dropZone.Subscribe(Draw).D(sys.D);

		sys.Show();
	}

	public override string ToString() => $"{Zone}";

	private void Draw(Maybe<Drop> activeDrop)
	{
		if (bmp == null) return;
		LayeredWindowUtils.Paint(
			sys.Handle,
			sys.GetWinR(),
			gfx =>
			{
				gfx.DrawImage(bmp, 0, 0);
				foreach (var drop in Zone.Drops)
				{
					var isOn = activeDrop.IsSome(out var activeDrop_) && activeDrop_.Id == drop.Id;
					var brush = drop.Bmp.GetBtnBrush(isOn);
					if (brush != null)
					{
						using var _ = gfx.PushOffset(drop.R.Pos - Zone.BmpR.Pos);
						gfx.FillRect(drop.R.WithZeroPos(), brush);
					}
				}
			}
		);
	}
}

