using Vanara.PInvoke;

namespace PowWin32.Geom;

public readonly record struct Sz(int Width, int Height)
{
	public static readonly Sz Empty = new(0, 0);
	public static readonly Sz MaxValue = new(int.MaxValue, int.MaxValue);

	public override string ToString() => $"{Width}x{Height}";

	public static Sz operator +(Sz a, Sz b) => new(a.Width + b.Width, a.Height + b.Height);
	//public static Sz operator -(Sz a, Sz b) => new(Math.Max(0, a.Width - b.Width), Math.Max(0, a.Height - b.Height));
	public static Sz operator -(Sz a, Sz b) => new(a.Width - b.Width, a.Height - b.Height);
	public static Sz operator *(Sz a, int z) => new(a.Width * z, a.Height * z);
	public static Sz operator *(Sz a, double z) => new((int)(a.Width * z), (int)(a.Height * z));
	public static Sz operator /(Sz a, double z) => new((int)(a.Width / z), (int)(a.Height / z));


	// Conversion operators
	// ====================
	public static implicit operator Sz(System.Drawing.Size e) => new(e.Width, e.Height);
	public static implicit operator System.Drawing.Size(Sz e) => new(e.Width, e.Height);
	public static implicit operator System.Drawing.SizeF(Sz e) => new(e.Width, e.Height);

	public static implicit operator Sz(SIZE e) => new(e.Width, e.Height);
	public static implicit operator SIZE(Sz e) => new(e.Width, e.Height);

	//public static implicit operator Sz(NetCoreEx.Geometry.Size e) => new(e.Width, e.Height);
	//public static implicit operator NetCoreEx.Geometry.Size(Sz e) => new(e.Width, e.Height);
	//public static implicit operator NetCoreEx.Geometry.SizeF(Sz e) => new(e.Width, e.Height);

	//public static implicit operator Sz(SKSizeI e) => new(e.Width, e.Height);
	//public static implicit operator SKSizeI(Sz e) => new(e.Width, e.Height);
	//public static implicit operator SKSize(Sz e) => new(e.Width, e.Height);
}
