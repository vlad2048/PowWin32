using PowWin32.Geom;

namespace FastForms.Docking.Logic.DropLogic_.Structs;

sealed record RSet(R[] Rs)
{
	public static implicit operator RSet(R r) => new([r]);
}