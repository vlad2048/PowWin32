using PowWin32.Geom;

namespace FastForms.Docking.Logic.DropLogic_.Structs;


enum ZoneBmp
{
	None,
	Single,
	Small,
	Big,
}


sealed record Zone(
	Docker Docker,
	string Id,
	RSet ZoneR,
	R BmpR,
	ZoneBmp Bmp,
	Drop[] Drops
)
{
	public override string ToString() => Id;
}


